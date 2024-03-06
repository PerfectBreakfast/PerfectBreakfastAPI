using Imgur.API.Authentication;
using Imgur.API.Endpoints;
using Microsoft.AspNetCore.Http;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;

namespace PerfectBreakfast.Infrastructure.ImgurServices;

public class ImgurService : IImgurService
{
    private readonly AppConfiguration _appConfiguration;

    public ImgurService(AppConfiguration appConfiguration)
    {
        _appConfiguration = appConfiguration;
    }
    public async Task<string> UploadImageAsync(IFormFile? file)
    {
        var apiClient = new ApiClient(_appConfiguration.ClientImgurId);
        var httpClient = new HttpClient();
        if (file is not { Length: > 0 }) return string.Empty;
        var imageEndpoint = new ImageEndpoint(apiClient, httpClient);

        await using var fileStream = file.OpenReadStream();
        var imageUpload = await imageEndpoint.UploadImageAsync(fileStream);
        return imageUpload?.Link;
    }
}