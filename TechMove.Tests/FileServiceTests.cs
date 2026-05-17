using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using TechMove.Web.Services;

namespace TechMove.Tests
{
    public class FileServiceTests
    {
        private IFormFile CreateMockFile(string fileName, string contentType)
        {
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(1024);
            mockFile.Setup(f => f.ContentType).Returns(contentType);
            return mockFile.Object;
        }

        [Fact]
        public void IsValidPdf_WithPdfFile_ReturnsTrue()
        {
            // Arrange
            var fileService = new FileService(null!);
            var mockFile = CreateMockFile("agreement.pdf", "application/pdf");

            // Act
            var result = fileService.IsValidPdf(mockFile);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidPdf_WithExeFile_ReturnsFalse()
        {
            // Arrange
            var fileService = new FileService(null!);
            var mockFile = CreateMockFile("malicious.exe", "application/octet-stream");

            // Act
            var result = fileService.IsValidPdf(mockFile);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidPdf_WithWordDoc_ReturnsFalse()
        {
            // Arrange
            var fileService = new FileService(null!);
            var mockFile = CreateMockFile("document.docx", "application/vnd.openxmlformats");

            // Act
            var result = fileService.IsValidPdf(mockFile);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidPdf_WithNullFile_ReturnsFalse()
        {
            // Arrange
            var fileService = new FileService(null!);

            // Act
            var result = fileService.IsValidPdf(null!);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidPdf_WithEmptyFile_ReturnsFalse()
        {
            // Arrange
            var fileService = new FileService(null!);
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("empty.pdf");
            mockFile.Setup(f => f.Length).Returns(0);

            // Act
            var result = fileService.IsValidPdf(mockFile.Object);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidPdf_WithUpperCasePdfExtension_ReturnsTrue()
        {
            // Arrange
            var fileService = new FileService(null!);
            var mockFile = CreateMockFile("AGREEMENT.PDF", "application/pdf");

            // Act
            var result = fileService.IsValidPdf(mockFile);

            // Assert
            result.Should().BeTrue();
        }
    }
}