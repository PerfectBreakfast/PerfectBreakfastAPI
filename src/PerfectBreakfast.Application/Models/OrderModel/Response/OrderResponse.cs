
using PerfectBreakfast.Application.Models.CompanyModels.Response;
using PerfectBreakfast.Application.Models.UserModels.Response;

namespace PerfectBreakfast.Application.Models.OrderModel.Response
{
    public sealed record OrderResponse
    {
        public Guid Id { get; set; }
        public string Note { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public string OrderStatus { get; set; }
        public int OrderCode { get; set; }
        public DateTime CreationDate { get; set; }
        public string? PaymentMethod { get; set; }
        public DateOnly BookingDate { get; set; }
        public string? Meal { get; set; }
        public UserResponse? User { get; set; }
        public CompanyDto? Company { get; set; }
        public List<OrderDetailResponse> OrderDetails { get; set; }
    }
}
