namespace PerfectBreakfast.Application.Utils;

public class ConstantRole
{
    public const string SUPER_ADMIN = "SUPER ADMIN";
    public const string CUSTOMER = "CUSTOMER";
    public const string SUPPLIER_ADMIN = "SUPPLIER ADMIN";
    public const string PARTNER_ADMIN = "PARTNER ADMIN";
    public const string DELIVERY_ADMIN = "DELIVERY ADMIN";
    public const string DELIVERY_STAFF = "DELIVERY STAFF";
    
    
    // Policy 
    public const string RequireDeliveryAdminRole = "RequireDeliveryAdminRole";
    public const string RequirePartnerAdminRole = "RequirePartnerAdminRole";
    public const string RequireSuperAdminRole = "RequireSuperAdminRole";
    public const string RequireCustomerRole = "RequireCustomerRole";
    public const string RequireDeliveryStaffRole = "RequireDeliveryStaffRole";
    public const string RequireSuperAdminAndCustomerRole = "RequireSuperAdminAndCustomerRole";
}