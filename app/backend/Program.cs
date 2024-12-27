var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHealthChecks(builder.Configuration);

builder.Configuration.ConfigureAzureKeyVault();

builder.Services.AddAzureServices();

builder.Services.AddAIServices();

builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

app.MapHealthChecks("/health", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapApi();

app.Run();