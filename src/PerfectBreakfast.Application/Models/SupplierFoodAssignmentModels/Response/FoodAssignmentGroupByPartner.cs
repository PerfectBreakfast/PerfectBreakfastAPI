﻿namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;

public record FoodAssignmentGroupByPartner
(
    string partnerName,
    List<SupplierDeliveryTime> SupplierDeliveryTimes)
    ;