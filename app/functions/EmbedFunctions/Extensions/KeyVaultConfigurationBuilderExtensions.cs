namespace EmbedFunctions.Extensions;

internal static class KeyVaultConfigurationBuilderExtensions
{
    internal static IConfigurationBuilder ConfigureAzureKeyVault(this IConfigurationBuilder builder)
    {
        var useKeyVaultEnvVariable = Environment.GetEnvironmentVariable("UseKeyVault");
        if (bool.TryParse(useKeyVaultEnvVariable, out var useKeyVaultSecret) && useKeyVaultSecret)
        {
            var azureKeyVaultEndpoint = Environment.GetEnvironmentVariable("AZURE_KEY_VAULT_ENDPOINT") ?? throw new InvalidOperationException("Azure Key Vault endpoint is not set.");
            ArgumentException.ThrowIfNullOrEmpty(azureKeyVaultEndpoint);

            builder.AddAzureKeyVault(new Uri(azureKeyVaultEndpoint), new DefaultAzureCredential());
        }
        return builder;
    }
}