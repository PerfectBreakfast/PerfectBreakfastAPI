using BenchmarkDotNet.Reports;
using Mapster;
using PerfectBreakfast.Application.Models.CategoryModels.Response;
using PerfectBreakfast.Application.Models.ComboModels.Response;
using PerfectBreakfast.Application.Models.DailyOrder.Response;
using PerfectBreakfast.Application.Models.DeliveryUnitModels.Response;
using PerfectBreakfast.Application.Models.FoodModels.Response;
using PerfectBreakfast.Application.Models.MealModels.Request;
using PerfectBreakfast.Application.Models.MealModels.Response;
using PerfectBreakfast.Application.Models.MenuModels.Response;
using PerfectBreakfast.Application.Models.OrderModel.Response;
using PerfectBreakfast.Application.Models.PartnerModels.Response;
using PerfectBreakfast.Application.Models.ShippingOrder.Response;
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

        // Order 
        config.NewConfig<Order, OrderHistoryResponse>()
            .Map(dest => dest.ComboCount, src => src.OrderDetails.Select(x => x.Quantity).Sum())
            .Map(dest => dest.CompanyName,src => src.Worker.Company.Name)
            .Map(dest => dest.DeliveryDate, src => src.DailyOrder.BookingDate)
            .Map(dest => dest.Meal, src => src.DailyOrder.MealSubscription.Meal.MealType);
        config.NewConfig<Order, OrderResponse>()
            .Map(dest => dest.PaymentMethod, src => src.PaymentMethod.Name)
            .Map(dest => dest.Meal, src => src.DailyOrder.MealSubscription.Meal.MealType)
            .Map(dest => dest.OrderDetails, src => src.OrderDetails);
        
        // Order Detail
        config.NewConfig<OrderDetail, OrderDetailResponse>()
            .Map(dest => dest.ComboName, src => src.Combo.Name)
            .Map(dest => dest.Image, src => src.Combo.Image)
            .Map(dest => dest.Foods, src => src.Food.Name);
        
        // Food 
        config.NewConfig<Food, FoodResponseCategory>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Price, src => src.Price)
            .Map(dest => dest.Image, src => src.Image)
            .Map(dest => dest.CategoryResponse, src => src.Category);
        
        // Category 
        config.NewConfig<Category, CategoryDetailFood>()
            .Map(dest => dest.FoodResponse, src => src.Foods);
        config.NewConfig<Category, CategoryResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name);
        
        // Supplier 
        config.NewConfig<Supplier, SupplierDetailResponse>()
            .Map(dest => dest.ManagementUnitDtos, src => src.SupplyAssignments.Select(x => x.Partner))
            .Map(dest => dest.CommissionRates, src => src.SupplierCommissionRates);
        
        // Meal 
        config.NewConfig<MealSubscription, MealResponse>()
            .Map(dest => dest.Id, src => src.Meal.Id)
            .Map(dest => dest.MealType, src => src.Meal.MealType)
            .Map(dest => dest.StartTime, src => src.StartTime)
            .Map(dest => dest.EndTime, src => src.EndTime);
        
        //Meal Subscription
        config.NewConfig<MealModel, MealSubscription>()
            .Map(dest => dest.CompanyId, src => src.MealId)
            .Map(dest => dest.MealId, src => src.MealId)
            .Map(dest => dest.StartTime, src => src.StartTime)
            .Map(dest => dest.EndTime, src => src.EndTime);
        
        // Menu 
        config.NewConfig<Menu,MenuResponse>()
            .Map(dest => dest.ComboFoodResponses, src => src.MenuFoods.Select(cf => cf.Combo).Where(combo => combo != null && !combo.IsDeleted))
            .Map(dest => dest.FoodResponses, src => src.MenuFoods.Select(cf => cf.Food).Where(food => food != null && !food.IsDeleted));
        config.NewConfig<Menu,MenuIsSelectedResponse>()
            .Map(dest => dest.ComboFoodResponses, src => src.MenuFoods.Select(cf => cf.Combo).Where(combo => combo != null && !combo.IsDeleted))
            .Map(dest => dest.FoodResponses, src => src.MenuFoods.Select(cf => cf.Food).Where(food => food != null && !food.IsDeleted));
        
        // Combo 
        config.NewConfig<Combo,ComboAndFoodResponse>()
            .Map(dest => dest.FoodResponses, src => src.ComboFoods.Select(cf => cf.Food))
            .Map(dest => dest.Foods, src => string.Join(", ", src.ComboFoods.Select(cf => cf.Food.Name)))
            .Map(dest => dest.Price, src => src.ComboFoods.Sum(cf => cf.Food.Price));
        config.NewConfig<Combo, ComboDetailResponse>()
            .Map(dest => dest.Foods, src => string.Join(", ", src.ComboFoods.Select(cf => cf.Food.Name)))
            .Map(dest => dest.comboPrice, src => src.ComboFoods.Sum(cf => cf.Food.Price));
        config.NewConfig<Combo, ComboResponse>()
            .Map(dest => dest.Foods, src => string.Join(", ", src.ComboFoods.Select(cf => cf.Food.Name)))
            .Map(dest => dest.comboPrice, src => src.ComboFoods.Sum(cf => cf.Food.Price));        
        
        // Partner 
        config.NewConfig<Partner,PartnerDetailResponse>()
            .Map(dest => dest.SupplierDTO, src => src.SupplyAssignments.Select(sa => sa.Supplier))
            .Map(dest => dest.Companies, src => src.Companies);
        
        // ShippingOrder
        config.NewConfig<ShippingOrder, ShippingOrderForShipperResponse>();
        
        // DailyOrder 
        config.NewConfig<DailyOrder, DailyOrderDto>()
            .Map(dest => dest.Meal, src => src.MealSubscription.Meal.MealType)
            .Map(dest => dest.PickupTime,src => src.MealSubscription.StartTime)
            .Map(dest => dest.HandoverTime,src => src.MealSubscription.StartTime)
            .Map(dest => dest.Company,src => src.MealSubscription.Company)
            .Map(dest => dest.Partner,src => src.MealSubscription.Company.Partner);
    }
}