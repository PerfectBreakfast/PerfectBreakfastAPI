using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Services
{
    public class SupplierFoodAssignmentService : ISupplierFoodAssignmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SupplierFoodAssignmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OperationResult<List<SupplierFoodAssignmentResponse>>> CreateSupplierFoodAssignment(SupplierFoodAssignmentRequest request)
        {
            var result = new OperationResult<List<SupplierFoodAssignmentResponse>>();
            try
            {
                var supplierFoodAssignments = _mapper.Map<List<SupplierFoodAssignment>>(request);

            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);

            }
            return result;
        }

        public Task<OperationResult<List<SupplierFoodAssignmentResponse>>> GetSupplierFoodAssignment(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
