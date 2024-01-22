using MapsterMapper;
using Microsoft.AspNetCore.Http;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.AuthModels.Request;
using PerfectBreakfast.Application.Models.UserModels.Request;
using PerfectBreakfast.Application.Models.UserModels.Response;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IClaimsService _claimsService;
    private readonly JWTService _jwtService;
    private readonly ICurrentTime _currentTime;
    private readonly IImgurService _imgurService;

    public UserService(IUnitOfWork unitOfWork
        ,IMapper mapper
        ,IClaimsService claimsService
        ,JWTService jwtService
        ,ICurrentTime currentTime
        ,IImgurService imgurService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _claimsService = claimsService;
        _jwtService = jwtService;
        _currentTime = currentTime;
        _imgurService = imgurService;
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
            result.Payload = await _jwtService.CreateJWT(user);
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
                result.AddError(ErrorCode.ServerError,identityResult.Errors.Select(x => x.Description).ToString());
                return result;
            }
            var identityRe = await _unitOfWork.UserManager.AddToRoleAsync(user, "CUSTOMER");
            if (!identityRe.Succeeded)
            {
                result.AddError(ErrorCode.ServerError,identityRe.Errors.Select(x => x.Description).ToString());
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

    public async Task<OperationResult<UserLoginResponse>> RefreshUserToken()
    {
        var result = new OperationResult<UserLoginResponse>();
        try
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(_claimsService.GetCurrentUserId);
            result.Payload = await _jwtService.CreateJWT(user);
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
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId,x => x.Company);
            var userDetailResponse = _mapper.Map<UserDetailResponse>(user);
            userDetailResponse = userDetailResponse with { CompanyName = user.Company.Name };
            result.Payload = userDetailResponse;
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
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
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

    public async Task<OperationResult<UserResponse>> CreateUser(CreateUserRequestModel requestModel)
    {
        var result = new OperationResult<UserResponse>();
        try
        {
            var user = _mapper.Map<User>(requestModel);
            
            // check User workspace to generate code\
            if (user.DeliveryUnitId.HasValue)
            {
                user.Code = await _unitOfWork.UserRepository.CalculateDeliveryUnitCode(user.DeliveryUnitId.Value);
            }
            else if (user.ManagementUnitId.HasValue)
            {
                user.Code = await _unitOfWork.UserRepository.CalculateManagementUnitCode(user.ManagementUnitId.Value);
            }
            else if (user.SupplierId.HasValue)
            {
                user.Code = await _unitOfWork.UserRepository.CalculateSupplierCode(user.SupplierId.Value);
            }
            user.Image = await _imgurService.UploadImageAsync(requestModel.Image);
            user.UserName = requestModel.Email;
            user.EmailConfirmed = true;
            user.CreationDate = _currentTime.GetCurrentTime();
            await _unitOfWork.UserManager.CreateAsync(user,"123456");
            await _unitOfWork.UserManager.AddToRoleAsync(user, requestModel.RoleName);
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
            _mapper.Map(requestModel, user);
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
}