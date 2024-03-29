﻿using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Models.DailyOrder.Request
{
    public record UpdateDailyOrderRequest
    {
        public decimal? TotalPrice { get; set; }
        public int? OrderQuantity { get; set; }
        public DateOnly BookingDate { get; set; }
        public DailyOrderStatus Status { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? AdminId { get; set; }
    }
}
