using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.MenuModels.Request;
using PerfectBreakfast.Application.Models.MenuModels.Response;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Services
{
    public class MenuService : IMenuService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MenuService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OperationResult<MenuResponse>> CreateMenu(MenuRequest menuRequest)
        {
            var result = new OperationResult<MenuResponse>();
            try
            {
                var menu = _mapper.Map<Menu>(menuRequest);
                await _unitOfWork.MenuRepository.AddAsync(menu);
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<MenuResponse>> Delete(Guid id)
        {
            var result = new OperationResult<MenuResponse>();
            try
            {
                var menu = await _unitOfWork.MenuRepository.GetByIdAsync(id);
                _unitOfWork.MenuRepository.Remove(menu);
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<MenuResponse>> DeleteMenu(Guid id)
        {
            var result = new OperationResult<MenuResponse>();
            try
            {
                var menu = await _unitOfWork.MenuRepository.GetByIdAsync(id);
                _unitOfWork.MenuRepository.SoftRemove(menu);
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<MenuResponse>> GetMenu(Guid id)
        {
            var result = new OperationResult<MenuResponse>();
            try
            {
                var menu = await _unitOfWork.MenuRepository.GetByIdAsync(id);
                result.Payload = _mapper.Map<MenuResponse>(menu);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<Pagination<MenuResponse>>> GetMenuPaginationAsync(int pageIndex = 0, int pageSize = 10)
        {
            var result = new OperationResult<Pagination<MenuResponse>>();
            try
            {
                var menu = await _unitOfWork.MenuRepository.ToPagination(pageIndex, pageSize);
                result.Payload = _mapper.Map<Pagination<MenuResponse>>(menu);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<List<MenuResponse>>> GetMenus()
        {
            var result = new OperationResult<List<MenuResponse>>();
            try
            {
                var menu = await _unitOfWork.MenuRepository.GetAllAsync();
                result.Payload = _mapper.Map<List<MenuResponse>>(menu);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<MenuResponse>> UpdateMenu(Guid id, MenuRequest menuRequest)
        {
            var result = new OperationResult<MenuResponse>();
            try
            {
                var menu = _mapper.Map<Menu>(menuRequest);
                menu.Id = id;
                _unitOfWork.MenuRepository.Update(menu);
                await _unitOfWork.SaveChangeAsync();
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }
    }
}
