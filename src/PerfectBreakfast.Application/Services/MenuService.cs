using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.CustomExceptions;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.MenuModels.Request;
using PerfectBreakfast.Application.Models.MenuModels.Response;
using PerfectBreakfast.Domain.Entities;
using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Memory;

namespace PerfectBreakfast.Application.Services
{
    public class MenuService : IMenuService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentTime _currentTime;
        private readonly IMemoryCache _cache;
        private readonly string cacheKey = "hehehe";

        public MenuService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentTime currentTime, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentTime = currentTime;
            _cache = cache;
        }

        public async Task<OperationResult<MenuResponse>> ChooseMenu(Guid id)
        {
            var result = new OperationResult<MenuResponse>();
            try
            {
                var menu = await _unitOfWork.MenuRepository.GetMenuFoodByStatusAsync();

                // Kiểm tra nếu 'menu' là null
                if (menu != null)
                {
                    menu.IsSelected = false;
                    _unitOfWork.MenuRepository.Update(menu);
                    var menuEntity = await _unitOfWork.MenuRepository.GetByIdAsync(id);
                    menuEntity.IsSelected = true;
                    _unitOfWork.MenuRepository.Update(menuEntity);
                    await _unitOfWork.SaveChangeAsync();
                }
                else
                {
                    var menuEntity = await _unitOfWork.MenuRepository.GetByIdAsync(id);
                    menuEntity.IsSelected = true;
                    _unitOfWork.MenuRepository.Update(menuEntity);
                    await _unitOfWork.SaveChangeAsync();
                }
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

        public async Task<OperationResult<MenuResponse>> CreateMenu(CreateMenuFoodRequest createMenuFoodRequest)
        {
            var result = new OperationResult<MenuResponse>();
            try
            {
                var menu = _mapper.Map<Menu>(createMenuFoodRequest);
                var list = new List<MenuFood>();
                foreach (var mf in createMenuFoodRequest.MenuFoodRequests)
                {
                    var menuFood = mf.ComboId is not null
                        ? new MenuFood { ComboId = (Guid)mf.ComboId }
                        : mf.FoodId is not null
                            ? new MenuFood { FoodId = mf.FoodId }
                            : null;

                    if (menuFood != null)
                    {
                        list.Add(menuFood);
                    }
                }

                menu.MenuFoods = list;
                var entity = await _unitOfWork.MenuRepository.AddAsync(menu);
                var isSuccess = await _unitOfWork.SaveChangeAsync() > 0;
                if (!isSuccess)
                {
                    result.AddError(ErrorCode.ServerError, "Food or Combo is not exist");
                    return result;
                }

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
                if (menu.IsSelected)
                {
                    result.AddError(ErrorCode.BadRequest, "Menu is currently selected");
                    return result;
                }

                _unitOfWork.MenuRepository.SoftRemove(menu);
                await _unitOfWork.SaveChangeAsync();
            }
            catch (NotFoundIdException)
            {
                result.AddUnknownError("Id is not exist");
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
                // Ánh xạ Menu chi tiết sang DTO
                var menuResponse = _mapper.Map<MenuResponse>(menu);
                result.Payload = menuResponse;
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }

            return result;
        }

        public async Task<OperationResult<MenuIsSelectedResponse>> GetMenuByStatus()
        {
            var result = new OperationResult<MenuIsSelectedResponse>();
            var currentTime = _currentTime.GetCurrentTime();
            if (_cache.TryGetValue(cacheKey, out MenuIsSelectedResponse menuResponse))
            {
                Console.WriteLine("Đã có cache được lưu ròi");
                result.Payload = menuResponse;
                return result;
            }

            try
            {
                var menu = await _unitOfWork.MenuRepository.GetMenuFoodByStatusAsync();
                if (menu is null)
                {
                    result.AddError(ErrorCode.NotFound,"No menu selected");
                    return result;
                }
                
                // lấy time settings 
                var  id = new Guid("a2ec296c-078d-4067-8ab7-14324d7620fa");                     // đang fix cứng ID của setting 
                var setting = await _unitOfWork.SettingRepository.GetByIdAsync(id);
                var menuDate = currentTime.AddDays(currentTime.Hour < setting.Time.Hour ? 1 : 2);

                // Ánh xạ Menu chi tiết sang DTO
                menuResponse = _mapper.Map<MenuIsSelectedResponse>(menu);
                menuResponse = menuResponse with { MenuDate = menuDate };
                result.Payload = menuResponse;

                // tạo option cho cache 
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(45)) // thời gian cache hết hạn nếu không có ai gọi tới 
                    .SetAbsoluteExpiration(
                        TimeSpan.FromSeconds(3600)) // thời gian mặc định cache sẽ phải hết hạn dù có gọi tới hay không 
                    .SetPriority(CacheItemPriority.Normal);

                // set lại data vào cache 
                Console.WriteLine("- Đã save cache mới -");
                _cache.Set(cacheKey, menuResponse, cacheEntryOptions);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<Pagination<MenuResponse>>> GetMenuPaginationAsync(string? searchTerm,
            int pageIndex = 0, int pageSize = 10)
        {
            var result = new OperationResult<Pagination<MenuResponse>>();
            try
            {
                // Tạo biểu thức tìm kiếm (predicate)
                Expression<Func<Menu, bool>>? searchPredicate = string.IsNullOrEmpty(searchTerm)
                    ? (x => !x.IsDeleted)
                    : (x => x.Name.ToLower().Contains(searchTerm.ToLower()) && !x.IsDeleted);
                var menu = await _unitOfWork.MenuRepository.ToPagination(pageIndex, pageSize, searchPredicate);
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

        public async Task<OperationResult<MenuResponse>> UpdateMenu(Guid id, UpdateMenuRequest menuRequest)
        {
            var result = new OperationResult<MenuResponse>();
            try
            {
                var menuEntity = await _unitOfWork.MenuRepository.GetByIdAsync(id, m => m.MenuFoods);
                foreach (var menuFood in menuEntity.MenuFoods)
                {
                    _unitOfWork.MenuFoodRepository.Remove(menuFood);
                }

                var list = new List<MenuFood>();
                foreach (var mf in menuRequest.MenuFoodRequests)
                {
                    var menuFood = mf.ComboId is not null
                        ? new MenuFood { ComboId = (Guid)mf.ComboId }
                        : mf.FoodId is not null
                            ? new MenuFood { FoodId = mf.FoodId }
                            : null;

                    if (menuFood != null)
                    {
                        list.Add(menuFood);
                    }
                }

                _mapper.Map(menuRequest, menuEntity);
                menuEntity.MenuFoods = list;
                _unitOfWork.MenuRepository.Update(menuEntity);
                await _unitOfWork.SaveChangeAsync();
                result.Payload = _mapper.Map<MenuResponse>(menuEntity);
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