using Microsoft.AspNetCore.Http;

namespace PerfectBreakfast.Application.Interfaces;

public interface IImgurService
{
    public Task<string> UploadImageAsync(IFormFile? file);
}