using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Repositories
{
    public interface ISupplierFoodAssignmentRepository : IGenericRepository<SupplierFoodAssignment>
    {
        public Task<List<SupplierFoodAssignment>> GetForSupplierByPackage(List<Guid> packageIds, Guid supplierId);
        public Task<List<SupplierFoodAssignment>> GetForPartnerByPackage(List<Guid> packageIds, Guid partnerId, Guid supplierId);
        public Task<List<SupplierFoodAssignment>> GetByPackage(Guid packageId);
        public Task<List<SupplierFoodAssignment>> GetByPackages(Guid packageId, Guid supplierId);
        public Task<Pagination<SupplierFoodAssignment>> ToPaginationByPartner(List<Guid> packageId, int pageNumber = 0,
            int pageSize = 10);
        public Task<Pagination<SupplierFoodAssignment>> ToPaginationBySupplier(List<Guid> packageId, int pageNumber = 0,
            int pageSize = 10);
        public Task<List<SupplierFoodAssignment>> GetByForSuperAdmin(DateOnly? fromDate, DateOnly? toDate);
    }
}
