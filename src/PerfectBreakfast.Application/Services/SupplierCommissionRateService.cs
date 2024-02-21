using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Application.Models.SupplierCommissionRate.Request;
using PerfectBreakfast.Application.Models.SupplierCommissionRate.Respone;
using PerfectBreakfast.Application.Models.SupplierModels.Response;
using PerfectBreakfast.Application.Repositories;
using PerfectBreakfast.Domain.Entities;


namespace PerfectBreakfast.Application.Services;

public class SupplierCommissionRateService : ISupplierCommissionRateService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    //private readonly ISupplierCommissionRateRepository _supplierCommissionRateRepository;

    public SupplierCommissionRateService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        //_supplierCommissionRateRepository = supplierCommissionRateRepository;
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

    public async Task<OperationResult<List<SupplierCommissionRateRespone>>> CreateSupplierCommissionRate(CreateSupplierCommissionRateRequest request)
    {
        var result = new OperationResult<List<SupplierCommissionRateRespone>>();
        var responses = new List<SupplierCommissionRateRespone>();

        foreach (var foodId in request.FoodIds)
        {
            // Check if FoodId is the same as SupplierId
            if (foodId == request.SupplierId)
            {
                result.AddValidationError($"Food ID and Supplier ID cannot be the same. FoodId: {foodId}");
                continue;
            }

            // Check if the combination of SupplierId and FoodId already exists
            bool alreadyExists = await _unitOfWork.SupplierCommissionRateRepository
                .AnyAsync(scr => scr.FoodId == foodId && scr.SupplierId == request.SupplierId);
            if (alreadyExists)
            {
                result.AddValidationError($"A commission rate for Supplier ID {request.SupplierId} and Food ID {foodId} already exists.");
                continue;
            }

            // Create new SupplierCommissionRate record
            var newCommissionRate = new SupplierCommissionRate
            {
                FoodId = foodId,
                SupplierId = request.SupplierId,
                CommissionRate = request.CommissionRate
            };
            await _unitOfWork.SupplierCommissionRateRepository.AddAsync(newCommissionRate);

            // Map to response DTO and add to the response list
            var responseDto = _mapper.Map<SupplierCommissionRateRespone>(newCommissionRate);
            responses.Add(responseDto);
        }

        await _unitOfWork.SaveChangeAsync();
        result.Payload = responses;
        return result;
    }

    public async Task<OperationResult<List<FoodResponse>>> GetSupplierMoreFood(Guid supplierId)
    {
        var result = new OperationResult<List<FoodResponse>>();
        try
        {
            // Assuming there's a method to get food items by supplierId
            var supp = await _unitOfWork.SupplierCommissionRateRepository.GetBySupplierId(supplierId);

            if (supp == null)
            {
                result.AddValidationError("ID is null");

                return result;
            }
            result.Payload = _mapper.Map<List<FoodResponse>>(supp.Select(x => x.Food)); 
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
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

    

    public async Task<OperationResult<SupplierCommissionRateRespone>> Delete(Guid id)
    {
        var result = new OperationResult<SupplierCommissionRateRespone>();
        try
        {
            var com = await _unitOfWork.SupplierCommissionRateRepository.GetByIdAsync(id);
            _unitOfWork.SupplierCommissionRateRepository.SoftRemove(com);
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