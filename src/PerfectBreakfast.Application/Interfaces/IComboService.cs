using Microsoft.AspNetCore.Http;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.ComboModels.Request;
using PerfectBreakfast.Application.Models.ComboModels.Response;

namespace PerfectBreakfast.Application.Interfaces
{
    public interface IComboService
    {
        public Task<OperationResult<List<ComboResponse>>> GetCombos();
        public Task<OperationResult<ComboResponse>> GetCombo(Guid id);
        public Task<OperationResult<ComboResponse>> CreateCombo(CreateComboRequest createComboRequest);
        public Task<OperationResult<ComboResponse>> DeleteCombo(Guid id);
        public Task<OperationResult<ComboResponse>> UpdateCombo(Guid id, ComboRequest comboRequest);
        public Task<OperationResult<Pagination<ComboResponse>>> GetComboPaginationAsync(int pageIndex = 0, int pageSize = 10);
        public Task<OperationResult<ComboResponse>> Delete(Guid id);
    }
}
