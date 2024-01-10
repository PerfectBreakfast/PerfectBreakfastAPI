using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.CompanyModels.Request;
using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;
using PerfectBreakfast.Application.Models.ManagementUnitModels.Resposne;
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
            var managerUnit = _mapper.Map<ManagementUnitResponseModel>(companyEntity.ManagementUnit);
            var deliveryUnit = _mapper.Map<DeliveryUnitResponseModel>(companyEntity.DeliveryUnit);
            var company = _mapper.Map<CompanyResponse>(companyEntity);
            company.DeliveryUnitResponseModel = deliveryUnit;
            company.ManagementUnitResponseModel = managerUnit;
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

    public async Task<OperationResult<CompanyResponse>> UpdateCompany(UpdateCompanyRequest updateCompanyRequest)
    {
        var result = new OperationResult<CompanyResponse>();
        try
        {
            var company = _mapper.Map<Company>(updateCompanyRequest);
            _unitOfWork.CompanyRepository.Update(company);
            await _unitOfWork.SaveChangeAsync();
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }
}