using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Application.Models.SupplierCommissionRate.Request;
using PerfectBreakfast.Application.Models.SupplierCommissionRate.Respone;
using PerfectBreakfast.Application.Models.SupplierModels.Response;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Services;

public class SupplierCommissionRateService : ISupplierCommissionRateService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SupplierCommissionRateService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OperationResult<List<SupplierCommissionRateRespone>>> GetSupplierCommissionRates()
    {
        var result = new OperationResult<List<SupplierCommissionRateRespone>>();
        try
        {
            var u = await _unitOfWork.SupplierCommissionRateRepository.GetAllAsync();
            result.Payload = _mapper.Map<List<SupplierCommissionRateRespone>>(u);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<SupplierCommissionRateRespone>> GetSupplierCommissionRateId(Guid id)
    {
        var result = new OperationResult<SupplierCommissionRateRespone>();
        try
        {
            var supplierCommissionRate = await _unitOfWork.SupplierCommissionRateRepository.FindSingleAsync(o => o.Id == id, o => o.Food, o=>o.Supplier);
            if (supplierCommissionRate is null)
            {
                result.AddUnknownError("Id is not exsit");
                return result;
            }
            var food = _mapper.Map<FoodResponse>(supplierCommissionRate.Food);
            var supplier = _mapper.Map<SupplierResponse>(supplierCommissionRate.Supplier);
            var o = _mapper.Map<SupplierCommissionRateRespone>(supplierCommissionRate);
            o.FoodResponses = food;
            o.SupplierResponse = supplier;
            result.Payload = o;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<SupplierCommissionRateRespone>> CreateSupplierCommissionRate(CreateSupplierCommissionRateRequest createSupplierCommissionRateRequest)
    {
        var result = new OperationResult<SupplierCommissionRateRespone>();
        try
        {
            var supplierCommissionRate = _mapper.Map<SupplierCommissionRate>(createSupplierCommissionRateRequest);
            await _unitOfWork.SupplierCommissionRateRepository.AddAsync(supplierCommissionRate);
            await _unitOfWork.SaveChangeAsync();
            result.Payload = _mapper.Map<SupplierCommissionRateRespone>(supplierCommissionRate);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<SupplierCommissionRateRespone>> DeleteCSupplierCommissionRate(Guid id)
    {
        var result = new OperationResult<SupplierCommissionRateRespone>();
        try
        {
            var supplierCommissionRate = await _unitOfWork.SupplierCommissionRateRepository.GetByIdAsync(id);
            _unitOfWork.SupplierCommissionRateRepository.SoftRemove(supplierCommissionRate);
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

    public async Task<OperationResult<SupplierCommissionRateRespone>> UpdateSupplierCommissionRate(Guid id, UpdateSupplierCommissionRateRequest supplierCommissionRateRequest)
    {
        var result = new OperationResult<SupplierCommissionRateRespone>();
        try
        {
            var supplierCommissionRate = await _unitOfWork.SupplierCommissionRateRepository.GetByIdAsync(id);
            _mapper.Map(supplierCommissionRateRequest, supplierCommissionRate);
            _unitOfWork.SupplierCommissionRateRepository.Update(supplierCommissionRate);
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

    public async Task<OperationResult<Pagination<SupplierCommissionRateRespone>>> GetSupplierCommissionRatePaginationAsync(int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<Pagination<SupplierCommissionRateRespone>>();
        try
        {
            var sup = await _unitOfWork.SupplierCommissionRateRepository.ToPagination(pageIndex, pageSize);
            result.Payload = _mapper.Map<Pagination<SupplierCommissionRateRespone>>(sup);
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }
        return result;
    }

    public async Task<OperationResult<SupplierCommissionRateRespone>> Delete(Guid id)
    {
        var result = new OperationResult<SupplierCommissionRateRespone>();
        try
        {
            var com = await _unitOfWork.SupplierCommissionRateRepository.GetByIdAsync(id);
            _unitOfWork.SupplierCommissionRateRepository.Remove(com);
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