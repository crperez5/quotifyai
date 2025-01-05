using EmbedFunctions;

var builder = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(builder => builder.ConfigureAzureKeyVault())
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddAIServices();

        services.AddSingleton<IDataLoader, DataLoader>();
    });

var host = builder.Build();

host.Run();