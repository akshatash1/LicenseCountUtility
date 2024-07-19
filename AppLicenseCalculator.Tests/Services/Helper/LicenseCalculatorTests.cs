using AppLicenseCalculator.Models;
using AppLicenseCalculator.Utility.Helper;
using AppLicenseCalculator.Utility.Interface;
using Moq;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppLicenseCalculator.Tests.Services.Helper
{
    public class LicenseCalculatorTests
    {
        private Mock<ILogger<LicenseCalculator>> _loggerMock;
        private ILicenseCalculator _licenseCalculator;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<LicenseCalculator>>();
            _licenseCalculator = new LicenseCalculator(_loggerMock.Object);
        }

        /// <summary>
        /// Test to check exception whether exception is thrown or not when required object is passed as null.
        /// </summary>
        [Test]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => new LicenseCalculator(null));
        }

        /// <summary>
        /// Test to check license count when user has only Laptop as computer type. 
        /// </summary>
        /// <returns>Should return 1.</returns>
        [Test]
        public async Task CalculateMinimumLicensesAsync_WithSingleLaptop_ShouldReturnOneLicense()
        {
            // Arrange
            var installationDetails = new List<InstallationDetails>
            {
             new() { ComputerId = 1, UserId = 1, ApplicationId = 374, ComputerType = "Laptop", Comment = "Exported from System A"},
            };

            // Act
            var result = await _licenseCalculator.CalculateMinimumLicensesAsync(installationDetails);

            // Assert
            Assert.That(result, Is.EqualTo(1));
        }

        /// <summary>
        /// Test to check license count when user has both desktop and Laptop as computer type. 
        /// </summary>
        /// <returns>Should return 2.</returns>
        [Test]
        public async Task CalculateMinimumLicensesAsync_WithMultipleLaptopsAndDesktops_ShouldReturnCorrectLicenseCount()
        {
            // List is returned post removing duplicates
            var installationDetails = new List<InstallationDetails>
            {
                new() { ComputerId = 1, UserId = 1, ApplicationId = 374, ComputerType = "DESKTOP", Comment = "Exported from System A"},
                new() { ComputerId = 3, UserId = 1, ApplicationId = 374, ComputerType = "Laptop", Comment = "Exported from System A"},
                new() { ComputerId = 2, UserId = 2, ApplicationId = 374, ComputerType = "desktop", Comment = "Exported from System B"}
            };

            // Act
            var result = await _licenseCalculator.CalculateMinimumLicensesAsync(installationDetails);

            // Assert
            Assert.That(result, Is.EqualTo(2)); // 1 for user1 (1 desktop and 1 laptop) and 1 for user2 (1 desktop) 
        }

        /// <summary>
        /// Test to check license count when user has both desktop and Laptop as computer type.
        /// </summary>
        /// <returns> Should return 3</returns>
        [Test]
        public async Task CalculateMinimumLicensesAsync_WithMixedInstallations_ShouldReturnCorrectLicenseCount()
        {
            // Arrange
            var installationDetails = new List<InstallationDetails>
            {   new() { ComputerId = 1, UserId = 1, ApplicationId = 374, ComputerType = "Laptop", Comment = "Exported from System A"},
                new() { ComputerId = 2, UserId = 1, ApplicationId = 374, ComputerType = "DESKTOP", Comment = "Exported from System A"},
                new() { ComputerId = 4, UserId = 2, ApplicationId = 374, ComputerType = "DESKTOP", Comment = "Exported from System A"},
                new() { ComputerId = 3, UserId = 2, ApplicationId = 374, ComputerType = "desktop", Comment = "Exported from System B"}
            };

            // Act
            var result = await _licenseCalculator.CalculateMinimumLicensesAsync(installationDetails);

            // Assert
            Assert.That(result, Is.EqualTo(3)); // 1 for user1 (1 laptop + 1 desktop), 2 for user2 (2 desktops)
        }

        /// <summary>
        ///  Test to check license count when user has computer type laptop and desktop but different application.
        /// </summary>
        /// <returns> Should return 0</returns>
        [Test]
        public async Task CalculateMinimumLicensesAsync_WithNoTargetApplication_ShouldReturnZero()
        {
            // Arrange
            var installationDetails = new List<InstallationDetails>
            {
                new() { ComputerId = 1, UserId = 1, ApplicationId = 379, ComputerType = "Laptop", Comment = "Exported from System A"},
                new() { ComputerId = 2, UserId = 1, ApplicationId = 560, ComputerType = "DESKTOP", Comment = "Exported from System A"},
            };

            // Act
            var result = await _licenseCalculator.CalculateMinimumLicensesAsync(installationDetails);

            // Assert
            Assert.That(result, Is.EqualTo(0)); // No installations for TargetApp
        }

        /// <summary>
        /// Test to check license count when user has different computer type other than laptop and desktop. 
        /// </summary>
        /// <returns>Should return 0</returns>
        [Test]
        public async Task CalculateMinimumLicensesAsync_WithNoTargetComputerType_ShouldReturnZero()
        {
            // Arrange
            var installationDetails = new List<InstallationDetails>
            {
                new() { ComputerId = 1, UserId = 1, ApplicationId = 374, ComputerType = "IPAD", Comment = "Exported from System A"},
                new() { ComputerId = 2, UserId = 1, ApplicationId = 374, ComputerType = "Tablet", Comment = "Exported from System A"},
            };

            // Act
            var result = await _licenseCalculator.CalculateMinimumLicensesAsync(installationDetails);

            // Assert
            Assert.That(result, Is.EqualTo(0)); // No installations for TargetApp
        }

        /// <summary>
        /// Test to check license count when no records found in csv file.
        /// </summary>
        /// <returns>Should return 0</returns>
        [Test]
        public async Task CalculateMinimumLicensesAsync_WithNoRecords()
        {
            // Arrange
            var installationDetails = new List<InstallationDetails>();
            
            // Act
            var result = await _licenseCalculator.CalculateMinimumLicensesAsync(installationDetails);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        /// <summary>
        /// Test to check license count when only desktop records found in csv file.
        /// </summary>
        /// <returns>Should return 2.</returns>
        [Test]
        public async Task CalculateMinimumLicensesAsync_OnlyDesktopRecords()
        {
            // Arrange
            var installationDetails = new List<InstallationDetails>
            {
                new() { ComputerId = 1, UserId = 1, ApplicationId = 374, ComputerType = "Desktop", Comment = "Exported from System A"},
                new() { ComputerId = 2, UserId = 2, ApplicationId = 374, ComputerType = "dEsktoP", Comment = "Exported from System A"},
            };

            // Act
            var result = await _licenseCalculator.CalculateMinimumLicensesAsync(installationDetails);

            // Assert
            Assert.That(result, Is.EqualTo(2));
        }
    }
}