using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHealthChecks(builder.Configuration);

builder.Configuration.ConfigureAzureKeyVault();

builder.Services.AddAzureServices();

builder.Services.AddAIServices();

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapHealthChecks("/health", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(s => 
    {
        s.Title = "QuotifyAI Backend API";
    });
}

app.MapApi();

app.Run();