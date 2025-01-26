using EmbedFunctions;

var builder = new HostBuilder();
builder
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(builder => builder.ConfigureAzureKeyVault())
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddAIServices(configuration);

        services.AddSingleton<IDataLoader, DataLoader>();
    });

var host = builder.Build();

host.Run();