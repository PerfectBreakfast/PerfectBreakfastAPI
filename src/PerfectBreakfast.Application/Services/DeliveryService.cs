using System.Linq.Expressions;
using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Request;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;
using PerfectBreakfast.Application.Utils;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Services;

public class DeliveryService : IDeliveryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DeliveryService(IUnitOfWork unitOfWork,IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<OperationResult<List<DeliveryResponseModel>>> GetDeliveries()
    {
        var result = new OperationResult<List<DeliveryResponseModel>>();
        try
        {
            var deliveries = await _unitOfWork.DeliveryRepository.GetAllAsync();
            result.Payload = _mapper.Map<List<DeliveryResponseModel>>(deliveries);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<DeliveryResponseModel>> CreateDelivery(CreateDeliveryRequest requestModel)
    {
        var result = new OperationResult<DeliveryResponseModel>();
        try
        {
            // map model to Entity
            var deliveryUnit = _mapper.Map<Delivery>(requestModel);
            // Add to DB
            var entity = await _unitOfWork.DeliveryRepository.AddAsync(deliveryUnit);
            // save change 
            await _unitOfWork.SaveChangeAsync();
            // map model to response
            result.Payload = _mapper.Map<DeliveryResponseModel>(entity);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<DeliveryResponseModel>> UpdateDelivery(Guid deliveryId, UpdateDeliveryRequest requestModel)
    {
        var result = new OperationResult<DeliveryResponseModel>();
        try
        {
            // find supplier by ID
            var deliveryUnit = await _unitOfWork.DeliveryRepository.GetByIdAsync(deliveryId);
            // map from requestModel => supplier
            _mapper.Map(requestModel, deliveryUnit);
            // update
            _unitOfWork.DeliveryRepository.Update(deliveryUnit);
            // saveChange
            await _unitOfWork.SaveChangeAsync();
            result.Payload = _mapper.Map<DeliveryResponseModel>(deliveryUnit);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<DeliveryResponseModel>> RemoveDelivery(Guid deliveryId)
    {
        var result = new OperationResult<DeliveryResponseModel>();
        try
        {
            // find supplier by ID
            var deliveryUnit = await _unitOfWork.DeliveryRepository.GetByIdAsync(deliveryId);
            // Remove
            var entity = _unitOfWork.DeliveryRepository.Remove(deliveryUnit);
            // saveChange
            await _unitOfWork.SaveChangeAsync();
            // map entity to SupplierResponse
            result.Payload = _mapper.Map<DeliveryResponseModel>(entity);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<DeliveryResponseModel>> GetDeliveryId(Guid deliveryId)
    {
        var result = new OperationResult<DeliveryResponseModel>();
            try
            {
                var deliveryUnit = await _unitOfWork.DeliveryRepository.GetByIdAsync(deliveryId);
                result.Payload = _mapper.Map<DeliveryResponseModel>(deliveryUnit);
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

    public async Task<OperationResult<Pagination<DeliveryResponseModel>>> GetDeliveryUnitPaginationAsync(string? searchTerm,int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<Pagination<DeliveryResponseModel>>();
        try
        {
            // xác định các thuộc tính include và theninclude 
            var userInclude = new IncludeInfo<Delivery>
            {
                NavigationProperty = c => c.Users
            };
            var companyInclude = new IncludeInfo<Delivery>
            {
                NavigationProperty = c => c.Companies
            };
            // Tạo biểu thức tìm kiếm (predicate)
            Expression<Func<Delivery, bool>>? searchPredicate = string.IsNullOrEmpty(searchTerm) 
                ? null 
                : (x => x.Name.ToLower().Contains(searchTerm.ToLower()));
            var deliveryPages = await _unitOfWork.DeliveryRepository.ToPagination(pageIndex, pageSize,searchPredicate,userInclude,companyInclude);
            var deliveryResponses = new List<DeliveryResponseModel>();
            foreach (var du in deliveryPages.Items)
            {
                var adminUserNames = new List<string>();

                foreach (var user in du.Users)   // lấy ra danh sách user có role là Supplier Admin
                {
                    if (await CheckIfUserIsAdmin(user))
                    {
                        adminUserNames.Add(user.Name);
                    }
                }

                var deliveryUnitResponse = new DeliveryResponseModel(
                    du.Id,
                    du.Name,
                    du.Address,
                    du.PhoneNumber,
                    du.CommissionRate,
                    du.Longitude,
                    du.Latitude,
                    adminUserNames, // Danh sách người dùng là admin
                    du.Companies.Select(c => c.Name).ToList(),
                    du.Users.Count);

                deliveryResponses.Add(deliveryUnitResponse);
            }
            
            result.Payload = new Pagination<DeliveryResponseModel>
            {
                PageIndex = deliveryPages.PageIndex,
                PageSize = deliveryPages.PageSize,
                TotalItemsCount = deliveryPages.TotalItemsCount,
                Items = deliveryResponses
            };
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }
    
    private async Task<bool> CheckIfUserIsAdmin(User user)
    {
        var roles = await _unitOfWork.UserManager.GetRolesAsync(user);
        return roles.Contains(ConstantRole.DELIVERY_ADMIN);
    }
}