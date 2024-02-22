namespace PerfectBreakfast.Domain.Enums;

public enum OrderStatus
{
    Complete,  
    Pending,   // chờ thanh toán
    Cancel,    // đã hủy 
    Paid       // đã thanh toán
}