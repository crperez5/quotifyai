namespace EmbedFunctions.Extensions;

internal static class KeyVaultConfigurationBuilderExtensions
{
    internal static IConfigurationBuilder ConfigureAzureKeyVault(this IConfigurationBuilder builder)
    {
        var azureKeyVaultEndpoint = Environment.GetEnvironmentVariable("AZURE_KEY_VAULT_ENDPOINT") ?? throw new InvalidOperationException("Azure Key Vault endpoint is not set.");
        ArgumentException.ThrowIfNullOrEmpty(azureKeyVaultEndpoint);

        builder.AddAzureKeyVault(new Uri(azureKeyVaultEndpoint), new DefaultAzureCredential());

        return builder;
    }
}