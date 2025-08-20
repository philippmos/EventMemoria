using MudBlazor.Services;
using Vogelhochzeit.Components;

namespace Vogelhochzeit.Common;

public static class PmoWebApp
{
    public static WebApplicationBuilder CreateBuilder(params string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddMudServices();

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

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

        return app;
    }
}
