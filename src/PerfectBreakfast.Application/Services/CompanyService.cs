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
using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Models.MealModels.Response;
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
            if (companyRequest.Meals.Any(meal => meal.StartTime > meal.EndTime))
            {
                result.AddError(ErrorCode.BadRequest, "Giờ bắt đầu phải trước giờ kết thúc");
                return result;
            }
            // map to Company 
            var company = _mapper.Map<Company>(companyRequest);
            // add to return CompanyId 
            var entity = await _unitOfWork.CompanyRepository.AddAsync(company);
            var mealSubscriptions = companyRequest.Meals.Select(mealModel => new MealSubscription
            {
                CompanyId = entity.Id,
                MealId = mealModel.MealId,
                StartTime = mealModel.StartTime,
                EndTime = mealModel.EndTime
            }).ToList();
            await _unitOfWork.MealSubscriptionRepository.AddRangeAsync(mealSubscriptions);

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

    public async Task<OperationResult<List<UserResponse>>> GetUsersByCompanyId(Guid id)
    {
        var result = new OperationResult<List<UserResponse>>();
        try
        {
            var users = await _unitOfWork.UserRepository.FindAll(x => x.CompanyId == id).ToListAsync();
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
            Expression<Func<Company, bool>> predicate = string.IsNullOrEmpty(searchTerm)
                ? (s => companyIds.Contains(s.Id) && !s.IsDeleted) 
                : (s=> companyIds.Contains(s.Id) && s.Name.ToLower().Contains(searchTerm.ToLower()) && !s.IsDeleted);
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
            Expression<Func<Company, bool>> predicate = string.IsNullOrEmpty(searchTerm)
                ? (s => companyIds.Contains(s.Id) && !s.IsDeleted) 
                : (s=> companyIds.Contains(s.Id) && s.Name.ToLower().Contains(searchTerm.ToLower()) && !s.IsDeleted);
                                                             

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

    public async Task<OperationResult<List<CompanyResponse>>> SearchCompany(string searchTerm)
    {
        var result = new OperationResult<List<CompanyResponse>>();
        try
        {
            var companies = await _unitOfWork.CompanyRepository.SearchCompany(searchTerm);
            if (companies == null)
            {
                result.AddError(ErrorCode.BadRequest, "Không có công ty hợp lệ");
            }
            result.Payload = _mapper.Map<List<CompanyResponse>>(companies);
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
            result.AddError(ErrorCode.NotFound,"Id is not exits");
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
            var companies = await _unitOfWork.CompanyRepository
                .FindAll(c => !c.IsDeleted)
                .ToListAsync();
            result.Payload = _mapper.Map<List<CompanyResponse>>(companies);
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
            var companyEntity = await _unitOfWork.CompanyRepository.GetCompanyDetailMealById(companyId);
            if (companyEntity is null)
            {
                result.AddError(ErrorCode.NotFound,"Id is not exist");
                return result;
            }
            
            var mealsSubscription = companyEntity.MealSubscriptions.ToList();
            var company = _mapper.Map<CompanyResponse>(companyEntity);
            company.Partner = !companyEntity.Partner.IsDeleted ? _mapper.Map<PartnerResponseModel>(companyEntity.Partner) : null;
            company.Delivery = !companyEntity.Delivery.IsDeleted ? _mapper.Map<DeliveryResponseModel>(companyEntity.Delivery) : null;
            company.MemberCount = companyEntity.Workers.Count;
            company.Meals = mealsSubscription
                .Where(mealSubscription => !mealSubscription.IsDeleted)
                .Select(mealsSubscription =>
                new MealResponse(
                    Id: mealsSubscription.Meal.Id, 
                    MealType: mealsSubscription.Meal.MealType, 
                    StartTime: mealsSubscription.StartTime, 
                    EndTime: mealsSubscription.EndTime
                )
            ).ToList();
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
            var partnerInclude= new IncludeInfo<Company>
            {
                NavigationProperty = c => c.Partner
            };
            var deliveryInclude = new IncludeInfo<Company>
            {
                NavigationProperty = c => c.Delivery
            };

            // Tạo biểu thức tìm kiếm (predicate)
            Expression<Func<Company, bool>>? searchPredicate = string.IsNullOrEmpty(searchTerm)
                ? (x => !x.IsDeleted)
                : (x => x.Name.ToLower().Contains(searchTerm.ToLower()) );

            var companyPages = await _unitOfWork.CompanyRepository.ToPagination(pageIndex, pageSize, searchPredicate,
                userInclude, partnerInclude, deliveryInclude);
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
            if (companyRequest.Meals.Any(meal => meal.StartTime > meal.EndTime))
            {
                result.AddError(ErrorCode.BadRequest, "Giờ bắt đầu phải trước giờ kết thúc");
                return result;
            }
            var company = await _unitOfWork.CompanyRepository.GetByIdAsync(id, m => m.MealSubscriptions);
            //_mapper.Map(companyRequest, company);
            company.Name = companyRequest.Name ?? company.Name;
            company.Address = companyRequest.Address ?? company.Address;
            company.PhoneNumber = companyRequest.PhoneNumber ?? company.PhoneNumber;
            company.Email = companyRequest.Email ?? company.Email;
            if (companyRequest.DeliveryId is not null)
            {
                // xuw ly
                company.DeliveryId = companyRequest.DeliveryId;
            }
            if (companyRequest.PartnerId is not null)
            {
                //xu ly 
                company.PartnerId = companyRequest.PartnerId;
            }
            
            var existingMealSubscriptions = company.MealSubscriptions;
            var newMealModels = companyRequest.Meals;

            foreach (var mealSubscription in company.MealSubscriptions)
            {
                var existMealSubscription = newMealModels.FirstOrDefault(ms => ms.MealId == mealSubscription.MealId);
                if (existMealSubscription == null)
                {
                    mealSubscription.IsDeleted = true;
                }
            }
            
            foreach (var mealModel in newMealModels)
            {
                var existingMealSubscription = existingMealSubscriptions.FirstOrDefault(ms => ms.MealId == mealModel.MealId);
                if (existingMealSubscription != null)
                {
                    // Nếu MealId tồn tại, cập nhật thông tin
                    existingMealSubscription.StartTime = mealModel.StartTime;
                    existingMealSubscription.EndTime = mealModel.EndTime;
                    existingMealSubscription.IsDeleted = false;
                }
                else
                {
                    // Nếu MealId không tồn tại, tạo mới MealSubscription và thêm vào danh sách
                    var newMealSubscription = new MealSubscription()
                    {
                        CompanyId = company.Id,
                        MealId = mealModel.MealId,
                        StartTime = mealModel.StartTime,
                        EndTime = mealModel.EndTime
                    };
                    existingMealSubscriptions.Add(newMealSubscription);
                }
            }

            // Sau khi cập nhật, gán lại danh sách MealSubscriptions cho company
            company.MealSubscriptions = existingMealSubscriptions;
            _unitOfWork.CompanyRepository.Update(company);
            await _unitOfWork.SaveChangeAsync();
            result.Payload = _mapper.Map<CompanyResponse>(company);
        }
        catch (NotFoundIdException)
        {
            result.AddError(ErrorCode.NotFound, "Id is not exsit");
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }
}