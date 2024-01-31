using Mapster;
using PerfectBreakfast.Application.Models.CategoryModels.Response;
using PerfectBreakfast.Application.Models.DaliyOrder.Response;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;
using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Application.Models.PartnerModels.Response;
using PerfectBreakfast.Application.Models.SupplyAssigmentModels.Response;
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
        config.NewConfig<Delivery, DeliveryResponseModel>();
        config.NewConfig<Partner, PartnerResponseModel>();
        config.NewConfig<SupplyAssignment, SupplyAssigmentResponse>();
        config.NewConfig<DailyOrder, DailyOrderModelResponse>()
            .Map(dest => dest.Company,src => src.Company);
    }
}