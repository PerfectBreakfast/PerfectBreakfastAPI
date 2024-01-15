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

    // public async Task<OperationResult<SupplierMoreFoodRespone>> GetSupplierMoreFood(Guid supplierId)
    // {
    //     var result = new OperationResult<SupplierMoreFoodRespone>();
    //     try
    //     {
    //         // Assuming there's a method to get food items by supplierId
    //         var foods = await _unitOfWork.SupplierCommissionRateRepository.GetByIdAsync(supplierId);
    //         if (foods == null)
    //         {
    //             result.AddValidationError("No food items found for the given supplier ID.");
    //             return result;
    //         }
    //
    //         var supplierCommissionRate = await _unitOfWork.SupplierCommissionRateRepository.GetByIdAsync(supplierId);
    //         if (supplierCommissionRate == null || supplierCommissionRate.FoodId == null)
    //         {
    //             result.AddValidationError("No food items found for the given supplier ID.");
    //             return result;
    //         }
    //         else
    //         {
    //             return result;
    //         }
    //
    //         
    //     }
    //     catch (Exception e)
    //     {
    //         result.AddUnknownError(e.Message);
    //     }
    //     return result;
    // }

    // public async Task<OperationResult<List<SupplierCommissionRateRespone>>> CreateSupplierCommissionRate(CreateSupplierMoreFood createSupplierCommissionRateRequest)
    // {
    //     var result = new OperationResult<List<SupplierCommissionRateRespone>>();
    //     try
    //     {
    //         var responses = new List<SupplierCommissionRateRespone>();
    //
    //         foreach (var commissionRateRequest in createSupplierCommissionRateRequest.FoodId)
    //         {
    //             
    //             if (commissionRateRequest.FoodId == commissionRateRequest.SupplierId)
    //             {
    //                 result.AddValidationError("Food ID and Supplier ID cannot be the same.");
    //                 continue;
    //             }
    //
    //             var supplierCommissionRate = _mapper.Map<SupplierCommissionRate>(commissionRateRequest);
    //             await _unitOfWork.SupplierCommissionRateRepository.AddAsync(supplierCommissionRate);
    //
    //             
    //             var response = _mapper.Map<SupplierCommissionRateRespone>(supplierCommissionRate);
    //             responses.Add(response);
    //         }
    //
    //         await _unitOfWork.SaveChangeAsync();
    //         result.Payload = responses;
    //     }
    //     catch (Exception e)
    //     {
    //         result.AddUnknownError(e.Message);
    //     }
    //     return result;
    // }
    


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