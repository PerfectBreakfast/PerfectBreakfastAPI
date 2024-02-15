﻿

using PerfectBreakfast.Domain.Enums;

namespace PerfectBreakfast.Application.Models.ShippingOrder.Response;

public record class ShippingOrderRespone
{
    public String Status { get; set; }
    public Guid? DailyOrderId { get; set; }
    public Guid? ShipperId { get; set; }
}