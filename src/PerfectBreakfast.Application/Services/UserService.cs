using MapsterMapper;
using Microsoft.AspNetCore.Http;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.AuthModels.Request;
using PerfectBreakfast.Application.Models.UserModels.Request;
using PerfectBreakfast.Application.Models.UserModels.Response;
using PerfectBreakfast.Domain.Entities;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PerfectBreakfast.Application.Models.AuthModels.googleModels;
using PerfectBreakfast.Application.Models.MailModels;
using PerfectBreakfast.Application.Utils;

namespace PerfectBreakfast.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IClaimsService _claimsService;
    private readonly JWTService _jwtService;
    private readonly ICurrentTime _currentTime;
    private readonly IImgurService _imgurService;
    private readonly IMailService _mailService;
    private readonly AppConfiguration _appConfiguration;

    public UserService(IUnitOfWork unitOfWork
        , IMapper mapper
        , IClaimsService claimsService
        , JWTService jwtService
        , ICurrentTime currentTime
        , IImgurService imgurService
        , IMailService mailService
        , AppConfiguration appConfiguration)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _claimsService = claimsService;
        _jwtService = jwtService;
        _currentTime = currentTime;
        _imgurService = imgurService;
        _mailService = mailService;
        _appConfiguration = appConfiguration;
    }

    public async Task<OperationResult<UserLoginResponse>> SignIn(SignInModel request)
    {
        var result = new OperationResult<UserLoginResponse>();
        try
        {
            var user = await _unitOfWork.UserRepository.GetUserByEmail(request.Email);
            if (user is null)
            {
                result.AddError(ErrorCode.UnAuthorize, "Email không tồn tại");
                return result;
            }
            // check đúng role là customer mới được 
            var isCustomer = user.UserRoles != null && user.UserRoles.Any(x => x.Role.Name == ConstantRole.CUSTOMER);
            if (!isCustomer)
            {
                result.AddError(ErrorCode.UnAuthorize, "Role không hợp lệ");
                return result;
            }

            var signInResult = await _unitOfWork.SignInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!signInResult.Succeeded)
            {
                if (signInResult.IsNotAllowed)
                {
                    result.AddError(ErrorCode.UnAuthorize, "need confirm account");
                    return result;
                }

                result.AddError(ErrorCode.UnAuthorize, "Sai mật Khẩu");
                return result;
            }

            user.RefreshToken = GenerateRefreshToken.RandomRefreshToken(); // tao RefreshToken mới
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMonths(2); // tạo ngày hết hạn mới là sau 2 tháng 
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangeAsync();
            result.Payload = await _jwtService.CreateJWT(user, user.RefreshToken!);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<UserLoginResponse>> ExternalLogin(string code)
    {
        var result = new OperationResult<UserLoginResponse>();
        try
        {
            var tokenResponse  = await ExchangeCodeForTokensAsync(code);
            if (!tokenResponse.IsSuccessStatusCode)
            {
                result.AddError(ErrorCode.BadRequest,tokenResponse.Content.ReadAsStringAsync().Result);
                return result;
            }
            var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
            var tokens = JsonConvert.DeserializeObject<GoogleTokenResponse>(tokenContent);
            var userInfoResponse = await GetUserInfoGoogle(tokens.AccessToken);
            if (!userInfoResponse.IsSuccessStatusCode)
            {
                result.AddError(ErrorCode.BadRequest,userInfoResponse.Content.ReadAsStringAsync().Result);
                return result;
            }

            var userInfo = await userInfoResponse.Content.ReadAsStringAsync();
            var googleUserInfo = JsonConvert.DeserializeObject<GoogleUserInfo>(userInfo);

             var info = new UserLoginInfo("GOOGLE", googleUserInfo.Id, "GOOGLE");
            
             var user = await _unitOfWork.UserManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
             if (user == null)
             {
                 user = await _unitOfWork.UserRepository.GetUserByEmail(googleUserInfo.Email);
                 if (user == null)
                 {
                     user = new User
                     {
                         Email = googleUserInfo.Email, 
                         UserName = googleUserInfo.Email ,
                         Name = googleUserInfo.Name,
                         EmailConfirmed = true,
                         CreationDate = _currentTime.GetCurrentTime(),
                         RefreshToken = GenerateRefreshToken.RandomRefreshToken(), // tao RefreshToken mới
                         RefreshTokenExpiryTime = DateTime.UtcNow.AddMonths(2), // tạo ngày hết hạn mới là sau 2 tháng 
                         // xử lý thêm ở đây vẫn còn thiếu
                     };
                     await _unitOfWork.UserManager.CreateAsync(user);
                     //prepare and send an email for the email confirmation
                     await _unitOfWork.UserManager.AddToRoleAsync(user, ConstantRole.CUSTOMER);
                     await _unitOfWork.UserManager.AddLoginAsync(user, info);
                 }
                 else
                 {
                     await _unitOfWork.UserManager.AddLoginAsync(user, info);
                 }
             }
            result.Payload = await _jwtService.CreateJWT(user, user.RefreshToken!);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<UserLoginResponse>> ManagementLogin(ManagementLoginModel request)
    {
        var result = new OperationResult<UserLoginResponse>();
        try
        {
            //var user = await _unitOfWork.UserManager.FindByEmailAsync(request.Email);
            var user = await _unitOfWork.UserRepository.GetUserByEmail(request.Email);
            if (user is null)
            {
                result.AddError(ErrorCode.UnAuthorize, "Email không tồn tại");
                return result;
            }

            var hasRole = user.UserRoles != null && user.UserRoles.Any(x => x.RoleId == request.RoleId);
            // check role account 
            if (!hasRole)
            {
                result.AddError(ErrorCode.NotFound, "Sai Role!!!");
                return result;
            }

            var signInResult = await _unitOfWork.SignInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!signInResult.Succeeded)
            {
                if (signInResult.IsNotAllowed)
                {
                    result.AddError(ErrorCode.UnAuthorize, "need confirm account");
                    return result;
                }

                result.AddError(ErrorCode.UnAuthorize, "Sai mật khẩu");
                return result;
            }

            user.RefreshToken = GenerateRefreshToken.RandomRefreshToken(); // tao RefreshToken mới
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMonths(2); // tạo ngày hết hạn mới là sau 2 tháng 
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangeAsync();
            result.Payload = await _jwtService.CreateJWT(user, user.RefreshToken!);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<bool>> SignUp(SignUpModel request)
    {
        var result = new OperationResult<bool>();
        try
        {
            var user = _mapper.Map<User>(request);

            // check User workspace to generate code
            if (user.CompanyId.HasValue)
            {
                user.Code = await _unitOfWork.UserRepository.CalculateCompanyCode(user.CompanyId.Value);
            }
            
            user.UserName = request.Email;
            user.EmailConfirmed = true;
            user.CreationDate = _currentTime.GetCurrentTime();
            var identityResult = await _unitOfWork.UserManager.CreateAsync(user, request.Password);
            if (!identityResult.Succeeded)
            {
                result.AddError(ErrorCode.BadRequest,
                    identityResult.Errors.Select(x => x.Description.ToString()).First());
                return result;
            }

            var identityRe = await _unitOfWork.UserManager.AddToRoleAsync(user, ConstantRole.CUSTOMER);
            if (!identityRe.Succeeded)
            {
                result.AddError(ErrorCode.ServerError, identityRe.Errors.Select(x => x.Description.ToString()).First());
                return result;
            }

            result.Payload = identityRe.Succeeded;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<UserLoginResponse>> RefreshUserToken(TokenModel tokenModel)
    {
        var result = new OperationResult<UserLoginResponse>();
        try
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(tokenModel.AccessToken);
            var userName = principal.Identity.Name;

            var user = await _unitOfWork.UserRepository.FindSingleAsync(x => x.UserName == userName);
            if (user is null || user.RefreshToken != tokenModel.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                result.AddError(ErrorCode.BadRequest, "Invalid client request");
            }

            user.RefreshToken = GenerateRefreshToken.RandomRefreshToken();
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangeAsync();
            result.Payload = await _jwtService.CreateJWT(user, user.RefreshToken!);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<UserDetailResponse>> GetCurrentUser()
    {
        var result = new OperationResult<UserDetailResponse>();
        try
        {
            var userId = _claimsService.GetCurrentUserId;
            var user = await _unitOfWork.UserRepository.GetInfoCurrentUserById(userId);
            var userDetailResponse = _mapper.Map<UserDetailResponse>(user);
            var unitName = user.Company?.Name
                              ?? user.Delivery?.Name
                              ?? user.Partner?.Name
                              ?? user.Supplier?.Name
                              ?? "Chưa xác định";
            var phoneNumber = user.PhoneNumber ?? "Chưa xác định";
            userDetailResponse = userDetailResponse with { CompanyName = unitName, PhoneNumber = phoneNumber, Roles = user.UserRoles.Select(x => x.Role.Name).ToList()};
            result.Payload = userDetailResponse;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<UserLoginResponse>> ResetPassword(ResetPasswordRequest request)
    {
        var result = new OperationResult<UserLoginResponse>();
        try
        {
            var user = await _unitOfWork.UserManager.FindByEmailAsync(request.Email);

            if (user != null)
            {
                var reset = await _unitOfWork.UserManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
                if (reset.Succeeded)
                {
                    result.Payload = _mapper.Map<UserLoginResponse>(user);
                }
                else
                {
                    result.AddError(ErrorCode.BadRequest, "Đổi mật khẩu thất bại");
                }
            }
            else
            {
                result.AddError(ErrorCode.BadRequest, "Email không tồn tại");
            }
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<string>> GeneratePasswordResetToken(string email)
    {
        var result = new OperationResult<string>();
        try
        {
            var user = await _unitOfWork.UserManager.FindByEmailAsync(email);

            if (user is not null)
            {
                var clientHost = _appConfiguration.Host;
                var token = await _unitOfWork.UserManager.GeneratePasswordResetTokenAsync(user);

                // Tạo dữ liệu email, sử dụng token trong nội dung email
                var mailData = new MailDataViewModel(
                    to: [email],
                    subject: "Reset Password",
                    body: $"Bấm để đổi mật khẩu: {clientHost}/reset-password?token={token}&email={email}"
                );
                var ct = new CancellationToken();

                // Gửi email và xử lý kết quả
                var sendResult = await _mailService.SendAsync(mailData, ct);
                if (sendResult)
                {
                    result.Payload = $"Bấm để đổi mật khẩu: {clientHost}/reset-password?token={token}&email?={email}";
                }
                else
                {
                    // Ghi nhận lỗi khi gửi email không thành công
                    result.AddError(ErrorCode.BadRequest, "Failed to send email.");
                }
            }
            else
            {
                result.AddError(ErrorCode.BadRequest, "Email không tồn tại");
            }
        }
        catch (Exception e)
        {
            result.AddUnknownError($"Lỗi gửi mail: {e.Message}");
            Console.WriteLine(e.StackTrace);
        }

        return result;
    }

    public async Task<OperationResult<bool>> ChangePassword(string currentPassword, string newPassword)
    {
        var result = new OperationResult<bool>();
        var userId = _claimsService.GetCurrentUserId;
        try
        {
            // Kiểm tra xem mật khẩu cũ và mới có giống nhau không
            if (currentPassword == newPassword)
            {
                result.AddError(ErrorCode.BadRequest, "Mật khẩu mới không được trùng với mật khẩu cũ.");
            }

            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId);
            var identityResult = await _unitOfWork.UserManager.ChangePasswordAsync(user, currentPassword, newPassword);
            result.Payload = identityResult.Succeeded;
        }
        catch (NotFoundIdException)
        {
            result.AddError(ErrorCode.NotFound, "User is not exist");
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<List<UserResponse>>> GetUsers()
    {
        var result = new OperationResult<List<UserResponse>>();
        try
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync();
            result.Payload = _mapper.Map<List<UserResponse>>(users);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<Pagination<UserResponse>>> GetUserPaginationAsync(int pageIndex = 0,
        int pageSize = 10)
    {
        var result = new OperationResult<Pagination<UserResponse>>();
        try
        {
            var users = await _unitOfWork.UserRepository.ToPagination(pageIndex, pageSize);
            result.Payload = _mapper.Map<Pagination<UserResponse>>(users);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<UserResponse>> GetUser(Guid id)
    {
        var result = new OperationResult<UserResponse>();
        try
        {
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(id);
            result.Payload = _mapper.Map<UserResponse>(user);
        }
        catch (NotFoundIdException e)
        {
            result.AddError(ErrorCode.NotFound, e.Message);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<bool>> CreateUser(CreateUserRequestModel requestModel)
    {
        var result = new OperationResult<bool>();
        try
        {
            var user = _mapper.Map<User>(requestModel);

            // check User workspace to generate code\
            if (user.DeliveryId.HasValue)
            {
                user.Code = await _unitOfWork.UserRepository.CalculateDeliveryCode(user.DeliveryId.Value);
            }
            else if (user.PartnerId.HasValue)
            {
                user.Code = await _unitOfWork.UserRepository.CalculatePartnerCode(user.PartnerId.Value);
            }
            else if (user.SupplierId.HasValue)
            {
                user.Code = await _unitOfWork.UserRepository.CalculateSupplierCode(user.SupplierId.Value);
            }

            if (requestModel.Image is not null)
            {
                user.Image = await _imgurService.UploadImageAsync(requestModel.Image);
            }
            user.UserName = requestModel.Email;
            user.EmailConfirmed = true;
            user.CreationDate = _currentTime.GetCurrentTime();
            var identityResult = await _unitOfWork.UserManager.CreateAsync(user, "123456");
            if (!identityResult.Succeeded)
            {
                result.AddError(ErrorCode.BadRequest,
                    identityResult.Errors.Select(x => x.Description.ToString()).First());
                return result;
            }

            var identityRe = await _unitOfWork.UserManager.AddToRoleAsync(user, requestModel.RoleName);
            if (!identityRe.Succeeded)
            {
                result.AddError(ErrorCode.BadRequest,
                    identityResult.Errors.Select(x => x.Description.ToString()).First());
                return result;
            }

            result.Payload = true;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<bool>> UpdateUser(Guid id, UpdateUserRequestModel requestModel)
    {
        var result = new OperationResult<bool>();
        try
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            //_mapper.Map(requestModel, user);
            user.Name = requestModel.Name ?? user.Name;
            user.PhoneNumber = requestModel.PhoneNumber ?? user.PhoneNumber;
            if (requestModel.Image is not null)
            {
                user.Image = await _imgurService.UploadImageAsync(requestModel.Image);
            }

            _unitOfWork.UserRepository.Update(user);
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            result.Payload = isSuccess;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<bool>> UpdateImageUser(Guid id, IFormFile image)
    {
        var result = new OperationResult<bool>();
        try
        {
            // hàm này đang fix 
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            user.Image = await _imgurService.UploadImageAsync(image);
            _unitOfWork.UserRepository.Update(user);
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            result.Payload = isSuccess;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<dynamic>> GetDeliveryStaffByDeliveryAdmin(int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<dynamic>();
        var deliveryAdminId = _claimsService.GetCurrentUserId;
        try
        {
            var deliveryInclude = new IncludeInfo<User>
            {
                NavigationProperty = c => c.Delivery
            };
            var deliveryAdmin = await _unitOfWork.UserRepository.GetUserByIdAsync(deliveryAdminId, deliveryInclude);
            if (deliveryAdmin == null)
            {
            }

            // Tạo biểu thức tìm kiếm (predicate)
            Expression<Func<User, bool>>? perdicate = u => u.DeliveryId == deliveryAdmin.DeliveryId;

            var userPages = await _unitOfWork.UserRepository.ToPagination(pageIndex, pageSize, perdicate);

            var pagination = _mapper.Map<Pagination<UserResponse>>(userPages);
            result.Payload = new { DeliveryId = deliveryAdmin.DeliveryId, pagination };
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<List<UserResponse>>> GetDeliveryStaffByDeliveryAdminList()
    {
        var result = new OperationResult<List<UserResponse>>();
        var deliveryAdminId = _claimsService.GetCurrentUserId;
        try
        {
            var deliveryInclude = new IncludeInfo<User>
            {
                NavigationProperty = c => c.Delivery
            };
            var deliveryAdmin = await _unitOfWork.UserRepository.GetUserByIdAsync(deliveryAdminId, deliveryInclude);
            var users = await _unitOfWork.UserRepository.FindAll(x => x.DeliveryId == deliveryAdmin.DeliveryId)
                .ToListAsync();
            result.Payload = _mapper.Map<List<UserResponse>>(users);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    // đổi code lấy token 
    private async Task<HttpResponseMessage> ExchangeCodeForTokensAsync(string code)
    {
        // Chuẩn bị data để gửi trong POST request
        var postData = new Dictionary<string, string>
        {
            { "code", code },
            { "client_id", _appConfiguration.Google.ClientId },
            { "client_secret", _appConfiguration.Google.ClientSecret },
            { "redirect_uri", _appConfiguration.Host },
            { "grant_type", "authorization_code" }
        };

        var requestContent = new FormUrlEncodedContent(postData);
        using var httpClient = new HttpClient();
        // Gửi request đến Google's token endpoint để trao đổi code lấy tokens
        var response = await httpClient.PostAsync("https://oauth2.googleapis.com/token", requestContent);

        return response;
    }
    
    // lấy thông tin User từ Google
    private async Task<HttpResponseMessage> GetUserInfoGoogle(string accessToken)
    {
        using var httpClient = new HttpClient();
        var userInfoResponse = await httpClient.GetAsync($"https://www.googleapis.com/oauth2/v2/userinfo?access_token={accessToken}");
        return userInfoResponse;
    }
}