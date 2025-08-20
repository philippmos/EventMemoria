using Vogelhochzeit.Common;

var builder = PmoWebApp.CreateBuilder(args);

var app = builder.BuildWebApp();

await app.RunAsync();
