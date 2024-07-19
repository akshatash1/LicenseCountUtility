using AppLicenseCalculator.Models;
using AppLicenseCalculator.Utility.Interface;
using CsvHelper;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace AppLicenseCalculator.Utility.Helper
{
    public class CsvProcessor : ICsvProcessor
    {
        private readonly ILogger<CsvProcessor> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvProcessor"/> class.
        /// </summary>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CsvProcessor(ILogger<CsvProcessor> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Removes duplicates from the CSV file based on ComputerId, UserID, ApplicationId, and ComputerType.
        /// </summary>
        /// <param name="filePath">The input CSV file path.</param>
        public async Task<List<InstallationDetails>> LoadFileWithoutDuplicatesAsync(string filePath)
        {
            try
            {
                //check whether file path and file exists or not.
                ValidateFilePath(filePath);

                //using CSVHelper, loads the unique data into object
                var uniqueKeys = new HashSet<string>();
                var records = new List<InstallationDetails>();

                using var reader = new StreamReader(filePath);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                csv.Context.RegisterClassMap<InstallationDetailsMap>();

                await foreach (var record in csv.GetRecordsAsync<InstallationDetails>())
                {
                    var key = $"{record.ComputerId},{record.UserId},{record.ApplicationId},{record.ComputerType?.ToUpper()}";

                    // Check for duplicates based on a composite key
                    if (uniqueKeys.Add(key))
                    {
                        records.Add(record);
                    }
                }

                return records;
            }
            catch (HeaderValidationException ex)
            {
                _logger.LogError(ex, $"{filePath} doesn't have valid header. {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{filePath}_An error occurred while removing duplicates from the CSV file.");
                throw;
            }
        }

        /// <summary>
        /// Validates file path is valid or not
        /// </summary>
        /// <param name="filePath">file path</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        private void ValidateFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found at '{filePath}'.", filePath);
            }
        }
    }
}
