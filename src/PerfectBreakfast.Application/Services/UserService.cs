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
using System.Net;
using System.Net.Mail;
using System.Web;
using Microsoft.EntityFrameworkCore;
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

    public UserService(IUnitOfWork unitOfWork
        ,IMapper mapper
        ,IClaimsService claimsService
        ,JWTService jwtService
        ,ICurrentTime currentTime
        ,IImgurService imgurService
        , IMailService mailService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _claimsService = claimsService;
        _jwtService = jwtService;
        _currentTime = currentTime;
        _imgurService = imgurService;
        _mailService = mailService;
    }

    public async Task<OperationResult<UserLoginResponse>> SignIn(SignInModel request)
    {
        var result = new OperationResult<UserLoginResponse>();
        try
        {
            var user = await _unitOfWork.UserRepository.FindSingleAsync(x => x.UserName == request.Email);
            if (user is null)
            {
                result.AddError(ErrorCode.UnAuthorize,"wrong email");
                return result;
            }
            var signInResult = await _unitOfWork.SignInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!signInResult.Succeeded)
            {
                if (signInResult.IsNotAllowed)
                {
                    result.AddError(ErrorCode.UnAuthorize,"need confirm account");
                    return result;
                }
                result.AddError(ErrorCode.UnAuthorize,"wrong pass");
                return result;
            }
            
            user.RefreshToken = user.RandomRefreshToken();             // tao RefreshToken mới
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMonths(2);  // tạo ngày hết hạn mới là sau 2 tháng 
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

    public async Task<OperationResult<UserLoginResponse>> DeliveryStaffSignIn(SignInModel request)
    {
        var result = new OperationResult<UserLoginResponse>();
        try
        {
            var user = await _unitOfWork.UserRepository.FindSingleAsync(x => x.UserName == request.Email);
            if (user is null)
            {
                result.AddError(ErrorCode.UnAuthorize,"wrong email");
                return result;
            }
            // check role account 
            if (!await _unitOfWork.UserManager.IsInRoleAsync(user, ConstantRole.DELIVERY_STAFF))
            {
                result.AddError(ErrorCode.NotFound,"Đây không phải Account DELIVERY_STAFF");
                return result;
            }
            var signInResult = await _unitOfWork.SignInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!signInResult.Succeeded)
            {
                if (signInResult.IsNotAllowed)
                {
                    result.AddError(ErrorCode.UnAuthorize,"need confirm account");
                    return result;
                }
                result.AddError(ErrorCode.UnAuthorize,"wrong pass");
                return result;
            }
            
            user.RefreshToken = user.RandomRefreshToken();             // tao RefreshToken mới
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMonths(2);  // tạo ngày hết hạn mới là sau 2 tháng 
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangeAsync();
            result.Payload = await _jwtService.CreateJWT(user,user.RefreshToken!);
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
                result.AddError(ErrorCode.BadRequest,identityResult.Errors.Select(x => x.Description.ToString()).First());
                return result;
            }
            var identityRe = await _unitOfWork.UserManager.AddToRoleAsync(user, ConstantRole.CUSTOMER);
            if (!identityRe.Succeeded)
            {
                result.AddError(ErrorCode.ServerError,identityRe.Errors.Select(x => x.Description.ToString()).First());
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
            user.RefreshToken = user.RandomRefreshToken(); 
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangeAsync();
            result.Payload = await _jwtService.CreateJWT(user,user.RefreshToken!);
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
            // xác định các thuộc tính include và theninclude 
            var companyInclude = new IncludeInfo<User>
            {
                NavigationProperty = c => c.Company
            };
            // xác định các thuộc tính include và theninclude 
            var supplierInclude = new IncludeInfo<User>
            {
                NavigationProperty = c => c.Supplier
            };
            // xác định các thuộc tính include và theninclude 
            var deliveryUnitInclude = new IncludeInfo<User>
            {
                NavigationProperty = c => c.Delivery
            };
            // xác định các thuộc tính include và theninclude 
            var managementUnitInclude = new IncludeInfo<User>
            {
                NavigationProperty = c => c.Partner
            };
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId,companyInclude,supplierInclude,deliveryUnitInclude,managementUnitInclude);
            string unitName = user.Company?.Name 
                              ?? user.Delivery?.Name 
                              ?? user.Partner?.Name 
                              ?? user.Supplier?.Name
                              ?? "Perfect Breakfast";
            var roles = await _unitOfWork.UserManager.GetRolesAsync(user);
            var userDetailResponse = _mapper.Map<UserDetailResponse>(user);
            userDetailResponse = userDetailResponse with { CompanyName = unitName };
            userDetailResponse = userDetailResponse with { Roles = roles };
            result.Payload = userDetailResponse;
        }
        catch (NotFoundIdException e)
        {
            result.AddError(ErrorCode.NotFound,"Not Found by Id");
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
                    result.AddError(ErrorCode.BadRequest, "Reset fail");
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
                var token = await _unitOfWork.UserManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = HttpUtility.UrlEncode(token);
                
                // Tạo dữ liệu email, sử dụng token trong nội dung email
                var mailData = new MailDataViewModel(
                    to: new List<string> { email },
                    subject: "Reset Password",
                    body: $"Please use this token to reset your password: {token}"
                );
                CancellationToken ct = new CancellationToken();
                
                // Gửi email và xử lý kết quả
                bool sendResult = await _mailService.SendAsync(mailData, ct);
                if (sendResult)
                {
                    result.Payload = token;
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
        var userId =  _claimsService.GetCurrentUserId;
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
            result.AddError(ErrorCode.NotFound,"User is not exist");
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

    public async Task<OperationResult<Pagination<UserResponse>>> GetUserPaginationAsync(int pageIndex = 0, int pageSize = 10)
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
            result.AddError(ErrorCode.NotFound,e.Message);
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
            user.Image = await _imgurService.UploadImageAsync(requestModel.Image);
            user.UserName = requestModel.Email;
            user.EmailConfirmed = true;
            user.CreationDate = _currentTime.GetCurrentTime();
            var identityResult = await _unitOfWork.UserManager.CreateAsync(user,"123456");
            if (!identityResult.Succeeded)
            {
                result.AddError(ErrorCode.BadRequest,identityResult.Errors.Select(x => x.Description.ToString()).First());
                return result;
            }
            var identityRe = await _unitOfWork.UserManager.AddToRoleAsync(user, requestModel.RoleName);
            if (!identityRe.Succeeded)
            {
                result.AddError(ErrorCode.BadRequest,identityResult.Errors.Select(x => x.Description.ToString()).First());
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

    public async Task<OperationResult<bool>> UpdateUser(Guid id,UpdateUserRequestModel requestModel)
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
            if (deliveryAdmin == null) { }
            // Tạo biểu thức tìm kiếm (predicate)
            Expression<Func<User, bool>>? perdicate =  u => u.DeliveryId == deliveryAdmin.DeliveryId;
             
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
            var users = await _unitOfWork.UserRepository.FindAll(x => x.DeliveryId == deliveryAdmin.DeliveryId).ToListAsync();
            result.Payload = _mapper.Map<List<UserResponse>>(users);

        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }
}