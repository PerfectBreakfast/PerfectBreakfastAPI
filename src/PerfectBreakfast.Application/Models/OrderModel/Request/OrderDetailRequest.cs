namespace PerfectBreakfast.Application.Models.OrderModel.Request;

public record OrderDetailRequest
{
    public int Quantity { get; set; }
    public Guid? FoodId { get; set; }
    public Guid? ComboId { get; set; }
}
