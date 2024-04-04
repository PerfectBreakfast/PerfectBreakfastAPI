using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.PackageModels.Response;

namespace PerfectBreakfast.Application.Services;

public class PackageService : IPackageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IClaimsService _claimsService;
    private readonly ICurrentTime _currentTime;

    public PackageService(IUnitOfWork unitOfWork, IMapper mapper, IClaimsService claimsService, ICurrentTime currentTime)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _claimsService = claimsService;
        _currentTime = currentTime;
    }


    public async Task<OperationResult<List<PackageResponse>>> GetPackageForPartner()
    {
        var result = new OperationResult<List<PackageResponse>>();
        var userId = _claimsService.GetCurrentUserId;
        try
        {
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId);
            var packages = await _unitOfWork.PackageRepository.GetByPartner(user.PartnerId);
            var packageGroup =  packages.GroupBy(p => p.DailyOrder.BookingDate)
                .OrderByDescending(x => x.Key)
                .ToDictionary(x => x.Key, g => g.ToList());;
            var packageResponse = packageGroup.Select(x =>
            {
                var bookingDate = x.Key;
                
                var companyGroup = x.Value.GroupBy(p => p.DailyOrder.MealSubscription.Company.Name).ToDictionary(x => x.Key, g => g.ToList());;
                var companyResponse = companyGroup.Select(x =>
                {
                    var companyName = x.Key;
                    var mealGroup = x.Value.GroupBy(p => p.DailyOrder.MealSubscription.Meal.MealType).ToDictionary(x => x.Key, g => g.ToList());;
                    var mealResponse = mealGroup.Select(x =>
                    {
                        var mealType = x.Key;
                        var packages = _mapper.Map<List<PackageForMeal>>(x.Value);
                        return new MealForCompany(mealType,packages);
                    }).ToList();

                    return new CompanyForPackage(companyName, mealResponse);
                }).ToList();
                
                return new PackageResponse(bookingDate, companyResponse);
            }).ToList();
            result.Payload = packageResponse;
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }
}