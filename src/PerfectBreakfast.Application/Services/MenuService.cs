using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.FoodModels.Response;
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

        public async Task<OperationResult<MenuResponse>> CreateMenu(CreateMenuFoodRequest createMenuFoodRequest)
        {
            var result = new OperationResult<MenuResponse>();
            try
            {
                var menu = _mapper.Map<Menu>(createMenuFoodRequest);
                var menuFood = _mapper.Map<ICollection<MenuFood?>>(createMenuFoodRequest.MenuFoodRequests);
                menu.MenuFoods = menuFood;
                await _unitOfWork.MenuRepository.AddAsync(menu);
                await _unitOfWork.SaveChangeAsync();
                result.Payload = _mapper.Map<MenuResponse>(menu);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<MenuResponse>> CreateMenuAndCombo(CreateMenuAndComboRequest createMenuAndComboRequest)
        {
            var result = new OperationResult<MenuResponse>();
            try
            {
                var menu = _mapper.Map<Menu>(createMenuAndComboRequest);
                var list = new List<MenuFood>();
                foreach (var mf in createMenuAndComboRequest.MenuFoodRequests)
                {
                    var menuFood = mf.ComboId is not null
                        ? new MenuFood { ComboId = (Guid)mf.ComboId }
                        : mf.FoodId is not null
                            ? new MenuFood { FoodId = mf.FoodId }
                            : mf.CreateComboRequest is not null
                                ? new MenuFood
                                {
                                    Combo = new Combo
                                    {
                                        Name = mf.CreateComboRequest.Name,
                                        Content = mf.CreateComboRequest.Content,
                                        ComboFoods = mf.CreateComboRequest.ComboFoodRequests
                                            .Select(cf => new ComboFood { FoodId = cf.FoodId })
                                            .ToList()
                                    }
                                }
                                : null;

                    if (menuFood != null)
                    {
                        list.Add(menuFood);
                    }
                }
                menu.MenuFoods = list;
                var entity = await _unitOfWork.MenuRepository.AddAsync(menu);
                await _unitOfWork.SaveChangeAsync();
                result.Payload = _mapper.Map<MenuResponse>(entity);
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
            catch (NotFoundIdException)
            {
                result.AddUnknownError("Id is not exsit");
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
            catch (NotFoundIdException)
            {
                result.AddUnknownError("Id is not exsit");
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
                var menu = await _unitOfWork.MenuRepository.GetMenuFoodByIdAsync(id);
                if (menu is null)
                {
                    result.AddUnknownError("Id is not exsit");
                    return result;
                }
                //var menu = await _unitOfWork.MenuRepository.GetByIdAsync(id, x => x.MenuFoods);

                // Lấy danh sách Food từ Menu
                var foodEntities = menu.MenuFoods.Select(cf => cf.Food).ToList();
                var foodResponses = _mapper.Map<List<ComboAndFoodResponse?>>(foodEntities);

                // Lấy danh sách Combo từ Menu
                var comboEntities = menu.MenuFoods.Select(cf => cf.Combo).ToList();
                var comboResponses = new List<ComboAndFoodResponse>();

                // Duyệt qua từng Combo để lấy thông tin chi tiết
                foreach (var combo in comboEntities)
                {
                    if (combo == null)
                    {
                        continue;
                    }
                    var detailedCombo = await _unitOfWork.ComboRepository.GetComboFoodByIdAsync(combo.Id);

                    // Lấy danh sách Food từ Combo
                    var foodEntitiesInCombo = detailedCombo.ComboFoods.Select(cf => cf.Food).ToList();
                    var foodResponsesInCombo = _mapper.Map<List<FoodResponse?>>(foodEntitiesInCombo);

                    // Ánh xạ Combo chi tiết sang DTO
                    var comboResponse = _mapper.Map<ComboAndFoodResponse>(detailedCombo);
                    comboResponse.FoodResponses = foodResponsesInCombo;

                    comboResponses.Add(comboResponse);
                }

                // Ánh xạ Menu chi tiết sang DTO
                var menuResponse = _mapper.Map<MenuResponse>(menu);
                menuResponse.ComboFoodResponses = foodResponses;
                menuResponse.ComboFoodResponses = comboResponses;
                result.Payload = menuResponse;
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
                var menuEntity = await _unitOfWork.MenuRepository.GetByIdAsync(id);
                _mapper.Map(menuRequest, menuEntity);
                _unitOfWork.MenuRepository.Update(menuEntity);
                await _unitOfWork.SaveChangeAsync();
            }
            catch (NotFoundIdException)
            {
                result.AddUnknownError("Id is not exsit");
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }
    }
}
