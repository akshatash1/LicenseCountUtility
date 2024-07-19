using AppLicenseCalculator.Config;
using AppLicenseCalculator.Utility.Interface;
using Microsoft.Extensions.Logging;

namespace AppLicenseCalculator.App
{
    /// <summary>
    /// Runs the application logic for calculating minimum application copy required for CSV based input.
    /// </summary>
    public class ApplicationRunner
    {
        private readonly ICsvProcessor _csvProcessor;
        private readonly ILicenseCalculator _licenseCalculator;
        private readonly ILogger<ApplicationRunner> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationRunner"/> class.
        /// </summary>
        /// <param name="csvProcessor">CSV processing service.</param>
        /// <param name="licenseCalculator">License calculation service.</param>
        /// <param name="logger">Logger for logging information.</param>
        public ApplicationRunner(ICsvProcessor csvProcessor, ILicenseCalculator licenseCalculator, ILogger<ApplicationRunner> logger)
        {
            _csvProcessor = csvProcessor ?? throw new ArgumentNullException(nameof(csvProcessor));
            _licenseCalculator = licenseCalculator ?? throw new ArgumentNullException(nameof(licenseCalculator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Runs the application asynchronously.
        /// </summary>
        /// <param name="filePath">input file path to process.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RunAsync(string filePath)
        {
            try
            {
                _logger.LogInformation("Application started.");

                // Remove duplicates from file asynchronously.
                // loading of data and removing of duplicates is done while loading the data from csv itself to avoid processing overhead with
                // huge files
                var appInstallDetails = await _csvProcessor.LoadFileWithoutDuplicatesAsync(filePath);

                // Calculate number of application licenses required

                var minAppCopy = await _licenseCalculator.CalculateMinimumLicensesAsync(appInstallDetails);

                // Print the output
                Console.WriteLine(
                    $"The minimum number of application copies required for the application {Configurations.TargetApplication} for the organisation is {minAppCopy}.");

                _logger.LogInformation("Application finished.");
                _logger.LogInformation(
                    $"The minimum number of application copies required for the application {Configurations.TargetApplication} for the organisation is {minAppCopy}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in RunAsync function while processing data");
            }
        }
    }
}
