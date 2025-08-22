using Azure.Storage.Blobs;
using Vogelhochzeit.Services;

namespace Vogelhochzeit;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureStorage");

        if (!string.IsNullOrEmpty(connectionString))
        {
            services.AddSingleton(x => new BlobServiceClient(connectionString));
            services.AddScoped<IStorageService, BlobStorageService>();
        }

        return services;
    }
}
