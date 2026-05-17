using Microsoft.EntityFrameworkCore;
using TechMove.Web.Data;
using TechMove.Web.Models;

namespace TechMove.Web.Repositories
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        private readonly AppDbContext _context;

        public ServiceRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceRequest>> GetAllAsync() =>
            await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .ToListAsync();

        public async Task<ServiceRequest?> GetByIdAsync(int id) =>
            await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c!.Client)
                .FirstOrDefaultAsync(sr => sr.Id == id);

        public async Task<IEnumerable<ServiceRequest>> GetByContractIdAsync(int contractId) =>
            await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .Where(sr => sr.ContractId == contractId)
                .ToListAsync();

        public async Task AddAsync(ServiceRequest serviceRequest)
        {
            _context.ServiceRequests.Add(serviceRequest);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ServiceRequest serviceRequest)
        {
            _context.ServiceRequests.Update(serviceRequest);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest != null)
            {
                _context.ServiceRequests.Remove(serviceRequest);
                await _context.SaveChangesAsync();
            }
        }
    }
}
