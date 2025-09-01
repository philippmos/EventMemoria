using Blazored.LocalStorage;
using EventMemoria.Web.Common.Settings;
using EventMemoria.Web.Components;
using Microsoft.AspNetCore.DataProtection;
using MudBlazor.Services;

namespace EventMemoria.Web.Common;

public static class PmoWebApp
{
    public static WebApplicationBuilder CreateBuilder(params string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddMudServices();

        builder.Services.AddHealthChecks();

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddServices(builder.Configuration);
        builder.Services.AddConfigurations(builder.Configuration);
        builder.Services.AddDataProtection(builder.Configuration);

        builder.Services.AddBlazoredLocalStorage();

        return builder;
    }

    public static WebApplication BuildWebApp(this WebApplicationBuilder builder)
    {
        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.MapHealthChecks("/health");

        return app;
    }

    private static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PhotoOptions>(configuration.GetSection(nameof(PhotoOptions)));
        services.Configure<CustomizationOptions>(configuration.GetSection(nameof(CustomizationOptions)));

        return services;
    }

    private static IServiceCollection AddDataProtection(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureStorage");

        var containerName = configuration.GetValue<string>("DataProtectionContainerName") ?? "dataprotectionkeys";

        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));

        services.AddDataProtection()
                .PersistKeysToAzureBlobStorage(connectionString, containerName, "keys.xml")
                .SetApplicationName(typeof(PmoWebApp).Assembly.GetName().Name!);

        return services;
    }
}
