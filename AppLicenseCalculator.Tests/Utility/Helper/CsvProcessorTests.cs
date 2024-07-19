using AppLicenseCalculator.Models;
using AppLicenseCalculator.Tests.Model;
using AppLicenseCalculator.Utility.Helper;
using CsvHelper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace AppLicenseCalculator.Tests.Utility.Helper;

[TestFixture]
public class CsvProcessorTests
{
    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<CsvProcessor>>();
        _csvProcessor = new CsvProcessor(_loggerMock.Object);
    }

    private Mock<ILogger<CsvProcessor>> _loggerMock;
    private CsvProcessor _csvProcessor;

    /// <summary>
    /// Test to check exception whether exception is thrown or not when required object is passed as null.
    /// </summary>
    [Test]
    public void Constructor_LoggerIsNull_ThrowsArgumentNullException()
    {
        //Act and Assert
        Assert.Throws<ArgumentNullException>(() => new CsvProcessor(null));
    }

    /// <summary>
    /// Test to check exception when Header is not provided in CSV file.
    /// </summary>
    /// <returns>Should return exception</returns>
    [Test]
    public async Task LoadFileWithoutHeader_ShouldThrowHeaderException()
    {
        //Arrange
        var testFilePath = "test.csv";
        var records = new List<InstallationDetails>
        {
            new()
            {
                ComputerId = 1, UserId = 1, ApplicationId = 374, ComputerType = "DESKTOP",
                Comment = "Exported from System A"
            },
            new()
            {
                ComputerId = 2, UserId = 2, ApplicationId = 374, ComputerType = "DESKTOP",
                Comment = "Exported from System A"
            }, // Duplicate
            new()
            {
                ComputerId = 2, UserId = 2, ApplicationId = 374, ComputerType = "desktop",
                Comment = "Exported from System B"
            }
        };

        // Pass false not to add header to CSV. Default value is true.
        CreateTestCsvFile(testFilePath, records, false);

        try
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<HeaderValidationException>(() => _csvProcessor.LoadFileWithoutDuplicatesAsync(testFilePath));
            Assert.That(ex, Is.TypeOf<HeaderValidationException>());
        }
        finally
        {
            // Clean up
            if (File.Exists(testFilePath))
            {
                File.Delete(testFilePath);
            }
        }
    }

    /// <summary>
    /// Test to check whether duplicate records or removed or not.
    /// </summary>
    /// <returns>Should remove duplicates and return List of objects</returns>
    [Test]
    public async Task LoadFileWithoutDuplicatesAsync_ValidInput_ShouldRemoveDuplicates()
    {
        //Arrange
        var testFilePath = "test.csv";
        var records = new List<InstallationDetails>
        {
            new()
            {
                ComputerId = 1, UserId = 1, ApplicationId = 374, ComputerType = "DESKTOP",
                Comment = "Exported from System A"
            },
            new()
            {
                ComputerId = 2, UserId = 2, ApplicationId = 374, ComputerType = "DESKTOP",
                Comment = "Exported from System A"
            }, // Duplicate
            new()
            {
                ComputerId = 2, UserId = 2, ApplicationId = 374, ComputerType = "desktop",
                Comment = "Exported from System B"
            }
        };

        CreateTestCsvFile(testFilePath, records);

        //Act
        var result = await _csvProcessor.LoadFileWithoutDuplicatesAsync(testFilePath);

        //Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].ComputerId, Is.EqualTo(1));
        Assert.That(result[1].ComputerId, Is.EqualTo(2));

        // Clean up
        File.Delete(testFilePath);
    }

    /// <summary>
    /// Test to check whether all records are added to list of objects or not ( Valid records in this scenario is used)
    /// </summary>
    /// <returns>Should return List of objects. All input records should be added</returns>
    [Test]
    public async Task LoadFileWithoutDuplicatesAsync_ValidInput_NoDuplicatesRemoval()
    {
        //Arrange
        var testFilePath = "test.csv";
        var records = new List<InstallationDetails>
        {
            new()
            {
                ComputerId = 1, UserId = 1, ApplicationId = 374, ComputerType = "DESKTOP",
                Comment = "Exported from System B"
            },
            new()
            {
                ComputerId = 2, UserId = 2, ApplicationId = 374, ComputerType = "DESKTOP",
                Comment = "Exported from System A"
            },
            new()
            {
                ComputerId = 3, UserId = 2, ApplicationId = 374, ComputerType = "desktop",
                Comment = "Exported from System B"
            }
        };

        CreateTestCsvFile(testFilePath, records);

        //Act
        var result = await _csvProcessor.LoadFileWithoutDuplicatesAsync(testFilePath);

        //Assert
        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result[0].ComputerId, Is.EqualTo(1));
        Assert.That(result[1].ComputerId, Is.EqualTo(2));
        Assert.That(result[2].ComputerId, Is.EqualTo(3));

        // Clean up
        File.Delete(testFilePath); 
    }

    /// <summary>
    /// Test to check whether all records are added to list of objects or not ( Valid records in this scenario is used)
    /// </summary>
    /// <returns>Should return List of objects. All input records should be added</returns>
    [Test]
    public async Task LoadFileWithoutDuplicatesAsync_ValidInput_LaptopDesktopCombination()
    {
        //Arrange
        var testFilePath = "test.csv";
        var records = new List<InstallationDetails>
        {
            new()
            {
                ComputerId = 1, UserId = 1, ApplicationId = 374, ComputerType = "DESKTOP",
                Comment = "Exported from System B"
            },
            new()
            {
                ComputerId = 2, UserId = 2, ApplicationId = 374, ComputerType = "DESKTOP",
                Comment = "Exported from System A"
            },
            new()
            {
                ComputerId = 3, UserId = 2, ApplicationId = 374, ComputerType = "Laptop",
                Comment = "Exported from System B"
            }
        };

        CreateTestCsvFile(testFilePath, records);

        //Act
        var result = await _csvProcessor.LoadFileWithoutDuplicatesAsync(testFilePath);

        //Assert
        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result[0].ComputerId, Is.EqualTo(1));
        Assert.That(result[1].ComputerId, Is.EqualTo(2));
        Assert.That(result[2].ComputerId, Is.EqualTo(3));

        // Clean up
        File.Delete(testFilePath);
    }

    /// <summary>
    /// Test to check whether all records are added to list of objects or not ( Valid records different computer type is used.)
    /// </summary>
    /// <returns>Should return List of objects. All input records should be added</returns>
    [Test]
    public async Task LoadFileWithoutDuplicatesAsync_ValidInput_DifferentComputerType_NoRemoval()
    {
        //Arrange
        var testFilePath = "test.csv";
        var records = new List<InstallationDetails>
        {
            new()
            {
                ComputerId = 1, UserId = 2, ApplicationId = 374, ComputerType = "DESKTOP",
                Comment = "Exported from System B"
            },
            new()
            {
                ComputerId = 2, UserId = 2, ApplicationId = 374, ComputerType = "IPAD",
                Comment = "Exported from System A"
            },
            new()
            {
                ComputerId = 3, UserId = 2, ApplicationId = 374, ComputerType = "Tablet",
                Comment = "Exported from System B"
            }
        };

        CreateTestCsvFile(testFilePath, records);

        //Act
        var result = await _csvProcessor.LoadFileWithoutDuplicatesAsync(testFilePath);

        //Assert
        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result[0].ComputerId, Is.EqualTo(1));
        Assert.That(result[1].ComputerId, Is.EqualTo(2));
        Assert.That(result[2].ComputerId, Is.EqualTo(3));

        File.Delete(testFilePath); // Clean up
    }

    /// <summary>
    /// Test to check whether Argument Exception is thrown or not when file path is null.
    /// </summary>
    [Test]
    public void LoadFileWithoutDuplicatesAsync_NullFilePath_ArgumentException()
    {
        //Arrange
        string invalidFilePath = null;

        //Act and Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _csvProcessor.LoadFileWithoutDuplicatesAsync(invalidFilePath));
        Assert.That(ex, Is.TypeOf<ArgumentException>());
    }

    /// <summary>
    /// Test to check whether FileNotFoundException is thrown or not when invalid file path is given.
    /// </summary>
    [Test]
    public void LoadFileWithoutDuplicatesAsync_InvalidFilePath_FileNotFoundException()
    {
        //Arrange
        var invalidFilePath = "invalidfile.csv";

        //Act and Assert
        var ex = Assert.ThrowsAsync<FileNotFoundException>(async () =>
            await _csvProcessor.LoadFileWithoutDuplicatesAsync(invalidFilePath));
        Assert.That(ex, Is.TypeOf<FileNotFoundException>());
    }

    /// <summary>
    ///  Create csv file from objects used for unit testing
    /// </summary>
    /// <param name="filePath">file path </param>
    /// <param name="records"> list of InstallationDetails</param>
    /// <param name="writeHeader">Whether to write header or not to csv file</param>
    private void CreateTestCsvFile(string filePath, List<InstallationDetails> records, bool writeHeader = true)
    {
        using var writer = new StreamWriter(filePath);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        if(writeHeader)
            csv.WriteHeader<CsvHeader>();
        csv.NextRecord();
        csv.WriteRecords(records);
    }
}