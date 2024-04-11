using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.StatisticResponse;

public record SupplierFoodAssignmentForSuperAdmin
    (
        DateOnly? CreationDate,
        DateOnly? BookingDate,
        List<FoodAssignmentGroupForSupplier> FoodAssignmentGroupBySuppliers
        );