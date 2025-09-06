using Azure.Storage.Blobs;
using EventMemoria.Web.Services;
using EventMemoria.Web.Services.Interfaces;

namespace EventMemoria.Web;

public static class DependencyInjection
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureStorage");

        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));

        services.AddSingleton(_ => new BlobServiceClient(connectionString));

        services.AddScoped<IStorageService, BlobStorageService>();
        services.AddScoped<IUploadService, UploadService>();
        services.AddScoped<IPhotoGridService, PhotoGridService>();
        services.AddScoped<IFileValidationService, FileValidationService>();
        services.AddScoped<IUserPreferencesService, UserPreferencesService>();
    }
}
