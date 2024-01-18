using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.CompanyModels.Request;
using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;
using PerfectBreakfast.Application.Models.ManagementUnitModels.Resposne;
using PerfectBreakfast.Application.Models.UserModels.Response;
using PerfectBreakfast.Domain.Entities;

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
            result.AddError(ErrorCode.NotFound,e.Message);
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
            var companyEntity = await _unitOfWork.CompanyRepository.FindSingleAsync(c => c.Id == companyId, c => c.DeliveryUnit, c => c.ManagementUnit);
            if (companyEntity is null)
            {
                result.AddUnknownError("Id is not exsit");
                return result;
            }
            var managerUnit = _mapper.Map<ManagementUnitResponseModel>(companyEntity.ManagementUnit);
            var deliveryUnit = _mapper.Map<DeliveryUnitResponseModel>(companyEntity.DeliveryUnit);
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

    public async Task<OperationResult<Pagination<CompanyResponse>>> GetCompanyPaginationAsync(int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<Pagination<CompanyResponse>>();
        try
        {
            var com = await _unitOfWork.CompanyRepository.ToPagination(pageIndex, pageSize);
            result.Payload = _mapper.Map<Pagination<CompanyResponse>>(com);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<CompanyResponse>> UpdateCompany(Guid Id, CompanyRequest companyRequest)
    {
        var result = new OperationResult<CompanyResponse>();
        try
        {
            var company = await _unitOfWork.CompanyRepository.GetByIdAsync(Id);
            _mapper.Map(companyRequest, company);
            _unitOfWork.CompanyRepository.Update(company);
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
}