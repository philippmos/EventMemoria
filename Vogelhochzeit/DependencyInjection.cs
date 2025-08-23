using Azure.Storage.Blobs;
using Vogelhochzeit.Services;
using Vogelhochzeit.Services.Interfaces;

namespace Vogelhochzeit;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureStorage");

        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));

        services.AddSingleton(x => new BlobServiceClient(connectionString));
        services.AddScoped<IStorageService, BlobStorageService>();

        services.AddScoped<IPhotoGridService, PhotoGridService>();
        services.AddScoped<IFileValidationService, FileValidationService>();

        return services;
    }
}
