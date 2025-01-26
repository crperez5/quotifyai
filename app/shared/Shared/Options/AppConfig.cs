namespace MinimalApi.Options;

public class AppConfig
{
    public const string ConfigSectionName = "AppConfig";

    public bool UseKeyVault { get; set; } = false;
}