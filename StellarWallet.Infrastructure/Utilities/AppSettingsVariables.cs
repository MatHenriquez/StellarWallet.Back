using Microsoft.Extensions.Configuration;

namespace StellarWallet.Infrastructure.Utilities;

public static class AppSettingsVariables
{
    public static IConfigurationRoot BuildConfig(string? environment)
    {
        var settedEnvironment = SetEnvironment(environment);

        string? directory;

        if (settedEnvironment == "test")
            directory = Path.Combine("..", "..", "..", "..", "StellarWallet.WebApi");
        else
            directory = Directory.GetCurrentDirectory();

        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Path.GetFullPath(directory));

        if (File.Exists($"appsettings.{settedEnvironment}.json"))
            configurationBuilder.AddJsonFile($"appsettings.{settedEnvironment}.json", optional: false);
        else
            throw new FileNotFoundException("Configuration file not found");

        return configurationBuilder.Build();
    }

    public static string GetConnectionString(string? environment)
    {
        return BuildConfig(SetEnvironment(environment)).GetConnectionString("StellarWallet") ?? throw new InvalidOperationException("Undefined connection string");
    }

    public static string GetSettingVariable(string sectionName, string variableName, string? environment)
    {
        return BuildConfig(SetEnvironment(environment)).GetSection(sectionName).GetValue<string>(variableName) ?? throw new InvalidOperationException("Undefined variable");
    }

    private static readonly Func<string?, string> SetEnvironment = (string? environment) =>
    {
        if (string.IsNullOrEmpty(environment))
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "test";
        else
            return environment;
    };
}
