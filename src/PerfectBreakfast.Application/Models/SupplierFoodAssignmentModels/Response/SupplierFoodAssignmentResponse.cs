﻿namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response
{
    public record SupplierFoodAssignmentResponse
    {
        public Guid? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public List<FoodAssignmentResponse> FoodAssignmentResponses { get; set; } = null;
    }
}
