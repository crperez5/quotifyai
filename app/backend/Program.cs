var builder = WebApplication.CreateBuilder(args);

builder.Configuration.ConfigureAzureKeyVault();

builder.Services.AddAzureServices();

builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

app.MapApi();

app.Run();