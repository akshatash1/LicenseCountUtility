using AppLicenseCalculator.Utility.Helper;
using AppLicenseCalculator.Utility.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AppLicenseCalculator.App;

public class Program
{
    static async Task Main(string[] args)
    {
        var services = CreateServices();

        string filePath;
        // read from command line arguments. If argument is not passed, read from console.
        // Ensure a filename is provided
        if (args.Length < 1)
        {
            Console.WriteLine("Please enter a valid path to the CSV file :");
            filePath = Console.ReadLine() ?? throw new InvalidOperationException("Invalid file path");
        }
        else
        {
            // Read the filename from command-line arguments
            filePath = args[0];
        }

        // Check if the file has a .csv extension else exit.
        if (Path.GetExtension(filePath).ToLower() != ".csv")
        {
            Console.WriteLine("The utility only supports CSV format. Please rerun the utility with a CSV file.");
            return;
        }

        // Resolve ApplicationRunner from the service provider
        var runner = services.GetRequiredService<ApplicationRunner>();

        // Run the main application logic
        await runner.RunAsync(filePath);

        // Dispose the service provider to release resources else it will not release
        if (services is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    /// <summary>
    /// Build and return the service provider. Used for adding dependency injection
    /// </summary>
    /// <returns>the service provider</returns>
    private static ServiceProvider CreateServices()
    {
        // Create a new instance of ServiceCollection to register services
        var services = new ServiceCollection();

        // Configure logging services
        services.AddLogging(opt =>
        {
            // Clear any existing logging providers
            opt.ClearProviders();

            // Add the console logging provider
            opt.AddConsole();
        });

        // Register CsvProcessor as a scoped service with the ICsvProcessor interface
        services.AddScoped<ICsvProcessor, CsvProcessor>();

        // Register CopyCalculator as a scoped service with the ILicenseCalculator interface
        services.AddScoped<ILicenseCalculator, LicenseCalculator>();

        // Register ApplicationRunner as a scoped service
        services.AddScoped<ApplicationRunner>();

        // Build and return the service provider
        return services.BuildServiceProvider();
    }

}