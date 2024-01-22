using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Request;
using PerfectBreakfast.Application.Models.SupplierFoodAssignmentModels.Response;
using PerfectBreakfast.Domain.Entities;
using PerfectBreakfast.Domain.Enums;

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
                var supplierFoodAssignmentsReslt = new List<SupplierFoodAssignment>();
                foreach (var supplierFoodAssignment in supplierFoodAssignments)
                {
                    supplierFoodAssignment.Status = SupplierFoodAssignmentStatus.Sent;
                    await _unitOfWork.SupplierFoodAssignmentRepository.AddAsync(supplierFoodAssignment);
                    await _unitOfWork.SaveChangeAsync();
                    supplierFoodAssignmentsReslt.Add(supplierFoodAssignment);
                }
                result.Payload = _mapper.Map<List<SupplierFoodAssignmentResponse>>(supplierFoodAssignmentsReslt);
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
