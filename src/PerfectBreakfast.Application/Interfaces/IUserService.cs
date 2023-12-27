using System.Linq.Expressions;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.UserModels.Request;
using PerfectBreakfast.Application.Models.UserModels.Response;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Interfaces;

public interface IUserService
{
    public Task<OperationResult<UserLoginResponse>> Login(LoginRequest query);
    public Task<OperationResult<List<UserResponse>>> GetAllUsers();
    public Task<OperationResult<Pagination<UserResponse>>> GetUserPaginationAsync(int pageIndex = 0, int pageSize = 10);
    public Task<OperationResult<UserResponse>> GetUserById(Guid id);
}