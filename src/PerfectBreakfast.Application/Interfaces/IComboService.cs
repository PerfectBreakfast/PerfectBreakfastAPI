﻿using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.ComboModels.Request;
using PerfectBreakfast.Application.Models.ComboModels.Response;

namespace PerfectBreakfast.Application.Interfaces
{
    public interface IComboService
    {
        public Task<OperationResult<List<ComboResponse>>> GetCombos();
        public Task<OperationResult<ComboDetailResponse>> GetCombo(Guid id);
        public Task<OperationResult<ComboResponse>> CreateCombo(CreateComboRequest createComboRequest);
        public Task<OperationResult<ComboResponse>> DeleteCombo(Guid id);
        public Task<OperationResult<ComboResponse>> UpdateCombo(Guid id, UpdateComboRequest updateComboRequest);
        public Task<OperationResult<Pagination<ComboResponse>>> GetComboPaginationAsync(string? searchTerm, int pageIndex = 0, int pageSize = 10);
    }
}
