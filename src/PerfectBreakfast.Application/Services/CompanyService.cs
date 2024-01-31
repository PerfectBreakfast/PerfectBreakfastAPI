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

namespace PerfectBreakfast.Application.Services;

public class CompanyService : ICompanyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CompanyService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OperationResult<CompanyResponse>> CreateCompany(CompanyRequest companyRequest)
    {
        var result = new OperationResult<CompanyResponse>();
        try
        {
            var company = _mapper.Map<Company>(companyRequest);
            await _unitOfWork.CompanyRepository.AddAsync(company);
            await _unitOfWork.SaveChangeAsync();
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
            var companyEntity = await _unitOfWork.CompanyRepository.FindSingleAsync(c => c.Id == companyId, c => c.Delivery, c => c.Partner);
            if (companyEntity is null)
            {
                result.AddUnknownError("Id is not exsit");
                return result;
            }
            var managerUnit = _mapper.Map<PartnerResponseModel>(companyEntity.Partner);
            var deliveryUnit = _mapper.Map<DeliveryUnitResponseModel>(companyEntity.Delivery);
            var company = _mapper.Map<CompanyResponse>(companyEntity);
            company.DeliveryUnit = deliveryUnit;
            company.ManagementUnit = managerUnit;
            result.Payload = company;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<Pagination<CompanyResponsePaging>>> GetCompanyPaginationAsync(string? searchTerm, int pageIndex = 0, int pageSize = 10)
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

            var companyPages = await _unitOfWork.CompanyRepository.ToPagination(pageIndex, pageSize, searchPredicate, userInclude, managementUnitInclude, deliveryUnitInclude);
            var companyResponses = companyPages.Items.Select(c =>
                new CompanyResponsePaging
                {
                    Id = c.Id,
                    Address = c.Address,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    Name = c.Name,
                    IsDeleted = c.IsDeleted,
                    StartWorkHour = c.StartWorkHour,
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

    public async Task<OperationResult<CompanyResponse>> UpdateCompany(Guid Id, UpdateCompanyRequest companyRequest)
    {
        var result = new OperationResult<CompanyResponse>();
        try
        {
            var company = await _unitOfWork.CompanyRepository.GetByIdAsync(Id);
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