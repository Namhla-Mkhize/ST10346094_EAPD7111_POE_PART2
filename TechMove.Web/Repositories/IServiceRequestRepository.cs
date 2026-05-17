using TechMove.Web.Models; 

namespace TechMove.Web.Repositories
{
    public interface IServiceRequestRepository
    {
        Task<IEnumerable<ServiceRequest>> GetAllAsync();
        Task<ServiceRequest?> GetByIdAsync(int id);
        Task<IEnumerable<ServiceRequest>> GetByContractIdAsync(int contractId);
        Task AddAsync(ServiceRequest serviceRequest);
        Task UpdateAsync(ServiceRequest serviceRequest);
        Task DeleteAsync(int id);
    }
}
