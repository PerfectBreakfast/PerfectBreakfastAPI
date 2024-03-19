namespace PerfectBreakfast.API.Contracts.Commons;

public sealed record ErrorResponse(int StatusCode,string StatusPhrase,dynamic Errors);