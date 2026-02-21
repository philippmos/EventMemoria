using Blazored.LocalStorage;
using EventMemoria.Web.Common.Settings;
using EventMemoria.Web.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.FeatureManagement;
using MudBlazor.Services;

namespace EventMemoria.Web.Common;

public static class PmoWebApp
{
    public static WebApplicationBuilder CreateBuilder(params string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile("appsettings.dev.json", optional: true, reloadOnChange: true);
        builder.Configuration.AddKeyPerFile("/run/secrets", optional: true);

        builder.Services.AddHttpClient();

        builder.Services.AddMudServices();

        builder.Services.AddHealthChecks();

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddFeatureManagement();

        builder.Services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(365);
        });

        builder.SetupAntiforgery();

        builder.Services.AddServices(builder.Configuration);
        builder.Services.AddConfigurations(builder.Configuration);
        builder.Services.AddDataProtection(builder.Configuration);

        builder.Services.AddBlazoredLocalStorage();

        return builder;
    }

    public static WebApplication BuildWebApp(this WebApplicationBuilder builder)
    {
        var app = builder.Build();

        if (builder.Configuration.GetValue<bool>("IsProduction"))
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            
            app.UseHttpsRedirection();
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.MapHealthChecks("/health");

        return app;
    }

    private static void SetupAntiforgery(this WebApplicationBuilder builder)
    {
        var isProduction = builder.Configuration.GetValue<bool>("IsProduction");

        builder.Services.AddAntiforgery(options =>
        {
            options.Cookie.SecurePolicy = isProduction ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
        });
    }

    private static void AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PhotoOptions>(configuration.GetSection(nameof(PhotoOptions)));
        services.Configure<CustomizationOptions>(configuration.GetSection(nameof(CustomizationOptions)));
    }

    private static void AddDataProtection(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureStorage");

        var containerName = configuration.GetValue<string>("DataProtectionContainerName") ?? "dataprotectionkeys";

        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));

        services.AddDataProtection()
                .PersistKeysToAzureBlobStorage(connectionString, containerName, "keys.xml")
                .SetApplicationName(typeof(PmoWebApp).Assembly.GetName().Name!);
    }
}
