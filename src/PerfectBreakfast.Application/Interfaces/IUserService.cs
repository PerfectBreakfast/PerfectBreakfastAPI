using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.AuthModels.Request;
using PerfectBreakfast.Application.Models.UserModels.Request;
using PerfectBreakfast.Application.Models.UserModels.Response;

namespace PerfectBreakfast.Application.Interfaces;

public interface IUserService
{
    // action auth
    public Task<OperationResult<UserLoginResponse>> SignIn(SignInModel request);
    public Task<OperationResult<bool>> SignUp(SignUpModel request);
    public Task<OperationResult<UserLoginResponse>> RefreshUserToken();
    public Task<OperationResult<UserDetailResponse>> GetCurrentUser();
    
    
    // action normal
    public Task<OperationResult<List<UserResponse>>> GetUsers();
    public Task<OperationResult<Pagination<UserResponse>>> GetUserPaginationAsync(int pageIndex = 0, int pageSize = 10);
    public Task<OperationResult<UserResponse>> GetUser(Guid id);
    public Task<OperationResult<UserResponse>> CreateUser(CreateUserRequestModel requestModel);
    public Task<OperationResult<bool>> UpdateUser(Guid id,UpdateUserRequestModel requestModel);
}