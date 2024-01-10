using Mapster;
using PerfectBreakfast.Application.Models.CategoryModels.Response;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;
using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Application.Models.ManagementUnitModels.Resposne;
using PerfectBreakfast.Application.Models.RoleModels.Response;
using PerfectBreakfast.Application.Models.UserModels.Response;
using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Infrastructure.Mappers;

public class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserResponse>();
        config.NewConfig<Food,FoodResponse>();
        config.NewConfig<Category, CategoryResponse>();
        config.NewConfig<DeliveryUnit, DeliveryUnitResponseModel>();
        config.NewConfig<ManagementUnit, ManagementUnitResponseModel>();
    }
}