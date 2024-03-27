using System.Drawing;
using System.Linq.Expressions;
using Hangfire;
using MapsterMapper;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.MailModels;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;
using PerfectBreakfast.Application.Utils;
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
        private readonly IMailService _mailService;

        public SupplierFoodAssignmentService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentTime currentTime, IFoodService foodService, IClaimsService claimsService, IMailService mailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentTime = currentTime;
            _foodService = foodService;
            _claimsService = claimsService;
            _mailService = mailService;
        }
        
        public async Task<OperationResult<bool>> CreateSupplierFoodAssignment(SupplierFoodAssignmentsRequest request)
        {
            var result = new OperationResult<bool>();
            var foodAssignmentsResult = new List<SupplierFoodAssignment>();
            var userId = _claimsService.GetCurrentUserId;
            try
            {
                var supplierFoodAssignmentsResult = new List<SupplierFoodAssignmentResponse>();
                var partnerInclude = new IncludeInfo<User>
                {
                    NavigationProperty = x => x.Partner,
                    ThenIncludes = 
                    [
                        sp => ((Partner)sp).SupplyAssignments,
                            sp => ((SupplyAssignment)sp).Supplier,
                                sp => ((Supplier)sp).SupplierCommissionRates,
                                    sp => ((SupplierCommissionRate)sp).Food
                    ]
                };
                var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId, partnerInclude);
                
                // Lấy supplier từ partner
                var suppliers = user.Partner.SupplyAssignments.Select(s => s.Supplier).ToList();
                if (suppliers.Count == 0)
                {
                    result.AddError(ErrorCode.BadRequest, "Đối tác chưa có nhà cung cấp");
                    return result;
                }
                
                //Kiểm tra các nhà cung cấp nhập vào có thuộc đối tác
                var supplierIds = suppliers.Select(s => s.Id).ToList();
                var isSuppliersExist = request.SupplierFoodAssignmentRequest
                    .Any(s => supplierIds.Contains((Guid)s.SupplierId));
                if (!isSuppliersExist)
                {
                    result.AddError(ErrorCode.BadRequest, "Có nhà cung cấp không thuộc đối tác này");
                    return result;
                }
                
                //Kiểm tra xem đã phân chia món chưa
                var existAssignmentFood = await _unitOfWork.SupplierFoodAssignmentRepository.GetByDailyOrder((Guid)request.DailyOrderId);
                if (existAssignmentFood.Count > 0)
                {
                    result.AddError(ErrorCode.BadRequest, "Đơn hàng đã được phân chia");
                    return result;
                }
                
                // Tổng số lượng food cần phân chia
                var totalFoodCountOperationResult = await _foodService.GetFoodsForPartner((Guid)request.DailyOrderId);
                var totalFoodCounts = totalFoodCountOperationResult.Payload;
                var totalFoodReceive = new Dictionary<string, int>();
                
                //Lay cac supplier trung voi supplier nhap vao
                var filteredSuppliers = new List<Supplier>();
                foreach (var requestId in request.SupplierFoodAssignmentRequest.Select(r => r.SupplierId))
                {
                    filteredSuppliers.AddRange(suppliers.Where(supplier => supplier.Id == requestId));
                }
                filteredSuppliers = filteredSuppliers.Distinct().ToList();

                foreach (var supplier in filteredSuppliers)
                {
                    // Gom các Nhà cung cấp và Món ăn giống nhau lại
                    var supplierFoodAssignments = request.SupplierFoodAssignmentRequest.Where(request => request.SupplierId == supplier.Id).ToList();
                    var supplierFoodAssignmentRequests = supplierFoodAssignments
                        .GroupBy(request => new { request.SupplierId, request.FoodId }) // Nhóm theo SupplierId, FoodId
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
                        var supplierCommissionRate = supplier.SupplierCommissionRates.SingleOrDefault(s => s.FoodId == supplierFoodAssignment.FoodId);
                        if (supplierCommissionRate == null)
                        {
                            result.AddError(ErrorCode.BadRequest, "Nhà cung cấp chưa đăng kí món");
                            return result;
                        }
                        var food = supplierCommissionRate.Food;
                        supplierFoodAssignment.ReceivedAmount = (food.Price * supplierCommissionRate.CommissionRate * supplierFoodAssignment.AmountCooked) / 100;
                        supplierFoodAssignment.SupplierCommissionRateId = supplierCommissionRate.Id;
                        supplierFoodAssignment.PartnerId = user.Partner.Id;
                        supplierFoodAssignment.Status = SupplierFoodAssignmentStatus.Pending;
                        supplierFoodAssignment.DailyOrderId = request.DailyOrderId;
                        
                        // Add food vao totalFoodReceive de so sanh
                        var foodNameKey = food.FoodStatus == FoodStatus.Combo ? $"{food.Name} - khẩu phần combo" : $"{food.Name} - khẩu phần đơn lẻ";
                        
                        // Thêm thức ăn vào totalFoodReceive hoặc cập nhật số lượng nếu đã tồn tại
                        if (totalFoodReceive.ContainsKey(foodNameKey))
                        {
                            // Nếu thức ăn đã tồn tại, cập nhật số lượng
                            totalFoodReceive[foodNameKey] += supplierFoodAssignment.AmountCooked;
                        }
                        else
                        {
                            // Nếu thức ăn chưa tồn tại, thêm mới với số lượng
                            totalFoodReceive.Add(foodNameKey, supplierFoodAssignment.AmountCooked);
                        }
                        foodAssignmentsResult.Add(supplierFoodAssignment);
                    }
                }
                //Lưu xuống DB
                await _unitOfWork.SupplierFoodAssignmentRepository.AddRangeAsync(foodAssignmentsResult);
                
                // Check xem số lượng nhập vào có đủ hay không
                if (totalFoodCounts.TotalFoodResponses != null)
                {
                    foreach (var foodResponse in totalFoodCounts.TotalFoodResponses)
                    {
                        var foodName = foodResponse.Name;
                        var requiredQuantity = foodResponse.Quantity;
                        // Kiểm tra xem totalFoodReceive có chứa tên thức ăn này không và số lượng có đủ không
                        if (totalFoodReceive.ContainsKey(foodName))
                        {
                            if (totalFoodReceive[foodName] != requiredQuantity)
                            {
                                // Nếu số lượng thực tế không bằng với số lượng yêu cầu
                                result.AddError(ErrorCode.BadRequest, $"Số lượng của {foodName} không đủ yêu cầu: Yêu cầu {requiredQuantity}, thực tế {totalFoodReceive[foodName]}.");
                                return result;
                            }
                        }
                        else
                        {
                            // Nếu tên thức ăn không tồn tại trong totalFoodReceive, coi như số lượng thực tế là 0
                            result.AddError(ErrorCode.BadRequest, $"{foodName} món này không có trong đơn hàng");
                            return result;
                        }
                    }
                }
                
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (isSuccess)
                {
                    //Trả kết quả về API
                    result.Payload = true;
                }
                else
                {
                    result.AddError(ErrorCode.ServerError, "Food or Supplier are not exist");
                    return result;
                }
                //Gửi mail thông báo đến các nhà cung cấp
                // var users = filteredSuppliers.SelectMany(p => p.Users).ToList();
                // var filteredUsers  = new List<User>();
                // foreach (var u in users)
                // {
                //     if(await _unitOfWork.UserManager.IsInRoleAsync(u, ConstantRole.SUPPLIER_ADMIN))
                //     {
                //         filteredUsers.Add(u);
                //     }
                // }
                // var emailList = filteredUsers.Select(user => user.Email).ToList();
            
                // Tạo dữ liệu email, sử dụng token trong nội dung email
                // var mailData = new MailDataViewModel(
                //     to: emailList,
                //     subject: "Thông báo",
                //     body: $"Đơn hàng hôm nay đã được phân chia. Các nhà cung cấp có thể xác nhận món"
                // );
                //
                // // gửi mail ở luồng khác 
                // var jobId  = BackgroundJob.Enqueue<IManagementService>(x => x.SendMailToSupplierWhenPartnerAssignFood(mailData));
                //
            }
            catch (NotFoundIdException)
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
                var supplierInclude = new IncludeInfo<User>
                {
                    NavigationProperty = x => x.Supplier,
                    ThenIncludes = new List<Expression<Func<object, object>>>
                    {
                        sp => ((Supplier)sp).SupplierCommissionRates
                    }
                };
                var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId, supplierInclude);
                var supplierCommissionRateIds = user.Supplier.SupplierCommissionRates.Select(s => s.Id).ToList();
                Expression<Func<SupplierFoodAssignment, bool>> predicate = s => supplierCommissionRateIds.Contains(s.SupplierCommissionRateId.Value) && s.Status != SupplierFoodAssignmentStatus.Declined;
                
                // Get Paging
                var foodInclude = new IncludeInfo<SupplierFoodAssignment>
                {
                    NavigationProperty = x => x.Food
                };
                var partnerInclude = new IncludeInfo<SupplierFoodAssignment>
                {
                    NavigationProperty = x => x.Partner
                };
                var dailyOrderInclude = new IncludeInfo<SupplierFoodAssignment>
                {
                    NavigationProperty = x => x.DailyOrder,
                    ThenIncludes = new List<Expression<Func<object, object >>>
                    {
                        sp => ((DailyOrder)sp).MealSubscription
                    }
                };
                var supplierFoodAssignmentPages =
                    await _unitOfWork.SupplierFoodAssignmentRepository.ToPagination(pageIndex, pageSize, predicate,
                         foodInclude, partnerInclude, dailyOrderInclude);
                
                var supplierFoodAssignmentByDateCooked = supplierFoodAssignmentPages.Items.GroupBy(x => x.DailyOrder.BookingDate)
                    .ToDictionary(x => x.Key, g => g.ToList());
                
                // custom output
                var supplierFoodAssignmentResponse = supplierFoodAssignmentByDateCooked.Select(x =>
                {
                    var bookingDate = x.Key; // BookingDate từ Dictionary
                    
                    var foodAssignmentsGroupByPartner = x.Value.GroupBy(y => y.Partner.Name)
                        .ToDictionary(y => y.Key, g => g.ToList());
                    
                    var foodAssignmentGroupByPartnerResponse = foodAssignmentsGroupByPartner.Select(x =>
                    {
                        var partnerName = x.Key;
                        
                        var foodAssignmentGroupByPartner = x.Value.GroupBy(y => y.DailyOrder.MealSubscription.StartTime)
                            .ToDictionary(y => y.Key, g => g.ToList());

                        var deliveryTimeResponse = foodAssignmentGroupByPartner.Select(x =>
                        {
                            var deliveryTime = x.Key.Value.AddHours(-1);
                            // Tạo danh sách FoodAssignmentResponse cho mỗi SupplierFoodAssignment trong x.Value
                            var foodAssignmentResponses = x.Value.Select(supplierFoodAssignment =>
                            {
                                var foodName = supplierFoodAssignment.Food?.Name;
                                if (supplierFoodAssignment.Food?.FoodStatus == FoodStatus.Combo)
                                {
                                    foodName += " - khẩu phần combo";
                                }
                                else if (supplierFoodAssignment.Food?.FoodStatus == FoodStatus.Retail)
                                {
                                    foodName += " - khẩu phần đơn lẻ";
                                }
                                // Tạo một FoodAssignmentResponse mới từ SupplierFoodAssignment
                                return new FoodAssignmentResponse
                                {
                                    Id = supplierFoodAssignment.Id,
                                    FoodName = foodName,
                                    AmountCooked = supplierFoodAssignment.AmountCooked,
                                    ReceivedAmount = supplierFoodAssignment.ReceivedAmount,
                                    Status = supplierFoodAssignment.Status.ToString()
                                };
                            }).ToList();
                            return new SupplierDeliveryTime(deliveryTime, foodAssignmentResponses);
                        }).ToList();
                        
                        return new FoodAssignmentGroupByPartner(partnerName, deliveryTimeResponse);
                        
                    }).ToList();
                    
                    // Tạo một SupplierFoodAssignmentForSupplier mới với thông tin đầy đủ
                    return new SupplierFoodAssignmentForSupplier(bookingDate, foodAssignmentGroupByPartnerResponse);
                }).ToList();
                
                result.Payload = new Pagination<SupplierFoodAssignmentForSupplier>
                {
                    PageIndex = supplierFoodAssignmentPages.PageIndex,
                    PageSize = supplierFoodAssignmentPages.PageSize,
                    TotalItemsCount = supplierFoodAssignmentPages.TotalItemsCount,
                    Items = supplierFoodAssignmentResponse
                };
            }
            catch (NotFoundIdException)
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
                var partnerInclude = new IncludeInfo<User>
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
                var dailyOrderInclude = new IncludeInfo<SupplierFoodAssignment>
                {
                    NavigationProperty = x => x.DailyOrder,
                    ThenIncludes = new List<Expression<Func<object, object >>>
                    {
                        sp => ((DailyOrder)sp).MealSubscription,
                            sp => ((MealSubscription)sp).Meal
                    }
                };
                var supplierFoodAssignmentPages =
                    await _unitOfWork.SupplierFoodAssignmentRepository.ToPagination(pageIndex, pageSize, predicate,
                        foodInclude, supplierInclude, dailyOrderInclude);
                
                var supplierFoodAssignmentByCreationDated = supplierFoodAssignmentPages.Items.GroupBy(x => x.DailyOrder.BookingDate)
                    .ToDictionary(x => x.Key, g => g.ToList());
                
                // custom output
                var supplierFoodAssignmentResponse = supplierFoodAssignmentByCreationDated.Select(x =>
                {
                    var bookingDate = x.Key; // Booking date từ Dictionary
                    var foodAssignmentsGroupBySupplier = x.Value.GroupBy(y => y.SupplierCommissionRate.Supplier.Name)
                        .ToDictionary(y => y.Key, g => g.ToList());

                    var foodAssignmentGroupBySupplierResponse = foodAssignmentsGroupBySupplier.Select(x =>
                    {
                        var supplierName = x.Key;
                        
                        var foodAssignmentGroupBySupplier = x.Value.GroupBy(y => 
                                new { y.DailyOrder.MealSubscription.MealId, y.DailyOrder.MealSubscription.Meal.MealType } )
                            .ToDictionary(y => y.Key, g => g.ToList());

                        var partnerMealResponse = foodAssignmentGroupBySupplier.Select( x =>
                        {
                            var mealId = x.Key.MealId;
                            var meal =  x.Key.MealType;
                            // Tạo danh sách FoodAssignmentResponse cho mỗi SupplierFoodAssignment trong x.Value
                            var foodAssignmentResponses = x.Value.Select(supplierFoodAssignment =>
                            {
                                var foodName = supplierFoodAssignment.Food?.Name;
                                if (supplierFoodAssignment.Food?.FoodStatus == FoodStatus.Combo)
                                {
                                    foodName += " - khẩu phần combo";
                                }
                                else if (supplierFoodAssignment.Food?.FoodStatus == FoodStatus.Retail)
                                {
                                    foodName += " - khẩu phần đơn lẻ";
                                }
                                // Tạo một FoodAssignmentResponse mới từ SupplierFoodAssignment
                                return new FoodAssignmentResponse
                                {
                                    Id = supplierFoodAssignment.Id,
                                    FoodName = foodName,
                                    AmountCooked = supplierFoodAssignment.AmountCooked,
                                    ReceivedAmount = supplierFoodAssignment.ReceivedAmount,
                                    Status = supplierFoodAssignment.Status.ToString()
                                };
                            }).ToList();
                            return new PartnerFoodMealResponse(meal , foodAssignmentResponses);
                        }).ToList();
                        
                        return new FoodAssignmentGroupBySupplier(supplierName, partnerMealResponse);
                    }).ToList();
                    
                    // Tạo một SupplierFoodAssignmentForSupplier mới với thông tin đầy đủ
                    return new SupplierFoodAssignmentForPartner(bookingDate, foodAssignmentGroupBySupplierResponse);
                }).ToList();
                
                result.Payload = new Pagination<SupplierFoodAssignmentForPartner>
                {
                    PageIndex = supplierFoodAssignmentPages.PageIndex,
                    PageSize = supplierFoodAssignmentPages.PageSize,
                    TotalItemsCount = supplierFoodAssignmentPages.TotalItemsCount,
                    Items = supplierFoodAssignmentResponse
                };
                
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

        public async Task<OperationResult<FoodAssignmentResponse>> ChangeStatusFoodAssignment(Guid id, int status)
        {
            var result = new OperationResult<FoodAssignmentResponse>();
            try
            {
                var supplierFoodAssignment = await _unitOfWork.SupplierFoodAssignmentRepository.GetByIdAsync(id);
                if (supplierFoodAssignment.Status == SupplierFoodAssignmentStatus.Confirmed)
                {
                    result.AddError(ErrorCode.BadRequest, "Phần giao đã được xác nhận");
                    return result;
                }
                if (supplierFoodAssignment.Status == SupplierFoodAssignmentStatus.Complete)
                {
                    result.AddError(ErrorCode.BadRequest, "Phần giao đã được hoàn thành");
                    return result;
                }
                if (status != 1 && status != 0)
                {
                    result.AddError(ErrorCode.BadRequest, "Status must be 1 or 0");
                    return result;
                }
                supplierFoodAssignment.Status = status == 1 ? SupplierFoodAssignmentStatus.Confirmed : SupplierFoodAssignmentStatus.Declined;
                _unitOfWork.SupplierFoodAssignmentRepository.Update(supplierFoodAssignment);
                var supplierFoodAssignments =
                    await _unitOfWork.SupplierFoodAssignmentRepository
                        .GetByDailyOrder((Guid)supplierFoodAssignment.DailyOrderId);
                var allConfirmed = supplierFoodAssignments.All(a => a.Status == SupplierFoodAssignmentStatus.Confirmed);
                if (allConfirmed)
                {
                    var dailyOrder = await _unitOfWork.DailyOrderRepository.GetByIdAsync((Guid)supplierFoodAssignment.DailyOrderId);
                    if (dailyOrder is not { Status: DailyOrderStatus.Processing })
                    {
                        switch (dailyOrder.Status)
                        {
                            case DailyOrderStatus.Complete:
                                result.AddError(ErrorCode.BadRequest, "Đơn đã hoàn thành rồi nhé");
                                return result;
                            case DailyOrderStatus.Cooking:
                                result.AddError(ErrorCode.BadRequest, "Đơn đang trong quá trình nấu");
                                return result;
                            case DailyOrderStatus.Initial:
                                result.AddError(ErrorCode.BadRequest, "Đơn chưa sẵn sàng");
                                return result;
                            case DailyOrderStatus.Processing:
                                result.AddError(ErrorCode.BadRequest, "Đơn đang trong quá xử lý");
                                return result;
                            case DailyOrderStatus.Waiting:
                                result.AddError(ErrorCode.BadRequest, "Đơn đang trong quá trình nấu");
                                return result;
                            case DailyOrderStatus.Delivering:
                                result.AddError(ErrorCode.BadRequest, "Đơn đã đang trong quá trình giao");
                                return result;
                            default:
                                result.AddError(ErrorCode.BadRequest, "Không thể xác nhận đơn hàng lúc này");
                                return result;
                        }
                    }
                    dailyOrder.Status = DailyOrderStatus.Cooking;
                    _unitOfWork.DailyOrderRepository.Update(dailyOrder);
                }
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
                if (supplierFoodAssignment.Status == SupplierFoodAssignmentStatus.Pending)
                {
                    result.AddError(ErrorCode.BadRequest, "Phần giao chưa được xử lí để hoàn thành");
                    return result;
                }
                if (supplierFoodAssignment.Status == SupplierFoodAssignmentStatus.Complete)
                {
                    result.AddError(ErrorCode.BadRequest, "Phần giao đã được hoàn thành");
                    return result;
                }
                supplierFoodAssignment.Status = SupplierFoodAssignmentStatus.Complete;
                _unitOfWork.SupplierFoodAssignmentRepository.Update(supplierFoodAssignment);
                var supplierFoodAssignments =
                    await _unitOfWork.SupplierFoodAssignmentRepository
                        .GetByDailyOrder((Guid)supplierFoodAssignment.DailyOrderId);
                var allConfirmed = supplierFoodAssignments.All(a => a.Status == SupplierFoodAssignmentStatus.Complete);
                if (allConfirmed)
                {
                    var dailyOrder = await _unitOfWork.DailyOrderRepository.GetByIdAsync((Guid)supplierFoodAssignment.DailyOrderId);
                    if (dailyOrder is not { Status: DailyOrderStatus.Cooking })
                    {
                        switch (dailyOrder.Status)
                        {
                            case DailyOrderStatus.Complete:
                                result.AddError(ErrorCode.BadRequest, "Đơn đã hoàn thành rồi nhé");
                                return result;
                            case DailyOrderStatus.Initial:
                                result.AddError(ErrorCode.BadRequest, "Đơn chưa sẵn sàng");
                                return result;
                            case DailyOrderStatus.Processing:
                                result.AddError(ErrorCode.BadRequest, "Đơn đang trong quá xử lý");
                                return result;
                            case DailyOrderStatus.Waiting:
                                result.AddError(ErrorCode.BadRequest, "Đơn đang trong quá trình nấu");
                                return result;
                            case DailyOrderStatus.Delivering:
                                result.AddError(ErrorCode.BadRequest, "Đơn đã đang trong quá trình giao");
                                return result;
                            default:
                                result.AddError(ErrorCode.BadRequest, "Không thể xác nhận đơn hàng lúc này");
                                return result;
                        }
                    }
                    dailyOrder.Status = DailyOrderStatus.Waiting;
                    _unitOfWork.DailyOrderRepository.Update(dailyOrder);
                }
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

        public async Task<OperationResult<SupplierFoodAssignmentResponse>> UpdateSupplierFoodAssignment(UpdateSupplierFoodAssignment updateSupplierFoodAssignment)
        {
            var result = new OperationResult<SupplierFoodAssignmentResponse>();
            try
            {
                var supplierFoodAssignment = await _unitOfWork.SupplierFoodAssignmentRepository.GetByIdAsync((Guid)updateSupplierFoodAssignment.SupplierFoodAssignmentId);
                if (supplierFoodAssignment.Status != SupplierFoodAssignmentStatus.Declined)
                {
                    result.AddError(ErrorCode.BadRequest, "Đơn hàng này phải bị từ chối");
                    return result;
                }
                
                //Đổi nhà cung cấp khác
                var supplierCommissionRates =
                    await _unitOfWork.SupplierCommissionRateRepository
                        .GetBySupplierId((Guid)updateSupplierFoodAssignment.SupplierId);
                var supplierCommissionRate = supplierCommissionRates.SingleOrDefault(s => s.FoodId == supplierFoodAssignment.FoodId);
                if (supplierCommissionRate == null)
                {
                    result.AddError(ErrorCode.BadRequest, "Nhà cung cấp này chưa đăng kí món");
                    return result;
                }
                supplierFoodAssignment.SupplierCommissionRateId = supplierCommissionRate.Id;
                supplierFoodAssignment.Status = SupplierFoodAssignmentStatus.Pending;
                _unitOfWork.SupplierFoodAssignmentRepository.Update(supplierFoodAssignment);
                await _unitOfWork.SaveChangeAsync();
                var foodAssignment = _mapper.Map<SupplierFoodAssignmentResponse>(supplierFoodAssignment);
                
                //Gửi mail thông báo đến các nhà cung cấp
                var supplier =
                    await _unitOfWork.SupplierRepository.GetSupplierById((Guid)updateSupplierFoodAssignment.SupplierId, s=> s.Users);
                if (supplier == null)
                {
                    result.AddError(ErrorCode.BadRequest, "Nhà cung cấp không có admin");
                    return result;
                }
                var filteredUsers  = new List<User>();
                foreach (var user in supplier.Users)
                {
                    if(await _unitOfWork.UserManager.IsInRoleAsync(user, ConstantRole.SUPPLIER_ADMIN))
                    {
                        filteredUsers.Add(user);
                    }
                }
                var emailList = filteredUsers.Select(user => user.Email).ToList();

                // Tạo dữ liệu email, sử dụng token trong nội dung email
                var mailData = new MailDataViewModel(
                    to: emailList,
                    subject: "Thông báo",
                    body: $"Đơn hàng hôm nay đã được phân chia. Các nhà cung cấp có thể xác nhận món"
                );
                var ct = new CancellationToken();

                // Gửi email và xử lý kết quả
                var sendResult = await _mailService.SendAsync(mailData, ct);
                if (sendResult == false)
                {
                    Console.WriteLine("Gửi mail thất bại");
                }
                
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
        
        public async Task<OperationResult<List<SupplierFoodAssignmentForSupplier>>> GetSupplierFoodAssignmentsByBookingDate(DateOnly bookingDate)
        {
            var result = new OperationResult<List<SupplierFoodAssignmentForSupplier>>();
            var userId =  _claimsService.GetCurrentUserId;
            try
            {
                var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId);
                var supplierFoodAssignments =
                    await _unitOfWork.SupplierFoodAssignmentRepository.GetByBookingDateForSupplier();

                supplierFoodAssignments = supplierFoodAssignments.Where(s => s.DailyOrder.BookingDate == bookingDate && s.SupplierCommissionRate.SupplierId == user.SupplierId).ToList();
                if (supplierFoodAssignments.Count == 0)
                {
                    result.AddError(ErrorCode.BadRequest, "Ngày này không có món được phân chia");
                }
                var supplierFoodAssignmentByBookingDate = supplierFoodAssignments.GroupBy(x => x.DailyOrder.BookingDate)
                    .ToDictionary(x => x.Key, g => g.ToList());
                
                // custom output
                var supplierFoodAssignmentResponse = supplierFoodAssignmentByBookingDate.Select(x =>
                {
                    var bookingDate = x.Key; // BookingDate từ Dictionary
                    
                    var foodAssignmentsGroupByPartner = x.Value.GroupBy(y => y.Partner.Name)
                        .ToDictionary(y => y.Key, g => g.ToList());
                    
                    var foodAssignmentGroupByPartnerResponse = foodAssignmentsGroupByPartner.Select(x =>
                    {
                        var partnerName = x.Key;
                        
                        var foodAssignmentGroupByPartner = x.Value.GroupBy(y => y.DailyOrder.MealSubscription.StartTime)
                            .ToDictionary(y => y.Key, g => g.ToList());

                        var deliveryTimeResponse = foodAssignmentGroupByPartner.Select(x =>
                        {
                            var deliveryTime = x.Key.Value.AddHours(-1);
                            // Tạo danh sách FoodAssignmentResponse cho mỗi SupplierFoodAssignment trong x.Value
                            var foodAssignmentResponses = x.Value.Select(supplierFoodAssignment =>
                            {
                                var foodName = supplierFoodAssignment.Food?.Name;
                                if (supplierFoodAssignment.Food?.FoodStatus == FoodStatus.Combo)
                                {
                                    foodName += " - khẩu phần combo";
                                }
                                else if (supplierFoodAssignment.Food?.FoodStatus == FoodStatus.Retail)
                                {
                                    foodName += " - khẩu phần đơn lẻ";
                                }
                                // Tạo một FoodAssignmentResponse mới từ SupplierFoodAssignment
                                return new FoodAssignmentResponse
                                {
                                    Id = supplierFoodAssignment.Id,
                                    FoodName = foodName,
                                    AmountCooked = supplierFoodAssignment.AmountCooked,
                                    ReceivedAmount = supplierFoodAssignment.ReceivedAmount,
                                    Status = supplierFoodAssignment.Status.ToString()
                                };
                            }).ToList();
                            return new SupplierDeliveryTime(deliveryTime, foodAssignmentResponses);
                        }).ToList();
                        
                        return new FoodAssignmentGroupByPartner(partnerName, deliveryTimeResponse);
                        
                    }).ToList();
                    
                    // Tạo một SupplierFoodAssignmentForSupplier mới với thông tin đầy đủ
                    return new SupplierFoodAssignmentForSupplier(bookingDate, foodAssignmentGroupByPartnerResponse);
                }).ToList();
                
                
                result.Payload = supplierFoodAssignmentResponse;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<List<SupplierFoodAssignmentForSuperAdmin>>> GetSupplierFoodAssignmentsForSuperAdmin(DateOnly fromDate, DateOnly toDate)
        {
            var result = new OperationResult<List<SupplierFoodAssignmentForSuperAdmin>>();
            try
            {
                var supplierFoodAssignments =
                    await _unitOfWork.SupplierFoodAssignmentRepository.GetByForSuperAdmin();

                supplierFoodAssignments = supplierFoodAssignments
                    .Where(s => fromDate <= s.DailyOrder.BookingDate && s.DailyOrder.BookingDate <= toDate)
                    .ToList();
                
                if (supplierFoodAssignments.Count == 0)
                {
                    result.AddError(ErrorCode.BadRequest, "Ngày này không có món được phân chia");
                    return result;
                }
                var supplierFoodAssignmentByDates = supplierFoodAssignments.GroupBy(x => new { x.DailyOrder.CreationDate, x.DailyOrder.BookingDate })
                    .ToDictionary(x => x.Key, g => g.ToList());

                // custom output
                var supplierFoodAssignmentResponse = supplierFoodAssignmentByDates.Select(x =>
                {
                    var creationDate = x.Key.CreationDate; // creationDate từ Dictionary
                    var bookingDate = x.Key.BookingDate;
                    var foodAssignmentsGroupByPartner = x.Value.GroupBy(y => y.Partner.Name)
                        .ToDictionary(y => y.Key, g => g.ToList());
                    
                    var foodAssignmentGroupByPartnerResponse = foodAssignmentsGroupByPartner.Select(x =>
                    {
                        var partnerName = x.Key;
                        
                        var foodAssignmentGroupByPartner = x.Value.GroupBy(y => y.DailyOrder.MealSubscription.StartTime)
                            .ToDictionary(y => y.Key, g => g.ToList());

                        var deliveryTimeResponse = foodAssignmentGroupByPartner.Select(x =>
                        {
                            var deliveryTime = x.Key.Value.AddHours(-1);
                            // Tạo danh sách FoodAssignmentResponse cho mỗi SupplierFoodAssignment trong x.Value
                            var foodAssignmentResponses = x.Value.Select(supplierFoodAssignment =>
                            {
                                var foodName = supplierFoodAssignment.Food?.Name;
                                if (supplierFoodAssignment.Food?.FoodStatus == FoodStatus.Combo)
                                {
                                    foodName += " - khẩu phần combo";
                                }
                                else if (supplierFoodAssignment.Food?.FoodStatus == FoodStatus.Retail)
                                {
                                    foodName += " - khẩu phần đơn lẻ";
                                }
                                // Tạo một FoodAssignmentResponse mới từ SupplierFoodAssignment
                                return new FoodAssignmentResponse
                                {
                                    Id = supplierFoodAssignment.Id,
                                    FoodName = foodName,
                                    AmountCooked = supplierFoodAssignment.AmountCooked,
                                    ReceivedAmount = supplierFoodAssignment.ReceivedAmount,
                                    CommissionRate = supplierFoodAssignment.SupplierCommissionRate?.CommissionRate,
                                    Status = supplierFoodAssignment.Status.ToString()
                                };
                            }).ToList();
                            return new SupplierDeliveryTime(deliveryTime, foodAssignmentResponses);
                        }).ToList();
                        
                        return new FoodAssignmentGroupByPartner(partnerName, deliveryTimeResponse);
                        
                    }).ToList();
                    
                    // Tạo một SupplierFoodAssignmentForSupplier mới với thông tin đầy đủ
                    return new SupplierFoodAssignmentForSuperAdmin(DateOnly.FromDateTime(creationDate), bookingDate, foodAssignmentGroupByPartnerResponse);
                }).ToList();
                
                
                result.Payload = supplierFoodAssignmentResponse;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return result;
        }
    }
}
