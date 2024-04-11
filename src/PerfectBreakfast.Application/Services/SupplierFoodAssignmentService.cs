using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Services;

public class SupplierFoodAssignmentService : ISupplierFoodAssignmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentTime _currentTime;
    private readonly IFoodService _foodService;
    private readonly IClaimsService _claimsService;
    private readonly IMailService _mailService;

    //Constant
    private const int Pending = 0;
    private const int Declined = 1;
    private const int Confirm = 2;
    private const int Complete = 3;

    public SupplierFoodAssignmentService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentTime currentTime,
        IFoodService foodService, IClaimsService claimsService, IMailService mailService)
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
            var suppliers = user.Partner!.SupplyAssignments.Select(s => s.Supplier).ToList();
            if (suppliers.Count == 0)
            {
                result.AddError(ErrorCode.BadRequest, "Đối tác chưa có nhà cung cấp");
                return result;
            }

            //Kiểm tra các nhà cung cấp nhập vào có thuộc đối tác
            var supplierIds = suppliers.Select(s => s.Id).ToList();
            var isSuppliersExist = request.SupplierFoodAssignmentRequest
                .Any(s => supplierIds.Contains(s.SupplierId));
            if (!isSuppliersExist)
            {
                result.AddError(ErrorCode.BadRequest, "Có nhà cung cấp không thuộc đối tác này");
                return result;
            }

            //Kiểm tra xem đã phân chia món chưa
            var dailyOrder =
                await _unitOfWork.DailyOrderRepository
                    .GetByIdAsync(request.DailyOrderId, d => d.MealSubscription!);
            if (dailyOrder.Package != null)
            {
                result.AddError(ErrorCode.BadRequest, "Đơn hàng đã được phân chia");
                return result;
            }

            // Tổng số lượng food cần phân chia
            var totalFoodCountOperationResult = await _foodService.GetFoodsForPartner(request.DailyOrderId);
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
                var supplierFoodAssignments = request.SupplierFoodAssignmentRequest
                    .Where(request => request.SupplierId == supplier.Id).ToList();
                var supplierFoodAssignmentRequests = supplierFoodAssignments
                    .GroupBy(request => new { request.SupplierId, request.FoodId }) // Nhóm theo SupplierId, FoodId
                    .Select(group => new SupplierFoodAssignmentRequest
                    {
                        SupplierId = group.Key.SupplierId, // Lấy SupplierId từ Key của mỗi nhóm
                        FoodId = group.Key.FoodId, // Lấy FoodId từ Key của mỗi nhóm
                        AmountCooked =
                            group.Sum(request => request.AmountCooked) // Tính tổng AmountCooked trong mỗi nhóm
                    }).ToList();

                // Xử lý mỗi trường hợp riêng lẻ
                foreach (var supplierFoodAssignmentRequest in supplierFoodAssignmentRequests)
                {
                    //Duyệt theo % ăn chia với mỗi loại thức ăn 
                    var supplierFoodAssignment = _mapper.Map<SupplierFoodAssignment>(supplierFoodAssignmentRequest);
                    var supplierCommissionRate =
                        supplier.SupplierCommissionRates.SingleOrDefault(s =>
                            s.FoodId == supplierFoodAssignment.FoodId);
                    if (supplierCommissionRate == null)
                    {
                        result.AddError(ErrorCode.BadRequest, "Nhà cung cấp chưa đăng kí món");
                        return result;
                    }

                    var food = supplierCommissionRate.Food;
                    supplierFoodAssignment.ReceivedAmount = (food.Price * supplierCommissionRate.CommissionRate *
                                                             supplierFoodAssignment.AmountCooked) / 100;
                    supplierFoodAssignment.SupplierCommissionRateId = supplierCommissionRate.Id;
                    supplierFoodAssignment.Status = SupplierFoodAssignmentStatus.Pending;
                    var bookingDateTime = dailyOrder.BookingDate.ToDateTime(new TimeOnly(0, 0));
                    var startTimeAdjusted = dailyOrder.MealSubscription!.StartTime.AddHours(-1).ToTimeSpan();
                    var deliveryDeadline = bookingDateTime.Add(startTimeAdjusted);
                    supplierFoodAssignment.DeliveryDeadline = deliveryDeadline;

                    // Add food vao totalFoodReceive de so sanh
                    var foodNameKey = food.FoodStatus == FoodStatus.Combo
                        ? $"{food.Name} - khẩu phần combo"
                        : $"{food.Name} - khẩu phần đơn lẻ";

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
            var package = new Package()
            {
                TotalFoodAssignQuantity = foodAssignmentsResult.Count,
                MissingQuantity = foodAssignmentsResult.Count,
                SuccessfulQuantity = 0,
                PartnerId = user.Partner.Id,
                DailyOrderId = dailyOrder.Id,
                SupplierFoodAssignments = foodAssignmentsResult!
            };
            await _unitOfWork.PackageRepository.AddAsync(package);

            dailyOrder.PackageId = package.Id;
            dailyOrder.Status = DailyOrderStatus.Divided;
            _unitOfWork.DailyOrderRepository.Update(dailyOrder);

            // Check xem số lượng nhập vào có đủ hay không
            if (totalFoodCounts.TotalFoodResponses != null)
            {
                foreach (var foodResponse in totalFoodCounts.TotalFoodResponses)
                {
                    var foodName = foodResponse.Name;
                    var requiredQuantity = foodResponse.Quantity;
                    // Kiểm tra xem totalFoodReceive có chứa tên thức ăn này không và số lượng có đủ không
                    if (totalFoodReceive.TryGetValue(foodName, out var value))
                    {
                        if (value != requiredQuantity)
                        {
                            // Nếu số lượng thực tế không bằng với số lượng yêu cầu
                            result.AddError(ErrorCode.BadRequest,
                                $"Số lượng của {foodName} không đủ yêu cầu: Yêu cầu {requiredQuantity}, thực tế {value}.");
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
        }
        catch (NotFoundIdException)
        {
            result.AddError(ErrorCode.NotFound, "Id is not exist");
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<Pagination<SupplierFoodAssignmentForSupplier>>>
        GetSupplierFoodAssignmentBySupplier(int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<Pagination<SupplierFoodAssignmentForSupplier>>();
        var userId = _claimsService.GetCurrentUserId;
        try
        {
            var supplierInclude = new IncludeInfo<User>
            {
                NavigationProperty = x => x.Supplier,
                ThenIncludes = [sp => ((Supplier)sp).SupplierCommissionRates]
            };
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId, supplierInclude);
            var supplierCommissionRateIds = user.Supplier.SupplierCommissionRates.Select(s => s.Id).ToList();

            // Get Paging
            var supplierFoodAssignmentPages =
                await _unitOfWork.SupplierFoodAssignmentRepository
                    .ToPaginationBySupplier(supplierCommissionRateIds, pageIndex, pageSize);

            var supplierFoodAssignmentByDateCooked = supplierFoodAssignmentPages.Items
                .GroupBy(x => x.Package.DailyOrder.BookingDate)
                .ToDictionary(x => x.Key, g => g.ToList());

            // custom output
            var supplierFoodAssignmentResponse = supplierFoodAssignmentByDateCooked.Select(x =>
            {
                var bookingDate = x.Key; // BookingDate từ Dictionary

                var supplierDeliveryTime = x.Value.GroupBy(y => y.Package.DailyOrder.MealSubscription.Meal.MealType)
                    .ToDictionary(y => y.Key, g => g.ToList());

                var supplierDeliveryTimeResponse = supplierDeliveryTime.Select(x =>
                {
                    var mealTime = x.Key;

                    var foodAssignmentGroupByPartner = x.Value.GroupBy(y => y.Package.Partner.Name)
                        .ToDictionary(y => y.Key, g => g.ToList());

                    var deliveryTimeResponse = foodAssignmentGroupByPartner.Select(x =>
                    {
                        var partnerName = x.Key;
                        var packageIds = x.Value.Select(s => s.PackageId).ToList();
                        var foodAssignmentResponse = _mapper.Map<List<FoodAssignmentResponse>>(x.Value);
                        var packages = x.Value.Select(s => s.Package).ToList();
                        var packagesToCheck = packages.Select(package => package.SupplierFoodAssignments
                                .Where(sfa => sfa.SupplierCommissionRate.SupplierId == user.SupplierId)
                                .ToList())
                            .Select(filteredAssignments =>
                                new Package { SupplierFoodAssignments = filteredAssignments })
                            .ToList();
                        var hasDeclined = packagesToCheck.Any(package =>
                            package.SupplierFoodAssignments.Any(sfa =>
                                sfa.Status == SupplierFoodAssignmentStatus.Declined)
                        );
                        var hasPending = packagesToCheck.Any(package =>
                            package.SupplierFoodAssignments.Any(sfa =>
                                sfa.Status == SupplierFoodAssignmentStatus.Pending)
                        );
                        var hasConfirm = packagesToCheck.Any(package =>
                            package.SupplierFoodAssignments.Any(sfa =>
                                sfa.Status == SupplierFoodAssignmentStatus.Confirmed)
                        );
                        var statusConstant = hasDeclined ? Declined :
                            hasPending ? Pending :
                            hasConfirm ? Confirm :
                            Complete;
                        return new FoodAssignmentGroupByPartner(partnerName, packageIds, statusConstant,
                            foodAssignmentResponse);
                    }).ToList();

                    return new SupplierDeliveryTime(mealTime, deliveryTimeResponse);
                }).ToList();

                // Tạo một SupplierFoodAssignmentForSupplier mới với thông tin đầy đủ
                return new SupplierFoodAssignmentForSupplier(bookingDate, supplierDeliveryTimeResponse);
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

    public async Task<OperationResult<Pagination<SupplierFoodAssignmentForPartner>>> GetSupplierFoodAssignmentByPartner(
        int pageIndex = 0, int pageSize = 10)
    {
        var result = new OperationResult<Pagination<SupplierFoodAssignmentForPartner>>();
        var userId = _claimsService.GetCurrentUserId;
        try
        {
            var partnerInclude = new IncludeInfo<User>
            {
                NavigationProperty = x => x.Partner,
                ThenIncludes = [sp => ((Partner)sp).Packages]
            };
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId, partnerInclude);
            var packageId = user.Partner!.Packages.Select(s => s.Id).ToList();

            // Get Paging
            var supplierFoodAssignmentPages =
                await _unitOfWork.SupplierFoodAssignmentRepository.ToPaginationByPartner(packageId, pageIndex,
                    pageSize);

            var supplierFoodAssignmentByCreationDated = supplierFoodAssignmentPages.Items
                .GroupBy(x => x.Package.DailyOrder.BookingDate)
                .ToDictionary(x => x.Key, g => g.ToList());

            // custom output
            var supplierFoodAssignmentResponse = supplierFoodAssignmentByCreationDated.Select(x =>
            {
                var bookingDate = x.Key; // Booking date từ Dictionary
                var foodAssignmentsGroupBySupplier = x.Value
                    .GroupBy(y => y.Package.DailyOrder.MealSubscription.Meal.MealType)
                    .ToDictionary(y => y.Key, g => g.ToList());

                var partnerMealResponse = foodAssignmentsGroupBySupplier.Select(x =>
                {
                    var mealType = x.Key;

                    var foodAssignmentGroupBySupplier = x.Value
                        .GroupBy(y => (y.SupplierCommissionRate.Supplier.Id, y.SupplierCommissionRate.Supplier.Name))
                        .ToDictionary(y => y.Key, g => g.ToList());

                    var partnerMealResponse = foodAssignmentGroupBySupplier.Select(x =>
                    {
                        var supplierId = x.Key.Id;
                        var supplierName = x.Key.Name;
                        var packageIds = x.Value.Select(s => s.PackageId).ToList();
                        var packages = x.Value.Select(s => s.Package).ToList();
                        var packagesToCheck = packages.Select(package => package.SupplierFoodAssignments
                                .Where(sfa => sfa.SupplierCommissionRate.SupplierId == supplierId)
                                .ToList())
                            .Select(filteredAssignments =>
                                new Package { SupplierFoodAssignments = filteredAssignments })
                            .ToList();

                        var hasDeclined = packagesToCheck.Any(package =>
                            package.SupplierFoodAssignments.Any(sfa =>
                                sfa.Status == SupplierFoodAssignmentStatus.Declined)
                        );
                        var hasPending = packagesToCheck.Any(package =>
                            package.SupplierFoodAssignments.Any(sfa =>
                                sfa.Status == SupplierFoodAssignmentStatus.Pending)
                        );
                        var hasConfirm = packagesToCheck.Any(package =>
                            package.SupplierFoodAssignments.Any(sfa =>
                                sfa.Status == SupplierFoodAssignmentStatus.Confirmed)
                        );
                        var statusConstant = hasDeclined ? Declined :
                            hasPending ? Pending :
                            hasConfirm ? Confirm :
                            Complete;
                        var foodAssignmentResponse = _mapper.Map<List<FoodAssignmentResponse>>(x.Value);
                        return new FoodAssignmentGroupBySupplier(supplierId, supplierName, packageIds, statusConstant,
                            foodAssignmentResponse);
                    }).ToList();

                    return new PartnerFoodMealResponse(mealType, partnerMealResponse);
                }).ToList();

                // Tạo một SupplierFoodAssignmentForSupplier mới với thông tin đầy đủ
                return new SupplierFoodAssignmentForPartner(bookingDate, partnerMealResponse);
            }).ToList();

            result.Payload = new Pagination<SupplierFoodAssignmentForPartner>
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

    public async Task<OperationResult<FoodAssignmentResponse>> ChangeStatusFoodAssignment(Guid id, int status)
    {
        var result = new OperationResult<FoodAssignmentResponse>();
        try
        {
            var supplierFoodAssignment =
                await _unitOfWork.SupplierFoodAssignmentRepository.GetByIdAsync(id, s => s.Package);
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

            var dailyOrder =
                await _unitOfWork.DailyOrderRepository.GetByIdAsync((Guid)supplierFoodAssignment.Package.DailyOrderId);
            if (dailyOrder is not { Status: DailyOrderStatus.Divided })
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

            supplierFoodAssignment.Status =
                status == 1 ? SupplierFoodAssignmentStatus.Confirmed : SupplierFoodAssignmentStatus.Declined;
            _unitOfWork.SupplierFoodAssignmentRepository.Update(supplierFoodAssignment);
            var supplierFoodAssignments =
                await _unitOfWork.SupplierFoodAssignmentRepository
                    .GetByPackage((Guid)supplierFoodAssignment.PackageId);
            var allConfirmed = supplierFoodAssignments.All(a => a.Status == SupplierFoodAssignmentStatus.Confirmed);
            if (allConfirmed)
            {
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
            var supplierFoodAssignment =
                await _unitOfWork.SupplierFoodAssignmentRepository.GetByIdAsync(id, s => s.Package);
            var dailyOrder =
                await _unitOfWork.DailyOrderRepository.GetByIdAsync((Guid)supplierFoodAssignment.Package.DailyOrderId);
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

            if (dailyOrder.Status != DailyOrderStatus.Cooking)
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
                    case DailyOrderStatus.Divided:
                        result.AddError(ErrorCode.BadRequest, "Đơn đang trong quá trình chờ xác nhận");
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

            supplierFoodAssignment.Status = SupplierFoodAssignmentStatus.Complete;
            supplierFoodAssignment.Package.MissingQuantity--;
            supplierFoodAssignment.Package.SuccessfulQuantity++;
            _unitOfWork.SupplierFoodAssignmentRepository.Update(supplierFoodAssignment);
            if (supplierFoodAssignment.Package.SuccessfulQuantity ==
                supplierFoodAssignment.Package.TotalFoodAssignQuantity)
            {
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

    public async Task<OperationResult<SupplierFoodAssignmentResponse>> UpdateSupplierFoodAssignment(
        UpdateSupplierFoodAssignment updateSupplierFoodAssignment)
    {
        var result = new OperationResult<SupplierFoodAssignmentResponse>();
        try
        {
            var supplierFoodAssignment =
                await _unitOfWork.SupplierFoodAssignmentRepository.GetByIdAsync(updateSupplierFoodAssignment
                    .SupplierFoodAssignmentId);
            if (supplierFoodAssignment.Status != SupplierFoodAssignmentStatus.Declined)
            {
                result.AddError(ErrorCode.BadRequest, "Đơn hàng này phải bị từ chối");
                return result;
            }

            //Đổi nhà cung cấp khác
            var supplierCommissionRates =
                await _unitOfWork.SupplierCommissionRateRepository
                    .GetBySupplierId(updateSupplierFoodAssignment.SupplierId);
            var supplierCommissionRate =
                supplierCommissionRates.SingleOrDefault(s => s.FoodId == supplierFoodAssignment.FoodId);
            if (supplierCommissionRate == null)
            {
                result.AddError(ErrorCode.BadRequest, "Nhà cung cấp này chưa đăng kí món");
                return result;
            }

            supplierFoodAssignment.SupplierCommissionRateId = supplierCommissionRate.Id;
            supplierFoodAssignment.Status = SupplierFoodAssignmentStatus.Pending;
            _unitOfWork.SupplierFoodAssignmentRepository.Update(supplierFoodAssignment);
            await _unitOfWork.SaveChangeAsync();

            //Gửi mail thông báo đến các nhà cung cấp
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

    public async Task<OperationResult<List<FoodAssignmentDownloadForSupplier>>>
        GetSupplierFoodAssignmentsForSupplierDownload(List<Guid> packageIds)
    {
        var result = new OperationResult<List<FoodAssignmentDownloadForSupplier>>();
        var userId = _claimsService.GetCurrentUserId;
        try
        {
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId);
            var supplierAssignment =
                await _unitOfWork.SupplierFoodAssignmentRepository.GetForSupplierByPackage(packageIds,
                    user.SupplierId!.Value);
            var foodAssignmentForCompany = supplierAssignment
                .GroupBy(x => (x.Package.DailyOrder.MealSubscription.Company.Name, x.Package.Partner.Name))
                .ToDictionary(x => x.Key, g => g.ToList());
            ;
            var foodAssignmentForCompanyResponse = foodAssignmentForCompany.Select(x =>
            {
                var companyName = x.Key.Item1;
                var partnerName = x.Key.Item2;
                var supplierFoodAssignments = _mapper.Map<List<FoodAssignmentResponse>>(x.Value);
                return new FoodAssignmentDownloadForSupplier(partnerName, companyName, supplierFoodAssignments);
            }).ToList();
            result.Payload = foodAssignmentForCompanyResponse;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<List<SupplierFoodAssignmentForSuperAdmin>>>
        GetSupplierFoodAssignmentsForSuperAdmin(DateOnly fromDate, DateOnly toDate)
    {
        var result = new OperationResult<List<SupplierFoodAssignmentForSuperAdmin>>();
        try
        {
            // var supplierFoodAssignments =
            //     await _unitOfWork.SupplierFoodAssignmentRepository.GetByForSuperAdmin();
            //
            // supplierFoodAssignments = supplierFoodAssignments
            //     .Where(s => fromDate <= s.DailyOrder.BookingDate && s.DailyOrder.BookingDate <= toDate)
            //     .ToList();
            //
            // if (supplierFoodAssignments.Count == 0)
            // {
            //     result.AddError(ErrorCode.BadRequest, "Ngày này không có món được phân chia");
            //     return result;
            // }
            // var supplierFoodAssignmentByDates = supplierFoodAssignments.GroupBy(x => new { x.DailyOrder.CreationDate, x.DailyOrder.BookingDate })
            //     .ToDictionary(x => x.Key, g => g.ToList());
            //
            // // custom output
            // var supplierFoodAssignmentResponse = supplierFoodAssignmentByDates.Select(x =>
            // {
            //     var creationDate = x.Key.CreationDate; // creationDate từ Dictionary
            //     var bookingDate = x.Key.BookingDate;
            //     var foodAssignmentsGroupByPartner = x.Value.GroupBy(y => y.Partner.Name)
            //         .ToDictionary(y => y.Key, g => g.ToList());
            //     
            //     var foodAssignmentGroupByPartnerResponse = foodAssignmentsGroupByPartner.Select(x =>
            //     {
            //         var partnerName = x.Key;
            //         
            //         var foodAssignmentGroupByPartner = x.Value.GroupBy(y => y.DailyOrder.MealSubscription.StartTime)
            //             .ToDictionary(y => y.Key, g => g.ToList());
            //
            //         var deliveryTimeResponse = foodAssignmentGroupByPartner.Select(x =>
            //         {
            //             var deliveryTime = x.Key.Value.AddHours(-1);
            //             // Tạo danh sách FoodAssignmentResponse cho mỗi SupplierFoodAssignment trong x.Value
            //             var foodAssignmentResponses = x.Value.Select(supplierFoodAssignment =>
            //             {
            //                 var foodName = supplierFoodAssignment.Food?.Name;
            //                 if (supplierFoodAssignment.Food?.FoodStatus == FoodStatus.Combo)
            //                 {
            //                     foodName += " - khẩu phần combo";
            //                 }
            //                 else if (supplierFoodAssignment.Food?.FoodStatus == FoodStatus.Retail)
            //                 {
            //                     foodName += " - khẩu phần đơn lẻ";
            //                 }
            //                 // Tạo một FoodAssignmentResponse mới từ SupplierFoodAssignment
            //                 return new FoodAssignmentResponse
            //                 {
            //                     Id = supplierFoodAssignment.Id,
            //                     FoodName = foodName,
            //                     AmountCooked = supplierFoodAssignment.AmountCooked,
            //                     ReceivedAmount = supplierFoodAssignment.ReceivedAmount,
            //                     CommissionRate = supplierFoodAssignment.SupplierCommissionRate?.CommissionRate,
            //                     Status = supplierFoodAssignment.Status.ToString()
            //                 };
            //             }).ToList();
            //             return new SupplierDeliveryTime(deliveryTime, foodAssignmentResponses);
            //         }).ToList();
            //         
            //         return new FoodAssignmentGroupByPartner(partnerName, deliveryTimeResponse);
            //         
            //     }).ToList();
            //     
            //     // Tạo một SupplierFoodAssignmentForSupplier mới với thông tin đầy đủ
            //     return new SupplierFoodAssignmentForSuperAdmin(DateOnly.FromDateTime(creationDate), bookingDate, foodAssignmentGroupByPartnerResponse);
            // }).ToList();
            //
            //
            // result.Payload = supplierFoodAssignmentResponse;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<List<FoodAssignmentGroupByCompany>>> GetSupplierFoodAssignmentDetailBySupplier(
        List<Guid> packageIds)
    {
        var result = new OperationResult<List<FoodAssignmentGroupByCompany>>();
        var userId = _claimsService.GetCurrentUserId;
        try
        {
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId);
            var supplierAssignment =
                await _unitOfWork.SupplierFoodAssignmentRepository.GetForSupplierByPackage(packageIds,
                    user.SupplierId!.Value);
            var foodAssignmentForCompany = supplierAssignment
                .GroupBy(x => x.Package.DailyOrder.MealSubscription.Company.Name)
                .ToDictionary(x => x.Key, g => g.ToList());
            ;
            var foodAssignmentForCompanyResponse = foodAssignmentForCompany.Select(x =>
            {
                var companyName = x.Key;
                var supplierFoodAssignments = _mapper.Map<List<FoodAssignmentResponse>>(x.Value);
                return new FoodAssignmentGroupByCompany(companyName, supplierFoodAssignments);
            }).ToList();
            result.Payload = foodAssignmentForCompanyResponse;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<List<FoodAssignmentGroupByCompany>>> GetSupplierFoodAssignmentDetailByPartner(
        FoodAssignmentByPartnerRequest request)
    {
        var result = new OperationResult<List<FoodAssignmentGroupByCompany>>();
        var userId = _claimsService.GetCurrentUserId;
        try
        {
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId);
            var supplier = await _unitOfWork.SupplierRepository.GetSupplierById((Guid)request.SupplierId);
            if (supplier is null)
            {
                result.AddError(ErrorCode.BadRequest, "Nhà cung cấp không tồn tại");
                return result;
            }

            var supplierAssignment =
                await _unitOfWork.SupplierFoodAssignmentRepository.GetForPartnerByPackage(request.PackageIds,
                    user.PartnerId!.Value, request.SupplierId);
            var foodAssignmentForCompany = supplierAssignment
                .GroupBy(x => x.Package.DailyOrder.MealSubscription.Company.Name)
                .ToDictionary(x => x.Key, g => g.ToList());
            ;
            var foodAssignmentForCompanyResponse = foodAssignmentForCompany.Select(x =>
            {
                var companyName = x.Key;
                var supplierFoodAssignments = _mapper.Map<List<FoodAssignmentResponse>>(x.Value);
                return new FoodAssignmentGroupByCompany(companyName, supplierFoodAssignments);
            }).ToList();
            result.Payload = foodAssignmentForCompanyResponse;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    public async Task<OperationResult<bool>> ConfirmAllFoodAssignments(Guid packageId)
    {
        var result = new OperationResult<bool>();
        var userId = _claimsService.GetCurrentUserId;
        try
        {
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId);
            
            var dailyOrder = await _unitOfWork.PackageRepository.FindAll(x => x.Id == packageId)
                .Select(x => x.DailyOrder).SingleOrDefaultAsync();
            if (dailyOrder is not { Status: DailyOrderStatus.Divided })
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
            
            // Retrieve all supplier food assignments related to the packages
            var assignments = await _unitOfWork.SupplierFoodAssignmentRepository.GetByPackages(packageId, user.SupplierId.Value);
            
            // Confirm all assignments if possible
            foreach (var assignment in assignments)
            {
                if (assignment.Status == SupplierFoodAssignmentStatus.Pending)
                {
                    assignment.Status = SupplierFoodAssignmentStatus.Confirmed;
                }
            }
            
            _unitOfWork.SupplierFoodAssignmentRepository.UpdateRange(assignments);
            
            var supplierFoodAssignments =
                await _unitOfWork.SupplierFoodAssignmentRepository
                    .GetByPackage(packageId);
            var allConfirmed = supplierFoodAssignments.All(a => a.Status == SupplierFoodAssignmentStatus.Confirmed);
            if (allConfirmed)
            {
                dailyOrder.Status = DailyOrderStatus.Cooking;
                _unitOfWork.DailyOrderRepository.Update(dailyOrder);
            }
            
            var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
            result.Payload = isSuccess; // Set the payload to the status of all confirmations
            
        }
        catch (Exception ex)
        {
            result.AddUnknownError(ex.Message);
        }

        return result;
    }

}