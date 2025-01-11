var builder = WebApplication.CreateBuilder(args);

builder.Configuration.ConfigureAzureKeyVault();

builder.Services.ConfigureHealthChecks();

builder.Services
    .AddAzureServices()
    .AddAIServices()
    .AddApplicationServices(builder.Configuration)
    .AddApplicationInsightsTelemetry()
    .AddOpenApi();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // file size limit is 10 MB
});

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN-HEADER";
    options.FormFieldName = "X-CSRF-TOKEN-FORM";
});

var app = builder.Build();

app.UseAntiforgery();

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