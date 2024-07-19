using AppLicenseCalculator.Models;

namespace AppLicenseCalculator.Utility.Interface
{
    public interface ILicenseCalculator
    {
        /// <summary>
        /// Calculates the minimum number of licenses required for the specified application.
        /// </summary>
        /// <param name="appInstallationDetails">The list of application installation details.</param>
        /// <returns>The minimum number of licenses required.</returns>
        public Task<int> CalculateMinimumLicensesAsync(List<InstallationDetails> appInstallationDetails);
    }
}
