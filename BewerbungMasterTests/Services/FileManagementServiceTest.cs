using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Moq;
using System;
using System.IO;
using BewerbungMasterApp.Services;

namespace BewerbungMasterApp.Tests.Services
{
    // Constructor Tests
    public class FileManagementServiceTests
    {
        [Fact]
        public void Constructor_ValidConfiguration_ShouldInitializePathsCorrectly()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            var environmentMock = new Mock<IWebHostEnvironment>();

            configurationMock.Setup(c => c["UserDirectoryPath"]).Returns("Users");
            environmentMock.Setup(e => e.WebRootPath).Returns(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));

            // Act
            var service = new FileManagementService(configurationMock.Object, environmentMock.Object);

            // Assert
            Assert.Equal(Path.Combine(environmentMock.Object.WebRootPath, "JobAppDocs"), service.JobAppDocsPath);
            Assert.Equal(Path.Combine(environmentMock.Object.WebRootPath, "Users"), service.UserDirectoryPath);
        }

        [Fact]
        public void Constructor_MissingUserDirectoryPath_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            var environmentMock = new Mock<IWebHostEnvironment>();

            configurationMock.Setup(c => c["UserDirectoryPath"]).Returns(string.Empty);
            environmentMock.Setup(e => e.WebRootPath).Returns(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => new FileManagementService(configurationMock.Object, environmentMock.Object));
            Assert.Equal("User directory path cannot be null or empty.", exception.Message);
        }

        [Fact]
        public void Constructor_NullConfiguration_ShouldThrowArgumentNullException()
        {
            // Arrange
            var environmentMock = new Mock<IWebHostEnvironment>();

            environmentMock.Setup(e => e.WebRootPath).Returns(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new FileManagementService(null, environmentMock.Object));
        }

        [Fact]
        public void Constructor_NullEnvironmentPath_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            var environmentMock = new Mock<IWebHostEnvironment>();

            configurationMock.Setup(c => c["UserDirectoryPath"]).Returns("Users");
            environmentMock.Setup(e => e.WebRootPath).Returns((string)null);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => new FileManagementService(configurationMock.Object, environmentMock.Object));

            Assert.Equal("Web root path cannot be null or empty.", exception.Message);
        }

    }
}
