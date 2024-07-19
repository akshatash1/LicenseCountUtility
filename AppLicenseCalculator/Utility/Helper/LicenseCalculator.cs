using AppLicenseCalculator.Config;
using AppLicenseCalculator.Models;
using AppLicenseCalculator.Utility.Interface;
using Microsoft.Extensions.Logging;

namespace AppLicenseCalculator.Utility.Helper;

/// <summary>
/// Service to calculate the minimum number of application copies(license) required based on installation data.
/// </summary>
public class LicenseCalculator : ILicenseCalculator
{
    private readonly ILogger<LicenseCalculator> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LicenseCalculator"/> class.
    /// </summary>
    /// <param name="logger">The logger instance to use for logging.</param>
    public LicenseCalculator(ILogger<LicenseCalculator> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Calculates the minimum number of licenses required for the specified application.
    /// </summary>
    /// <param name="appInstallDetails">List of Application Installation details</param>
    /// <returns>The minimum number of licenses required.</returns>
    public async Task<int> CalculateMinimumLicensesAsync(List<InstallationDetails> appInstallDetails)
    {
        _logger.LogInformation("Starting calculation of minimum licenses.");

        if (appInstallDetails.Count > 0)
        {
            try
            {
                // Offload CPU-bound work to a background task
                return await Task.Run(() =>
                {
                    // Group installations by UserId and filter by TargetApplicationID
                    var userInstallations = appInstallDetails
                        .Where(i => i.ApplicationId == Configurations.TargetApplication)
                        .GroupBy(i => i.UserId)
                        .Select(g => new { UserID = g.Key, Installations = g.ToList() });

                    var totalLicenses = 0;

                    foreach (var userInstallation in userInstallations)
                    {
                        //get number of laptops installations for a user
                        var laptopCount = userInstallation.Installations.Count(i =>
                            i.ComputerType != null && i.ComputerType.ToUpper() == "LAPTOP");
                        //get number of desktop installations for a user
                        var desktopCount = userInstallation.Installations.Count(i =>
                            i.ComputerType != null && i.ComputerType.ToUpper() == "DESKTOP");

                        switch (laptopCount)
                        {
                            case > 0 when desktopCount > 0:
                                // If there are laptops, each laptop can cover one desktop. If there are 2 desktops and 1 laptop, max copy required is 2
                                totalLicenses += Math.Max(laptopCount, desktopCount);
                                break;
                            case > 0:
                                // If there are no desktops, count all laptops.
                                totalLicenses += laptopCount;
                                break;
                            default:
                                // If there are no laptops, count all desktops.
                                totalLicenses += desktopCount;
                                break;
                        }
                    }

                    _logger.LogInformation($"Calculation completed. Minimum licenses required: {totalLicenses}");
                    return totalLicenses;
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing minimum licenses.");
                throw;
            }
        }
        else
        {
            _logger.LogInformation("No records found.");
            return 0;
        }
    }
}