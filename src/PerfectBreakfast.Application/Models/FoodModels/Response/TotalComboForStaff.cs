namespace PerfectBreakfast.Application.Models.FoodModels.Response;

public record TotalComboForStaff
(
    DateOnly? BookingDate,
    List<TotalFoodForCompanyResponse> TotalFoodForCompanyResponses
        );