namespace PerfectBreakfast.Application.Models.MenuModels.Response;

public record MenuResponsePaging
(
    Guid Id, 
    string Name,
    bool IsSelected,
    DateTime CreationDate,
    bool IsDeleted
 );