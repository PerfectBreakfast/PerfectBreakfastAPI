using System.Linq.Expressions;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.PartnerModels.Response;
using PerfectBreakfast.Application.Models.SupplierModels.Request;
using PerfectBreakfast.Application.Models.SupplierModels.Response;
using PerfectBreakfast.Application.Utils;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Services;

public class SupplierService : ISupplierService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IClaimsService _claimsService;

    public SupplierService(IUnitOfWork unitOfWork,IMapper mapper, IClaimsService claimsService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _claimsService = claimsService;
    }
    
    public async Task<OperationResult<List<SupplierResponse>>> GetSuppliers()
    {
        var result = new OperationResult<List<SupplierResponse>>();
        try
        {
            var suppliers = await _unitOfWork.SupplierRepository.GetAllAsync();
            result.Payload = _mapper.Map<List<SupplierResponse>>(suppliers);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<SupplierDetailResponse>> GetSupplierId(Guid id)
    {
        var result = new OperationResult<SupplierDetailResponse>();
        try
        {
            var supp = await _unitOfWork.SupplierRepository.GetSupplierDetail(id);
            if (supp == null)
            {
                result.AddUnknownError("Id does not exist");
                return result;
            }

            var managementUnit = supp.SupplyAssignments.Select(o => o.Partner).ToList();
            
            var supplier = _mapper.Map<SupplierDetailResponse>(supp);
            
            supplier.ManagementUnitDtos = _mapper.Map<List<PartnerDTO>>(managementUnit);

            result.Payload = supplier;
        }
        catch (NotFoundIdException e)
        {
            result.AddError(ErrorCode.NotFound, e.Message);
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
        }

        return result;
    }

    public async Task<OperationResult<SupplierResponse>> CreateSupplier(CreateSupplierRequestModel requestModel)
    {
        var result = new OperationResult<SupplierResponse>();
        try
        {
            // map model to Entity
            var supplier = _mapper.Map<Supplier>(requestModel);
            // Add to DB
            var entity = await _unitOfWork.SupplierRepository.AddAsync(supplier);
            // save change 
            await _unitOfWork.SaveChangeAsync();
            // map model to response
            result.Payload = _mapper.Map<SupplierResponse>(entity);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<SupplierResponse>> UpdateSupplier(Guid supplierId,UpdateSupplierRequestModel requestModel)
    {
        var result = new OperationResult<SupplierResponse>();
        try
        {
            // find supplier by ID
            var supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(supplierId);
            // map from requestModel => supplier
            _mapper.Map(requestModel, supplier);
            // update
            _unitOfWork.SupplierRepository.Update(supplier);
            // saveChange
            await _unitOfWork.SaveChangeAsync();
            result.Payload = _mapper.Map<SupplierResponse>(supplier);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<Pagination<SupplierResponse>>> GetPaginationAsync( string? searchTerm,int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<Pagination<SupplierResponse>>();
        try
        {
            // xác định các thuộc tính include và theninclude 
            var userInclude = new IncludeInfo<Supplier>
            {
                NavigationProperty = c => c.Users
            };
            var managementUnitInclude = new IncludeInfo<Supplier>
            {
                NavigationProperty = x => x.SupplyAssignments,
                ThenIncludes = new List<Expression<Func<object, object>>>
                {
                    sp => ((SupplyAssignment)sp).Partner
                }
            };
            // Tạo biểu thức tìm kiếm (predicate)
            Expression<Func<Supplier, bool>>? searchPredicate = string.IsNullOrEmpty(searchTerm) 
                ? null 
                : (x => x.Name.ToLower().Contains(searchTerm.ToLower()) || x.Address.ToLower().Contains(searchTerm.ToLower()));
            
            var supplierPages = await _unitOfWork.SupplierRepository.ToPagination(pageIndex, pageSize,searchPredicate,userInclude,managementUnitInclude);
            
            /*var supplierResponses = supplierPages.Items.Select(sp => 
                new SupplierResponse(
                    sp.Id,
                    sp.Name,
                    sp.Address,
                    sp.Longitude,
                    sp.Latitude,
                    sp.Users.Where(u => CheckIfUserIsAdmin(u))
                        .Select(u => u.Name).ToList(),
                    sp.SupplyAssignments.Select(sa => sa.ManagementUnit.Name).ToList(),
                    sp.Users.Count)).
                ToList();*/
            
            var supplierResponses = new List<SupplierResponse>();

            foreach (var sp in supplierPages.Items)
            {
                var adminUserNames = new List<string>();

                foreach (var user in sp.Users)   // lấy ra danh sách user có role là Supplier Admin
                {
                    if (await CheckIfUserIsAdmin(user))
                    {
                        adminUserNames.Add(user.Name);
                    }
                }

                var supplierResponse = new SupplierResponse(
                    sp.Id,
                    sp.Name,
                    sp.Address,
                    sp.PhoneNumber,
                    sp.Longitude,
                    sp.Latitude,
                    adminUserNames, // Danh sách người dùng là admin
                    sp.SupplyAssignments.Select(sa => sa.Partner.Name).ToList(),
                    sp.Users.Count);

                supplierResponses.Add(supplierResponse);
            }
            
            result.Payload = new Pagination<SupplierResponse>
            {
                PageIndex = supplierPages.PageIndex,
                PageSize = supplierPages.PageSize,
                TotalItemsCount = supplierPages.TotalItemsCount,
                Items = supplierResponses
            };
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<SupplierResponse>> RemoveSupplier(Guid supplierId)
    {
        var result = new OperationResult<SupplierResponse>();
        try
        {
            // find supplier by ID
            var supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(supplierId);
            // Remove
            var entity = _unitOfWork.SupplierRepository.Remove(supplier);
            // saveChange
            await _unitOfWork.SaveChangeAsync();
            // map entity to SupplierResponse
            result.Payload = _mapper.Map<SupplierResponse>(entity);
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
        return roles.Contains(ConstantRole.SUPPLIER_ADMIN);
    }

    public async Task<OperationResult<Pagination<SupplierDTO>>> GetSupplierByPartner(string? searchTerm,int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<Pagination<SupplierDTO>>();
        try
        {
            var userId = _claimsService.GetCurrentUserId;
            var partnerInclude = new IncludeInfo<User>
            {
                NavigationProperty = x => x.Partner,
                ThenIncludes = new List<Expression<Func<object, object>>>
                {
                    sp => ((Partner)sp).SupplyAssignments
                }
            };
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId, partnerInclude);
            var supplierIds = user.Partner.SupplyAssignments.Select(s => s.SupplierId).ToList();
            Expression<Func<Supplier, bool>> predicate = s => supplierIds.Contains(s.Id);
            
            Expression<Func<Supplier, bool>>? searchPredicate = string.IsNullOrEmpty(searchTerm) 
                ? null 
                : (x => x.Name.ToLower().Contains(searchTerm.ToLower()) || x.Address.ToLower().Contains(searchTerm.ToLower()));

            var supplierPages =
                await _unitOfWork.SupplierRepository.ToPagination(pageIndex, pageSize, predicate);

            var supplierResponses = _mapper.Map<List<SupplierDTO>>(supplierPages.Items);
            
            result.Payload = new Pagination<SupplierDTO>()
            {
                PageIndex = supplierPages.PageIndex,
                PageSize = supplierPages.PageSize,
                TotalItemsCount = supplierPages.TotalItemsCount,
                Items = supplierResponses
            };
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
}