using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Models.MenuModels.Request;
using PerfectBreakfast.Application.Models.MenuModels.Response;

namespace PerfectBreakfast.Application.Interfaces
{
    public interface IMenuService
    {
        public Task<OperationResult<List<MenuResponse>>> GetMenus();
        public Task<OperationResult<MenuResponse>> GetMenu(Guid id);
        public Task<OperationResult<MenuResponse>> CreateMenu(CreateMenuFoodRequest createMenuFoodRequest);
        public Task<OperationResult<MenuResponse>> DeleteMenu(Guid id);
        public Task<OperationResult<MenuResponse>> UpdateMenu(Guid id, MenuRequest menuRequest);
        public Task<OperationResult<Pagination<MenuResponse>>> GetMenuPaginationAsync(int pageIndex = 0, int pageSize = 10);
        public Task<OperationResult<MenuResponse>> Delete(Guid id);
    }
}
