namespace PerfectBreakfast.Domain.Enums;

public enum OrderStatus
{
    Complete,  
    Pending,   // chờ thanh toán
    Cancle,    // đã hủy 
    Paid       // đã thanh toán
}