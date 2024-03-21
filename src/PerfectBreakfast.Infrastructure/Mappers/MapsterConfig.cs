using Mapster;
using PerfectBreakfast.Application.Models.CategoryModels.Response;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;
using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Application.Models.MealModels.Response;
using PerfectBreakfast.Application.Models.OrderModel.Response;
using PerfectBreakfast.Application.Models.PartnerModels.Response;
using PerfectBreakfast.Application.Models.SupplierModels.Response;
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
        /*config.NewConfig<DailyOrder, DailyOrderModelResponse>()
            .Map(dest => dest.Company,src => src.Company);*/
        config.NewConfig<Order, OrderHistoryResponse>()
            .Map(dest => dest.ComboCount, src => src.OrderDetails.Select(x => x.Quantity).Sum())
            .Map(dest => dest.CompanyName,src => src.Worker.Company.Name);
        config.NewConfig<Food, FoodResponeCategory>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Price, src => src.Price)
            .Map(dest => dest.Image, src => src.Image)
            .Map(dest => dest.CategoryResponse, src => src.Category);
        config.NewConfig<Category, CategoryResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name);
        config.NewConfig<Supplier, SupplierDetailResponse>()
            .Map(dest => dest.ManagementUnitDtos, src => src.SupplyAssignments.Select(x => x.Partner))
            .Map(dest => dest.CommissionRates, src => src.SupplierCommissionRates);
        config.NewConfig<Category, CategoryDetailFood>()
            .Map(dest => dest.FoodResponse, src => src.Foods);
        config.NewConfig<MealSubscription, MealResponse>()
            .Map(dest => dest.Id, src => src.Meal.Id)
            .Map(dest => dest.MealType, src => src.Meal.MealType)
            .Map(dest => dest.StartTime, src => src.StartTime)
            .Map(dest => dest.EndTime, src => src.EndTime);
    }
}