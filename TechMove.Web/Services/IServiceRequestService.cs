using TechMove.Web.Models;

namespace TechMove.Web.Services
{

    public interface IServiceRequestService
    {
        Task<(bool success, string message)> CreateServiceRequestAsync(ServiceRequest serviceRequest);
    }
}
