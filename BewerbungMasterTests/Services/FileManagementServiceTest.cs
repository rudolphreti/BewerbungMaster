using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Moq;
using System;
using System.IO;
using BewerbungMasterApp.Services;

namespace BewerbungMasterTests.Services
{
    // Unit tests for the FileManagementService constructor
    public class FileManagementServiceTests
    {
        [Fact]
        public void Constructor_ValidConfiguration_ShouldInitializePathsCorrectly()
        {
            // Arrange: Set up mocks for IConfiguration and IWebHostEnvironment
            var configurationMock = new Mock<IConfiguration>();
            var environmentMock = new Mock<IWebHostEnvironment>();

            // Mock the configuration to return "Users" for the "UserDirectoryPath" key
            configurationMock.Setup(c => c["UserDirectoryPath"]).Returns("Users");
            // Mock the environment to return a path combining current directory and "wwwroot"
            environmentMock.Setup(e => e.WebRootPath).Returns(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));

            // Act: Create an instance of FileManagementService using the mocks
            var service = new FileManagementService(configurationMock.Object, environmentMock.Object);

            // Assert: Check that the paths are initialized correctly
            Assert.Equal(Path.Combine(environmentMock.Object.WebRootPath, "JobAppDocs"), service.JobAppDocsPath);
            Assert.Equal(Path.Combine(environmentMock.Object.WebRootPath, "Users"), service.UserDirectoryPath);
        }

        [Fact]
        public void Constructor_MissingUserDirectoryPath_ShouldThrowInvalidOperationException()
        {
            // Arrange: Set up mocks with a missing user directory path
            var configurationMock = new Mock<IConfiguration>();
            var environmentMock = new Mock<IWebHostEnvironment>();

            // Mock the configuration to return an empty string for "UserDirectoryPath"
            configurationMock.Setup(c => c["UserDirectoryPath"]).Returns(string.Empty);
            // Mock the environment to return a valid web root path
            environmentMock.Setup(e => e.WebRootPath).Returns(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));

            // Act & Assert: Verify that an InvalidOperationException is thrown due to the empty "UserDirectoryPath"
            var exception = Assert.Throws<InvalidOperationException>(() => new FileManagementService(configurationMock.Object, environmentMock.Object));
            Assert.Equal("User directory path cannot be null or empty.", exception.Message);
        }

        [Fact]
        public void Constructor_NullConfiguration_ShouldThrowArgumentNullException()
        {
            // Arrange: Set up a mock for IWebHostEnvironment
            var environmentMock = new Mock<IWebHostEnvironment>();

            // Mock the environment to return a valid web root path
            environmentMock.Setup(e => e.WebRootPath).Returns(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));

            // Act & Assert: Verify that an ArgumentNullException is thrown when IConfiguration is null
            Assert.Throws<ArgumentNullException>(() => new FileManagementService(null, environmentMock.Object));
        }

        [Fact]
        public void Constructor_NullEnvironmentPath_ShouldThrowInvalidOperationException()
        {
            // Arrange: Set up mocks for IConfiguration and IWebHostEnvironment with a null environment path
            var configurationMock = new Mock<IConfiguration>();
            var environmentMock = new Mock<IWebHostEnvironment>();

            // Mock the configuration to return "Users" for "UserDirectoryPath"
            configurationMock.Setup(c => c["UserDirectoryPath"]).Returns("Users");
            // Mock the environment to return null for the web root path
            environmentMock.Setup(e => e.WebRootPath).Returns((string)null);

            // Act & Assert: Verify that an InvalidOperationException is thrown due to the null web root path
            var exception = Assert.Throws<InvalidOperationException>(() => new FileManagementService(configurationMock.Object, environmentMock.Object));

            Assert.Equal("Web root path cannot be null or empty.", exception.Message);
        }
    }
}
