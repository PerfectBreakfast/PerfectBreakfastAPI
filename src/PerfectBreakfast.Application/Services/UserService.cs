using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.AuthModels.Request;
using PerfectBreakfast.Application.Models.UserModels.Request;
using PerfectBreakfast.Application.Models.UserModels.Response;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IClaimsService _claimsService;
    private readonly JWTService _jwtService;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentTime _currentTime;

    public UserService(IUnitOfWork unitOfWork
        ,IMapper mapper
        ,IClaimsService claimsService
        ,JWTService jwtService
        ,IUserRepository userRepository
        ,ICurrentTime currentTime)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _claimsService = claimsService;
        _jwtService = jwtService;
        _userRepository = userRepository;
        _currentTime = currentTime;
    }

    public async Task<OperationResult<UserLoginResponse>> SignIn(SignInModel request)
    {
        var result = new OperationResult<UserLoginResponse>();
        try
        {
            var user = await _userRepository.FindSingleAsync(x => x.UserName == request.Email);
            if (user is null)
            {
                result.AddError(ErrorCode.UnAuthorize,"wrong email");
                return result;
            }
            var isSuccess = await _userRepository
                .CheckPasswordSignin(user, request.Password,false);
            if (!isSuccess.Succeeded)
            {
                if (isSuccess.IsNotAllowed)
                {
                    result.AddError(ErrorCode.UnAuthorize,"need confirm account");
                    return result;
                }
                result.AddError(ErrorCode.UnAuthorize,"wrong pass");
                return result;
            }

            var token = await _jwtService.CreateJWT(user);
            result.Payload = new UserLoginResponse(token);
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
            if(!request.RoleId.HasValue)
                
            // check User workspace to generate code
            if (user.CompanyId.HasValue)
            {
                user.Code = await _userRepository.CalculateCompanyCode(user.CompanyId.Value);
            }
            else if (user.DeliveryUnitId.HasValue)
            {
                user.Code = await _userRepository.CalculateDeliveryUnitCode(user.DeliveryUnitId.Value);
            }
            else if (user.ManagementUnitId.HasValue)
            {
                user.Code = await _userRepository.CalculateManagementUnitCode(user.ManagementUnitId.Value);
            }
            else if (user.SupplierId.HasValue)
            {
                user.Code = await _userRepository.CalculateSupplierCode(user.SupplierId.Value);
            }
            user.UserName = request.Email;
            user.EmailConfirmed = true;
            user.CreationDate = _currentTime.GetCurrentTime();

            result.Payload = await _userRepository.AddAsync(user, request.Password);
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
            var user = await _userRepository.GetByIdAsync(_claimsService.GetCurrentUserId);
            var token = await _jwtService.CreateJWT(user);
            result.Payload = new UserLoginResponse(token); 
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
            var users = await _userRepository.GetAllAsync();
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
            var users = await _userRepository.ToPagination(pageIndex, pageSize);
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
            var user = await _userRepository.GetByIdAsync(id);
            result.Payload = _mapper.Map<UserResponse>(user);
        }
        /*catch (NotFoundIdException e)
        {
            result.AddError(ErrorCode.NotFound,e.Message);
        }*/
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

            // check User workspace to generate code
            /*if (user.CompanyId.HasValue)
            {
                user.Code = await _unitOfWork.UserRepository.CalculateCompanyCode(user.CompanyId.Value);
            }
            else if (user.DeliveryUnitId.HasValue)
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
            await _unitOfWork.UserRepository.AddAsync(user);*/
            await _unitOfWork.SaveChangeAsync();
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }
}