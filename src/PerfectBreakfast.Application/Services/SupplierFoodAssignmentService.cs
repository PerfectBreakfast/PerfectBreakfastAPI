﻿using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.DaliyOrder.Response;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Services
{
    public class SupplierFoodAssignmentService : ISupplierFoodAssignmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentTime _currentTime;
        private readonly IDailyOrderService _dailyOrderService;

        public SupplierFoodAssignmentService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentTime currentTime, IDailyOrderService dailyOrderService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentTime = currentTime;
            _dailyOrderService = dailyOrderService;
        }

        public async Task<OperationResult<List<SupplierFoodAssignmentResponse>>> CreateSupplierFoodAssignment(Guid managementUnitId, List<SupplierFoodAssignmentRequest> request)
        {
            var result = new OperationResult<List<SupplierFoodAssignmentResponse>>();
            try
            {
                var supplierfoodAssignmentsResult = new List<SupplierFoodAssignmentResponse>();

                // Lấy supplier từ management Unit
                var suppliers = await _unitOfWork.SupplierRepository.GetSupplierUnitByManagementUnit(managementUnitId);
                // lấy Dailyorder từ management Unit
                var dailyOrders = await _dailyOrderService.GetDailyOrderByManagementUnit(managementUnitId);

                if (dailyOrders == null || suppliers == null)
                {
                    result.AddUnknownError("ManagementUnit doesn't have order or supplier");
                    return result;
                }

                // Tổng số lượng food cần nấu cho tất cả cty thuộc management Unit
                var totalFoodCount = GetTotalFoodCounts(dailyOrders);
                var totalFoodReceive = new Dictionary<string, int>();

                //Lay cac supplier trung voi supplier nhap vao
                var filteredSuppliers = suppliers
                    .Where(supplier => request.Any(request => request.SupplierId == supplier.Id))
                    .ToList();

                foreach (var supplier in filteredSuppliers)
                {
                    //Tìm suppplier khớp với supplier truyền vào và tìm % ăn chia supplier 
                    var foodAssignmentsResult = new List<SupplierFoodAssignment>();
                    var supplierFoodAssignmentRequest = request.SingleOrDefault(request => request.SupplierId == supplier.Id);
                    var supplierCommissionRates = await _unitOfWork.SupplierCommissionRateRepository.GetBySupplier(supplier.Id);

                    //Duyệt theo % ăn chia với mỗi loại thức ăn 
                    foreach (var foodAssignmentRequest in supplierFoodAssignmentRequest.foodAssignmentRequests)
                    {
                        var supplierFoodAssignment = _mapper.Map<SupplierFoodAssignment>(foodAssignmentRequest);
                        var supplierCommissionRate = supplierCommissionRates.SingleOrDefault(s => s.FoodId == supplierFoodAssignment.FoodId);
                        var food = await _unitOfWork.FoodRepository.GetByIdAsync((Guid)supplierFoodAssignment.FoodId);
                        if (supplierCommissionRate == null)
                        {
                            result.AddUnknownError(" NCC chua co comission Rate");
                            return result;
                        }
                        supplierFoodAssignment.ReceivedAmount = food.Price * supplierCommissionRate.CommissionRate * supplierFoodAssignment.AmountCooked;
                        supplierFoodAssignment.SupplierCommissionRate = supplierCommissionRate;
                        supplierFoodAssignment.DateCooked = DateOnly.FromDateTime(_currentTime.GetCurrentTime());
                        supplierFoodAssignment.Status = SupplierFoodAssignmentStatus.Sent;

                        // Add food vao totalFoodReceive de so sanh
                        string foodName = food.Name;
                        if (totalFoodReceive.ContainsKey(foodName))
                        {
                            totalFoodReceive[foodName] += supplierFoodAssignment.AmountCooked;
                        }
                        else
                        {
                            totalFoodReceive.Add(foodName, supplierFoodAssignment.AmountCooked);
                        }

                        await _unitOfWork.SupplierFoodAssignmentRepository.AddAsync(supplierFoodAssignment);
                        foodAssignmentsResult.Add(supplierFoodAssignment);

                    }
                    var a = _mapper.Map<List<FoodAssignmentResponse>>(foodAssignmentsResult);
                    SupplierFoodAssignmentResponse supplierFoodAssignmentResponse = new SupplierFoodAssignmentResponse()
                    {
                        SupplierId = supplier.Id
                    };
                    supplierfoodAssignmentsResult.Add(supplierFoodAssignmentResponse);
                }

                // Sau khi hoàn thành vòng lặp foreach (var supplier in suppliers)
                foreach (var item in totalFoodCount)
                {
                    string foodName = item.Key; // Tên thức ăn
                    int count = item.Value; // Tổng số lượng từ DailyOrderResponseExcels

                    if (!totalFoodReceive.ContainsKey(foodName) || totalFoodReceive[foodName] != count)
                    {
                        // Có lỗi: Số lượng không khớp
                        result.AddUnknownError(foodName + " khong du so luong");
                        return result;
                    }
                }

                await _unitOfWork.SaveChangeAsync();
                result.Payload = supplierfoodAssignmentsResult;

            }
            catch (NotFoundIdException ex)
            {
                result.AddUnknownError("Food khong ton tai");
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);

            }
            return result;
        }

        public Task<OperationResult<List<SupplierFoodAssignmentResponse>>> GetSupplierFoodAssignment(Guid id)
        {
            throw new NotImplementedException();
        }

        static Dictionary<string, int> GetTotalFoodCounts(List<DailyOrderResponseExcel> dailyOrderResponseExcels)
        {
            var totalFoodCounts = new Dictionary<string, int>();

            foreach (var order in dailyOrderResponseExcels)
            {
                foreach (var foodItem in order.FoodCount)
                {
                    string foodName = foodItem.Key; // Assuming Key is FoodId as a string
                    if (totalFoodCounts.ContainsKey(foodName))
                    {
                        totalFoodCounts[foodName] += foodItem.Value;
                    }
                    else
                    {
                        totalFoodCounts.Add(foodName, foodItem.Value);
                    }
                }
            }

            return totalFoodCounts;
        }

    }
}