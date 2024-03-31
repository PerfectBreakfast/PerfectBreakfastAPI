using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories
{
    public interface ISupplierFoodAssignmentRepository : IGenericRepository<SupplierFoodAssignment>
    {
        public Task<List<SupplierFoodAssignment>> GetForSupplierByPackage(List<Guid?> packageIds, Guid supplierId);
        public Task<List<SupplierFoodAssignment>> GetForPartnerByPackage(List<Guid?> packageIds, Guid partnerId);
        public Task<List<SupplierFoodAssignment>> GetByPackage(Guid packageId);
        public Task<List<SupplierFoodAssignment>> GetByBookingDateForSupplier();
        public Task<List<SupplierFoodAssignment>> GetByForSuperAdmin();
    }
}
