using System.Linq.Expressions;
using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Request;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;
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
    public async Task<OperationResult<List<DeliveryUnitResponseModel>>> GetDeliveries()
    {
        var result = new OperationResult<List<DeliveryUnitResponseModel>>();
        try
        {
            var deliveryUnits = await _unitOfWork.DeliveryRepository.GetAllAsync();
            result.Payload = _mapper.Map<List<DeliveryUnitResponseModel>>(deliveryUnits);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<DeliveryUnitResponseModel>> CreateDelivery(CreateDeliveryUnitRequest requestModel)
    {
        var result = new OperationResult<DeliveryUnitResponseModel>();
        try
        {
            // map model to Entity
            var deliveryUnit = _mapper.Map<Delivery>(requestModel);
            // Add to DB
            var entity = await _unitOfWork.DeliveryRepository.AddAsync(deliveryUnit);
            // save change 
            await _unitOfWork.SaveChangeAsync();
            // map model to response
            result.Payload = _mapper.Map<DeliveryUnitResponseModel>(entity);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<DeliveryUnitResponseModel>> UpdateDelivery(Guid deliveryId, UpdateDeliveryUnitRequest requestModel)
    {
        var result = new OperationResult<DeliveryUnitResponseModel>();
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
            result.Payload = _mapper.Map<DeliveryUnitResponseModel>(deliveryUnit);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<DeliveryUnitResponseModel>> RemoveDelivery(Guid deliveryId)
    {
        var result = new OperationResult<DeliveryUnitResponseModel>();
        try
        {
            // find supplier by ID
            var deliveryUnit = await _unitOfWork.DeliveryRepository.GetByIdAsync(deliveryId);
            // Remove
            var entity = _unitOfWork.DeliveryRepository.Remove(deliveryUnit);
            // saveChange
            await _unitOfWork.SaveChangeAsync();
            // map entity to SupplierResponse
            result.Payload = _mapper.Map<DeliveryUnitResponseModel>(entity);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<DeliveryUnitResponseModel>> GetDeliveryId(Guid deliveryId)
    {
        var result = new OperationResult<DeliveryUnitResponseModel>();
            try
            {
                var deliveryUnit = await _unitOfWork.DeliveryRepository.GetByIdAsync(deliveryId);
                result.Payload = _mapper.Map<DeliveryUnitResponseModel>(deliveryUnit);
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

    public async Task<OperationResult<Pagination<DeliveryUnitResponseModel>>> GetDeliveryUnitPaginationAsync(string? searchTerm,int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<Pagination<DeliveryUnitResponseModel>>();
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
            var deliveryUnitPages = await _unitOfWork.DeliveryRepository.ToPagination(pageIndex, pageSize,searchPredicate,userInclude,companyInclude);
            var deliveryUnitResponses = new List<DeliveryUnitResponseModel>();
            foreach (var du in deliveryUnitPages.Items)
            {
                var adminUserNames = new List<string>();

                foreach (var user in du.Users)   // lấy ra danh sách user có role là Supplier Admin
                {
                    if (await CheckIfUserIsAdmin(user))
                    {
                        adminUserNames.Add(user.Name);
                    }
                }

                var deliveryUnitResponse = new DeliveryUnitResponseModel(
                    du.Id,
                    du.Name,
                    du.Address,
                    du.Longitude,
                    du.Latitude,
                    adminUserNames, // Danh sách người dùng là admin
                    du.Companies.Select(c => c.Name).ToList(),
                    du.Users.Count);

                deliveryUnitResponses.Add(deliveryUnitResponse);
            }
            
            result.Payload = new Pagination<DeliveryUnitResponseModel>
            {
                PageIndex = deliveryUnitPages.PageIndex,
                PageSize = deliveryUnitPages.PageSize,
                TotalItemsCount = deliveryUnitPages.TotalItemsCount,
                Items = deliveryUnitResponses
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
        return roles.Contains("DELIVERY UNIT ADMIN");
    }
}