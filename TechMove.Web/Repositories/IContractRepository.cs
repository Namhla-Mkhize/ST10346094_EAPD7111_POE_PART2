using TechMove.Web.Models; 

namespace TechMove.Web.Repositories
{
    public interface IContractRepository
    {
        Task<IEnumerable<Contract>> GetAllAsync();
        Task<Contract?> GetByIdAsync(int id);
        Task<IEnumerable<Contract>> SearchAsync(DateTime? startDate, DateTime? endDate, ContractStatus? status);
        Task AddAsync(Contract contract);
        Task UpdateAsync(Contract contract);
        Task DeleteAsync(int id);
    }
}
