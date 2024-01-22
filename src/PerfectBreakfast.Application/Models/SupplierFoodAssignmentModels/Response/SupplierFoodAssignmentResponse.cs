using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response
{
    public record SupplierFoodAssignmentResponse
    {
        public decimal? ReceivedAmount { get; set; }
        public Food? Food { get; set; }
    }
}
