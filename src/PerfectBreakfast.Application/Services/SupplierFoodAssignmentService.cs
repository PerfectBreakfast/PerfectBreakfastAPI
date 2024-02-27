using System.Linq.Expressions;
using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
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
        private readonly IFoodService _foodService;
        private readonly IClaimsService _claimsService;

        public SupplierFoodAssignmentService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentTime currentTime, IFoodService foodService, IClaimsService claimsService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentTime = currentTime;
            _foodService = foodService;
            _claimsService = claimsService;
        }
        
        public async Task<OperationResult<List<SupplierFoodAssignmentResponse>>> CreateSupplierFoodAssignment(List<SupplierFoodAssignmentRequest> request)
        {
            var result = new OperationResult<List<SupplierFoodAssignmentResponse>>();
            var userId = _claimsService.GetCurrentUserId;
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                var supplierFoodAssignmentsResult = new List<SupplierFoodAssignmentResponse>();
                var partner = await _unitOfWork.PartnerRepository.GetByIdAsync((Guid)user.PartnerId);
                // Lấy supplier từ management Unit
                var suppliers = await _unitOfWork.SupplierRepository.GetSupplierByPartner((Guid)user.PartnerId);
                if (suppliers == null)
                {
                    result.AddError(ErrorCode.BadRequest, "Partner doesn't have supplier");
                    return result;
                }
                
                // Tổng số lượng food cần nấu cho tất cả cty thuộc management Unit
                var totalFoodCountOperationResult = await _foodService.GetFoodsForPartner();
                var totalFoodCount = totalFoodCountOperationResult.Payload;
                var totalFoodReceive = new Dictionary<string, int>();
                
                //Lay cac supplier trung voi supplier nhap vao
                var filteredSuppliers = new List<Supplier>();
                foreach (var requestId in request.Select(r => r.SupplierId))
                {
                    filteredSuppliers.AddRange(suppliers.Where(supplier => supplier.Id == requestId));
                }

                filteredSuppliers = filteredSuppliers.Distinct().ToList();

                foreach (var supplier in filteredSuppliers)
                {
                    //Tìm supplier khớp với supplier truyền vào và tìm % ăn chia supplier 
                    var foodAssignmentsResult = new List<SupplierFoodAssignment>();
                    var supplierCommissionRates =
                        await _unitOfWork.SupplierCommissionRateRepository.GetBySupplierId(supplier.Id);
                    // Lấy các phần tử từ danh sách request có SupplierId khớp với supplier.Id
                    var supplierFoodAssignments = request.Where(request => request.SupplierId == supplier.Id).ToList();
                    var supplierFoodAssignmentRequests = supplierFoodAssignments
                        .GroupBy(request => new { request.SupplierId, request.FoodId }) // Nhóm theo SupplierId và FoodId
                        .Select(group => new SupplierFoodAssignmentRequest
                        {
                            SupplierId = group.Key.SupplierId, // Lấy SupplierId từ Key của mỗi nhóm
                            FoodId = group.Key.FoodId, // Lấy FoodId từ Key của mỗi nhóm
                            AmountCooked = group.Sum(request => request.AmountCooked) // Tính tổng AmountCooked trong mỗi nhóm
                        }).ToList();
                    
                    // Xử lý mỗi trường hợp riêng lẻ
                    foreach (var supplierFoodAssignmentRequest in supplierFoodAssignmentRequests)
                    {
                        //Duyệt theo % ăn chia với mỗi loại thức ăn 
                        var supplierFoodAssignment = _mapper.Map<SupplierFoodAssignment>(supplierFoodAssignmentRequest);
                        var supplierCommissionRate = supplierCommissionRates.SingleOrDefault(s => s.FoodId == supplierFoodAssignment.FoodId);
                        var food = await _unitOfWork.FoodRepository.GetByIdAsync((Guid)supplierFoodAssignment.FoodId);
                        if (supplierCommissionRate == null)
                        {
                            result.AddError(ErrorCode.BadRequest, " Supplier don't have commission rate");
                            return result;
                        }
                        supplierFoodAssignment.ReceivedAmount = (food.Price * supplierCommissionRate.CommissionRate * supplierFoodAssignment.AmountCooked) / 100;
                        supplierFoodAssignment.SupplierCommissionRate = supplierCommissionRate;
                        supplierFoodAssignment.PartnerId = partner.Id;
                        supplierFoodAssignment.Status = SupplierFoodAssignmentStatus.Pending;

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
                    
                    var foodAssignmentResponses = new List<FoodAssignmentResponse>();
                    foreach (var item in foodAssignmentsResult)
                    {
                        var fo = await _unitOfWork.FoodRepository.GetByIdAsync((Guid)item.FoodId);
                        FoodAssignmentResponse foodAssignmentResponse = new FoodAssignmentResponse()
                        {
                            PartnerName = partner.Name,
                            AmountCooked = item.AmountCooked,
                            FoodName = fo.Name,
                            //DateCooked = item.DateCooked,
                            ReceivedAmount = item.ReceivedAmount,
                            Status = item.Status.ToString()
                        };
                        foodAssignmentResponses.Add(foodAssignmentResponse);
                    }

                    SupplierFoodAssignmentResponse supplierFoodAssignmentResponse = new SupplierFoodAssignmentResponse()
                    {
                        SupplierName = supplier.Name,
                        FoodAssignmentResponses = foodAssignmentResponses
                    };
                    supplierFoodAssignmentsResult.Add(supplierFoodAssignmentResponse);
                }
                
                // Sau khi hoàn thành vòng lặp foreach (var supplier in suppliers)
                foreach (var item in totalFoodCount)
                {
                    string foodName = item.Name; // Tên thức ăn
                    int count = item.Quantity; // Tổng số lượng từ DailyOrderResponseExcels

                    if (!totalFoodReceive.ContainsKey(foodName) || totalFoodReceive[foodName] != count)
                    {
                        // Có lỗi: Số lượng không khớp
                        result.AddError(ErrorCode.BadRequest, foodName + " doesn't enough amount");
                        return result;
                    }
                }

                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess)
                {
                    result.AddError(ErrorCode.ServerError, "Food or Supplier are not exist");
                    return result;
                }
                result.Payload = supplierFoodAssignmentsResult;

            }
            catch (NotFoundIdException ex)
            {
                result.AddError(ErrorCode.NotFound, "Food is not exist");
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<Pagination<SupplierFoodAssignmentForSupplier>>> GetSupplierFoodAssignmentBySupplier(int pageIndex = 0, int pageSize = 10)
        {
            var result = new OperationResult<Pagination<SupplierFoodAssignmentForSupplier>>();
            var userId = _claimsService.GetCurrentUserId;
            try
            {
                /*var supplierInclude = new IncludeInfo<User>
                {
                    NavigationProperty = x => x.Supplier,
                    ThenIncludes = new List<Expression<Func<object, object>>>
                    {
                        sp => ((Supplier)sp).SupplierCommissionRates
                    }
                };
                var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId, supplierInclude);
                var supplierCommissionRateIds = user.Supplier.SupplierCommissionRates.Select(s => s.Id).ToList();
                Expression<Func<SupplierFoodAssignment, bool>> predicate = s => supplierCommissionRateIds.Contains(s.SupplierCommissionRateId.Value);
                
                // Get Paging
                var foodInclude = new IncludeInfo<SupplierFoodAssignment>
                {
                    NavigationProperty = x => x.Food
                };
                var partnerInclude = new IncludeInfo<SupplierFoodAssignment>
                {
                    NavigationProperty = x => x.Partner
                };
                var supplierFoodAssignmentPages =
                    await _unitOfWork.SupplierFoodAssignmentRepository.ToPagination(pageIndex, pageSize, predicate,
                         foodInclude, partnerInclude);
                
                var supplierFoodAssignmentByDateCooked = supplierFoodAssignmentPages.Items.GroupBy(x => x.CreationDate.Date)
                    .ToDictionary(x => x.Key, g => g.ToList());
                
                // custom output
                var supplierFoodAssignmentResponse = supplierFoodAssignmentByDateCooked.Select(x =>
                {
                    var creationDated = x.Key; // BookingDate từ Dictionary
                    
                    var foodAssignmentGroupByPartner = x.Value.GroupBy(y => y.Partner.Name)
                        .ToDictionary(y => y.Key, g => g.ToList());

                    // Tạo danh sách FoodAssignmentResponse cho mỗi SupplierFoodAssignment trong x.Value
                    var foodAssignmentResponses = x.Value.Select(supplierFoodAssignment =>
                    {
                        // Tạo một FoodAssignmentResponse mới từ SupplierFoodAssignment
                        return new FoodAssignmentResponse
                        {
                            Id = supplierFoodAssignment.Id,
                            PartnerName = supplierFoodAssignment.Partner?.Name,
                            FoodName = supplierFoodAssignment.Food?.Name,
                            AmountCooked = supplierFoodAssignment.AmountCooked,
                            //DateCooked = supplierFoodAssignment.DateCooked,
                            ReceivedAmount = supplierFoodAssignment.ReceivedAmount,
                            Status = supplierFoodAssignment.Status.ToString()
                        };
                    var foodAssignmentGroupByPartnerResponse = foodAssignmentGroupByPartner.Select(x =>
                    {
                        var partnerName = x.Key;
                        
                        // Tạo danh sách FoodAssignmentResponse cho mỗi SupplierFoodAssignment trong x.Value
                        var foodAssignmentResponses = x.Value.Select(supplierFoodAssignment =>
                        {
                            // Tạo một FoodAssignmentResponse mới từ SupplierFoodAssignment
                            return new FoodAssignmentResponse
                            {
                                Id = supplierFoodAssignment.Id,
                                FoodName = supplierFoodAssignment.Food?.Name,
                                AmountCooked = supplierFoodAssignment.AmountCooked,
                                DateCooked = supplierFoodAssignment.DateCooked,
                                ReceivedAmount = supplierFoodAssignment.ReceivedAmount,
                                Status = supplierFoodAssignment.Status.ToString()
                            };
                        }).ToList();
                        return new FoodAssignmentGroupByPartner(partnerName, foodAssignmentResponses);
                    }).ToList();
                    

                    // Tạo một SupplierFoodAssignmentForSupplier mới với thông tin đầy đủ
                    return new SupplierFoodAssignmentForSupplier(DateOnly.FromDateTime(creationDated), foodAssignmentGroupByPartnerResponse);
                }).ToList();
                
                result.Payload = new Pagination<SupplierFoodAssignmentForSupplier>
                {
                    PageIndex = supplierFoodAssignmentPages.PageIndex,
                    PageSize = supplierFoodAssignmentPages.PageSize,
                    TotalItemsCount = supplierFoodAssignmentPages.TotalItemsCount,
                    Items = supplierFoodAssignmentResponse
                };*/
            }
            catch (NotFoundIdException ex)
            {
                result.AddError(ErrorCode.NotFound, "Food is not exist");
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }

            return result;
        }

        public async Task<OperationResult<Pagination<SupplierFoodAssignmentForPartner>>> GetSupplierFoodAssignmentByPartner(int pageIndex = 0, int pageSize = 10)
        {
            var result = new OperationResult<Pagination<SupplierFoodAssignmentForPartner>>();
            var userId = _claimsService.GetCurrentUserId;
            try
            {
                /*var partnerInclude = new IncludeInfo<User>
                {
                    NavigationProperty = x => x.Partner,
                    ThenIncludes = new List<Expression<Func<object, object>>>
                    {
                        sp => ((Partner)sp).SupplierFoodAssignments
                    }
                };
                var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId, partnerInclude);
                var partnerId = user.Partner.SupplierFoodAssignments.Select(s => s.PartnerId).ToList();
                Expression<Func<SupplierFoodAssignment, bool>> predicate = s => partnerId.Contains(s.PartnerId);
                
                // Get Paging
                var foodInclude = new IncludeInfo<SupplierFoodAssignment>
                {
                    NavigationProperty = x => x.Food
                };
                var supplierInclude = new IncludeInfo<SupplierFoodAssignment>
                {
                    NavigationProperty = x => x.SupplierCommissionRate,
                    ThenIncludes = new List<Expression<Func<object, object>>>
                    {
                        sp => ((SupplierCommissionRate)sp).Supplier
                    }
                };
                var supplierFoodAssignmentPages =
                    await _unitOfWork.SupplierFoodAssignmentRepository.ToPagination(pageIndex, pageSize, predicate,
                        foodInclude, supplierInclude);
                
                var supplierFoodAssignmentByCreationDated = supplierFoodAssignmentPages.Items.GroupBy(x => x.CreationDate.Date)
                    .ToDictionary(x => x.Key, g => g.ToList());
                
                // custom output
                var supplierFoodAssignmentResponse = supplierFoodAssignmentByCreationDated.Select(x =>
                {
                    var creationDated = x.Key; // Creation từ Dictionary
                    var foodAssignmentGroupBySupplier = x.Value.GroupBy(y => y.SupplierCommissionRate.Supplier.Name)
                        .ToDictionary(y => y.Key, g => g.ToList());

                    var foodAssignmentGroupBySupplierResponse = foodAssignmentGroupBySupplier.Select(x =>
                    {
                        var supplierName = x.Key;
                        // Tạo danh sách FoodAssignmentResponse cho mỗi SupplierFoodAssignment trong x.Value
                        var foodAssignmentResponses = x.Value.Select(supplierFoodAssignment =>
                        {
                            // Tạo một FoodAssignmentResponse mới từ SupplierFoodAssignment
                            return new FoodAssignmentResponse
                            {
                                Id = supplierFoodAssignment.Id,
                                FoodName = supplierFoodAssignment.Food?.Name,
                                AmountCooked = supplierFoodAssignment.AmountCooked,
                                DateCooked = supplierFoodAssignment.DateCooked,
                                ReceivedAmount = supplierFoodAssignment.ReceivedAmount,
                                Status = supplierFoodAssignment.Status.ToString()
                            };
                        }).ToList();
                        return new FoodAssignmentGroupBySupplier(supplierName, foodAssignmentResponses);
                    }).ToList();
                    
                    // Tạo một SupplierFoodAssignmentForSupplier mới với thông tin đầy đủ
                    return new SupplierFoodAssignmentForPartner(DateOnly.FromDateTime(creationDated), foodAssignmentGroupBySupplierResponse);
                }).ToList();
                
                result.Payload = new Pagination<SupplierFoodAssignmentForPartner>
                {
                    PageIndex = supplierFoodAssignmentPages.PageIndex,
                    PageSize = supplierFoodAssignmentPages.PageSize,
                    TotalItemsCount = supplierFoodAssignmentPages.TotalItemsCount,
                    Items = supplierFoodAssignmentResponse
                };*/
                
            }
            catch (NotFoundIdException ex)
            {
                result.AddError(ErrorCode.NotFound, "Food is not exist");
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }

            return result;
        }

        public async Task<OperationResult<FoodAssignmentResponse>> ConfirmFoodAssignment(Guid id)
        {
            var result = new OperationResult<FoodAssignmentResponse>();
            try
            {
                var supplierFoodAssignment = await _unitOfWork.SupplierFoodAssignmentRepository.GetByIdAsync(id);
                if (supplierFoodAssignment.Status != SupplierFoodAssignmentStatus.Pending)
                {
                    result.AddError(ErrorCode.BadRequest, "This order is already Confirm");
                    return result;
                }
                supplierFoodAssignment.Status = SupplierFoodAssignmentStatus.Confirmed;
                _unitOfWork.SupplierFoodAssignmentRepository.Update(supplierFoodAssignment);
                await _unitOfWork.SaveChangeAsync();
                var foodAssignment = _mapper.Map<FoodAssignmentResponse>(supplierFoodAssignment);
                result.Payload = foodAssignment;
            }
            catch (NotFoundIdException)
            {
                result.AddError(ErrorCode.NotFound, "Food assignment is not exist");
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<FoodAssignmentResponse>> CompleteFoodAssignment(Guid id)
        {
            var result = new OperationResult<FoodAssignmentResponse>();
            try
            {
                var supplierFoodAssignment = await _unitOfWork.SupplierFoodAssignmentRepository.GetByIdAsync(id);
                if (supplierFoodAssignment.Status != SupplierFoodAssignmentStatus.Confirmed)
                {
                    result.AddError(ErrorCode.BadRequest, "This order is already complete or can not complete");
                    return result;
                }
                supplierFoodAssignment.Status = SupplierFoodAssignmentStatus.Completed;
                _unitOfWork.SupplierFoodAssignmentRepository.Update(supplierFoodAssignment);
                await _unitOfWork.SaveChangeAsync();
                var foodAssignment = _mapper.Map<FoodAssignmentResponse>(supplierFoodAssignment);
                result.Payload = foodAssignment;
            }
            catch (NotFoundIdException)
            {
                result.AddError(ErrorCode.NotFound, "Food assignment is not exist");
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }
    }
}
