using TechMove.Web.Models;
using TechMove.Web.Repositories;

namespace TechMove.Web.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IContractRepository _contractRepository;

        public ServiceRequestService(
            IServiceRequestRepository serviceRequestRepository,
            IContractRepository contractRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _contractRepository = contractRepository;
        }

        public async Task<(bool success, string message)> CreateServiceRequestAsync(ServiceRequest serviceRequest)
        {
            // Get the parent contract
            var contract = await _contractRepository.GetByIdAsync(serviceRequest.ContractId);

            if (contract == null)
                return (false, "Contract not found.");

            // Workflow logic - block if Expired or OnHold
            if (contract.Status == ContractStatus.Expired)
                return (false, "Cannot create a Service Request for an Expired contract.");

            if (contract.Status == ContractStatus.OnHold)
                return (false, "Cannot create a Service Request for a contract that is On Hold.");

            await _serviceRequestRepository.AddAsync(serviceRequest);
            return (true, "Service Request created successfully.");
        }
    }
}
