var builder = WebApplication.CreateBuilder(args);
var appConfig = new AppConfig();
builder.Configuration.GetSection(AppConfig.ConfigSectionName).Bind(appConfig);
builder.Services.Configure<AppConfig>(builder.Configuration.GetSection(AppConfig.ConfigSectionName));

if (appConfig.UseKeyVault)
{
    builder.Configuration.ConfigureAzureKeyVault();
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyMethod()  
            .AllowAnyHeader()   
            .AllowCredentials()  
            .WithExposedHeaders("Upgrade", "Connection"); 
    });
});

builder.Services.ConfigureHealthChecks();

builder.Services
    .AddLogging(configure => configure.AddConsole())
    .AddAzureServices()
    .AddAIServices(builder.Configuration)
    .AddApplicationInsightsTelemetry()
    .AddCustomSignalR()
    .AddOpenApi();

await builder.Services.AddApplicationServicesAsync(builder.Configuration);

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

app.UseStaticFiles();

app.UseCors("AllowAll");

app.MapHub<ChatHub>("/chatHub");

app.MapApi();

app.Run();