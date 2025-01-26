namespace MinimalApi.Extensions;

internal static class ConfigurationBuilderExtensions
{
    internal static IConfigurationBuilder ConfigureAzureKeyVault(this IConfigurationBuilder builder)
    {
        var azureKeyVaultEndpoint = Environment.GetEnvironmentVariable("AZURE_KEY_VAULT_ENDPOINT") ?? throw new InvalidOperationException("Azure Key Vault endpoint is not set.");
        var secretClient = new SecretClient(new Uri(azureKeyVaultEndpoint), new DefaultAzureCredential());
        builder.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
        return builder;
    }
}
