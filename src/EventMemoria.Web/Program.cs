using EventMemoria.Web.Common;

var builder = PmoWebApp.CreateBuilder(args);

var app = builder.BuildWebApp();

await app.RunAsync();
