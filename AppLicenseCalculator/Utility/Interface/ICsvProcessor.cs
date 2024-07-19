using AppLicenseCalculator.Models;

namespace AppLicenseCalculator.Utility.Interface
{
    public interface ICsvProcessor
    {
        /// <summary>
        /// Loads installations from the specified CSV file by removing duplicates from the input file based on ComputerId, UserID, ApplicationId, and ComputerType.
        /// </summary>
        /// <param name="filePath">The path to the CSV file.</param>
        /// <returns>A list of InstallationDetails.</returns>
        Task<List<InstallationDetails>> LoadFileWithoutDuplicatesAsync(string filePath);
    }
}
