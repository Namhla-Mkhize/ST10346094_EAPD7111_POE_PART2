using FluentAssertions;
using Moq;
using TechMove.Web.Models;
using TechMove.Web.Repositories;
using TechMove.Web.Services;

namespace TechMove.Tests
{
    public class ServiceRequestServiceTests
    {
        private readonly Mock<IServiceRequestRepository> _mockServiceRequestRepo;
        private readonly Mock<IContractRepository> _mockContractRepo;
        private readonly ServiceRequestService _service;

        public ServiceRequestServiceTests()
        {
            _mockServiceRequestRepo = new Mock<IServiceRequestRepository>();
            _mockContractRepo = new Mock<IContractRepository>();
            _service = new ServiceRequestService(_mockServiceRequestRepo.Object, _mockContractRepo.Object);
        }

        [Fact]
        public async Task CreateServiceRequest_ActiveContract_ReturnsSuccess()
        {
            // Arrange
            var contract = new Contract { Id = 1, Status = ContractStatus.Active };
            _mockContractRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contract);
            _mockServiceRequestRepo.Setup(r => r.AddAsync(It.IsAny<ServiceRequest>())).Returns(Task.CompletedTask);

            var serviceRequest = new ServiceRequest { ContractId = 1, CostUSD = 100 };

            // Act
            var (success, message) = await _service.CreateServiceRequestAsync(serviceRequest);

            // Assert
            success.Should().BeTrue();
            message.Should().Be("Service Request created successfully.");
        }

        [Fact]
        public async Task CreateServiceRequest_ExpiredContract_ReturnsFailure()
        {
            // Arrange
            var contract = new Contract { Id = 1, Status = ContractStatus.Expired };
            _mockContractRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contract);

            var serviceRequest = new ServiceRequest { ContractId = 1, CostUSD = 100 };

            // Act
            var (success, message) = await _service.CreateServiceRequestAsync(serviceRequest);

            // Assert
            success.Should().BeFalse();
            message.Should().Contain("Expired");
        }

        [Fact]
        public async Task CreateServiceRequest_OnHoldContract_ReturnsFailure()
        {
            // Arrange
            var contract = new Contract { Id = 1, Status = ContractStatus.OnHold };
            _mockContractRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contract);

            var serviceRequest = new ServiceRequest { ContractId = 1, CostUSD = 100 };

            // Act
            var (success, message) = await _service.CreateServiceRequestAsync(serviceRequest);

            // Assert
            success.Should().BeFalse();
            message.Should().Contain("On Hold");
        }

        [Fact]
        public async Task CreateServiceRequest_ContractNotFound_ReturnsFailure()
        {
            // Arrange
            _mockContractRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Contract?)null);

            var serviceRequest = new ServiceRequest { ContractId = 99, CostUSD = 100 };

            // Act
            var (success, message) = await _service.CreateServiceRequestAsync(serviceRequest);

            // Assert
            success.Should().BeFalse();
            message.Should().Contain("not found");
        }

        [Fact]
        public async Task CreateServiceRequest_DraftContract_ReturnsSuccess()
        {
            // Arrange
            var contract = new Contract { Id = 1, Status = ContractStatus.Draft };
            _mockContractRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contract);
            _mockServiceRequestRepo.Setup(r => r.AddAsync(It.IsAny<ServiceRequest>())).Returns(Task.CompletedTask);

            var serviceRequest = new ServiceRequest { ContractId = 1, CostUSD = 100 };

            // Act
            var (success, message) = await _service.CreateServiceRequestAsync(serviceRequest);

            // Assert
            success.Should().BeTrue();
        }
    }
}