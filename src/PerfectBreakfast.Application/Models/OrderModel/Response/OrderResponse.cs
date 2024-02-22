﻿
namespace PerfectBreakfast.Application.Models.OrderModel.Response
{
    public record OrderResponse
    {
        public Guid Id { get; set; }
        public string Note { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public string OrderStatus { get; set; }
        public int OrderCode { get; set; }
        public DateTime CreationDate { get; set; }
        public List<OrderDetailResponse> orderDetails { get; set; }

    }
}
