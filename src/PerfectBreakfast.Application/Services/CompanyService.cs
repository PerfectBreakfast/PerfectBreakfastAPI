using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.CompanyModels.Request;
using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;
using PerfectBreakfast.Application.Models.UserModels.Response;
using PerfectBreakfast.Domain.Entities;
using System.Linq.Expressions;
using PerfectBreakfast.Application.Models.PartnerModels.Response;
using CompanyResponse = PerfectBreakfast.Application.Models.CompanyModels.Response.CompanyResponse;

namespace PerfectBreakfast.Application.Services;

public class CompanyService : ICompanyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IClaimsService _claimsService;

    public CompanyService(IUnitOfWork unitOfWork, IMapper mapper, IClaimsService claimsService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _claimsService = claimsService;
    }

    public async Task<OperationResult<CompanyResponse>> CreateCompany(CompanyRequest companyRequest)
    {
        var result = new OperationResult<CompanyResponse>();
        try
        {
            // map to Company 
            var company = _mapper.Map<Company>(companyRequest);
            // add to return CompanyId 
            var entity = await _unitOfWork.CompanyRepository.AddAsync(company);
            foreach (var mealSubscription in companyRequest.Meals.Select(mealModel =>
                         new MealSubscription
                         {
                             CompanyId = entity.Id,
                             MealId = mealModel.MealId,
                             StartTime = mealModel.StartTime,
                             EndTime = mealModel.EndTime
                         }))
            {
                await _unitOfWork.MealSubscriptionRepository.AddAsync(mealSubscription);
            }

            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            if (!isSuccess)
            {
                result.AddError(ErrorCode.ServerError, "Partner or Delivery or Meal is not exist");
                return result;
            }

            result.Payload = _mapper.Map<CompanyResponse>(company);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<CompanyResponse>> Delete(Guid id)
    {
        var result = new OperationResult<CompanyResponse>();
        try
        {
            var com = await _unitOfWork.CompanyRepository.GetByIdAsync(id);
            _unitOfWork.CompanyRepository.Remove(com);
            await _unitOfWork.SaveChangeAsync();
        }
        catch (NotFoundIdException)
        {
            result.AddUnknownError("Id is not exsit");
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<List<UserResponse>>> GetUsersByCompanyId(Guid id)
    {
        var result = new OperationResult<List<UserResponse>>();
        try
        {
            var company = await _unitOfWork.CompanyRepository.GetByIdAsync(id, c => c.Workers);
            var users = company.Workers;
            result.Payload = _mapper.Map<List<UserResponse>>(users);
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

    public async Task<OperationResult<Pagination<CompanyResponsePaging>>> GetCompanyByPartner(string? searchTerm,
        int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<Pagination<CompanyResponsePaging>>();
        try
        {
            var userId = _claimsService.GetCurrentUserId;

            var partnerInclude = new IncludeInfo<User>
            {
                NavigationProperty = x => x.Partner,
                ThenIncludes = new List<Expression<Func<object, object>>>
                {
                    sp => ((Partner)sp).Companies
                }
            };

            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId, partnerInclude);
            var companyIds = user.Partner.Companies.Select(s => s.Id).ToList();
            Expression<Func<Company, bool>> predicate = s => companyIds.Contains(s.Id);

            var companyPages =
                await _unitOfWork.CompanyRepository.ToPagination(pageIndex, pageSize, predicate);

            var companyResponses = _mapper.Map<List<CompanyResponsePaging>>(companyPages.Items);

            result.Payload = new Pagination<CompanyResponsePaging>()
            {
                PageIndex = companyPages.PageIndex,
                PageSize = companyPages.PageSize,
                TotalItemsCount = companyPages.TotalItemsCount,
                Items = companyResponses
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

    public async Task<OperationResult<Pagination<CompanyResponsePaging>>> GetCompanyByDelivery(string? searchTerm,
        int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<Pagination<CompanyResponsePaging>>();
        try
        {
            var userId = _claimsService.GetCurrentUserId;

            var deliveryInclude = new IncludeInfo<User>
            {
                NavigationProperty = x => x.Delivery,
                ThenIncludes = new List<Expression<Func<object, object>>>
                {
                    sp => ((Delivery)sp).Companies
                }
            };

            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId, deliveryInclude);
            var companyIds = user.Delivery.Companies.Select(s => s.Id).ToList();
            Expression<Func<Company, bool>> predicate = s => companyIds.Contains(s.Id);

            var companyPages =
                await _unitOfWork.CompanyRepository.ToPagination(pageIndex, pageSize, predicate);

            var companyResponses = _mapper.Map<List<CompanyResponsePaging>>(companyPages.Items);

            result.Payload = new Pagination<CompanyResponsePaging>()
            {
                PageIndex = companyPages.PageIndex,
                PageSize = companyPages.PageSize,
                TotalItemsCount = companyPages.TotalItemsCount,
                Items = companyResponses
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

    public async Task<OperationResult<CompanyResponse>> DeleteCompany(Guid id)
    {
        var result = new OperationResult<CompanyResponse>();
        try
        {
            var com = await _unitOfWork.CompanyRepository.GetByIdAsync(id);
            _unitOfWork.CompanyRepository.SoftRemove(com);
            await _unitOfWork.SaveChangeAsync();
        }
        catch (NotFoundIdException)
        {
            result.AddUnknownError("Id is not exsit");
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<List<CompanyResponse>>> GetAllCompanies()
    {
        var result = new OperationResult<List<CompanyResponse>>();
        try
        {
            var com = await _unitOfWork.CompanyRepository.GetAllAsync();
            result.Payload = _mapper.Map<List<CompanyResponse>>(com);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<CompanyResponse>> GetCompany(Guid companyId)
    {
        var result = new OperationResult<CompanyResponse>();
        try
        {
            var companyEntity =
                await _unitOfWork.CompanyRepository.FindSingleAsync(c => c.Id == companyId, c => c.Delivery,
                    c => c.Partner);
            if (companyEntity is null)
            {
                result.AddUnknownError("Id is not exsit");
                return result;
            }

            var partner = _mapper.Map<PartnerResponseModel>(companyEntity.Partner);
            var delivery = _mapper.Map<DeliveryResponseModel>(companyEntity.Delivery);
            var company = _mapper.Map<CompanyResponse>(companyEntity);
            company.Delivery = delivery;
            company.Partner = partner;
            result.Payload = company;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<Pagination<CompanyResponsePaging>>> GetCompanyPaginationAsync(string? searchTerm,
        int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<Pagination<CompanyResponsePaging>>();
        try
        {
            // xác định các thuộc tính include và theninclude 
            var userInclude = new IncludeInfo<Company>
            {
                NavigationProperty = c => c.Workers
            };
            var managementUnitInclude = new IncludeInfo<Company>
            {
                NavigationProperty = c => c.Partner
            };
            var deliveryUnitInclude = new IncludeInfo<Company>
            {
                NavigationProperty = c => c.Delivery
            };

            // Tạo biểu thức tìm kiếm (predicate)
            Expression<Func<Company, bool>>? searchPredicate = string.IsNullOrEmpty(searchTerm)
                ? null
                : (x => x.Name.ToLower().Contains(searchTerm.ToLower()));

            var companyPages = await _unitOfWork.CompanyRepository.ToPagination(pageIndex, pageSize, searchPredicate,
                userInclude, managementUnitInclude, deliveryUnitInclude);
            var companyResponses = companyPages.Items.Select(c =>
                new CompanyResponsePaging
                {
                    Id = c.Id,
                    Address = c.Address,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    Name = c.Name,
                    IsDeleted = c.IsDeleted,
                    MemberCount = c.Workers.Count,
                    ManagementUnit = c.Partner.Name,
                    DeliveryUnit = c.Delivery.Name
                }).ToList();

            result.Payload = new Pagination<CompanyResponsePaging>
            {
                PageIndex = companyPages.PageIndex,
                PageSize = companyPages.PageSize,
                TotalItemsCount = companyPages.TotalItemsCount,
                Items = companyResponses
            };
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<CompanyResponse>> UpdateCompany(Guid id, UpdateCompanyRequest companyRequest)
    {
        var result = new OperationResult<CompanyResponse>();
        try
        {
            var company = await _unitOfWork.CompanyRepository.GetByIdAsync(id);
            _mapper.Map(companyRequest, company);
            _unitOfWork.CompanyRepository.Update(company);
            await _unitOfWork.SaveChangeAsync();
            result.Payload = _mapper.Map<CompanyResponse>(company);
        }
        catch (NotFoundIdException)
        {
            result.AddUnknownError("Id is not exsit");
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }
}