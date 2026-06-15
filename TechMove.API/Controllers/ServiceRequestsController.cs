using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechMove.Web.Models;
using TechMove.Web.Repositories;
using TechMove.Web.Services;

namespace TechMove.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IServiceRequestService _serviceRequestService;
        private readonly ICurrencyService _currencyService;

        public ServiceRequestsController(
            IServiceRequestRepository serviceRequestRepository,
            IServiceRequestService serviceRequestService,
            ICurrencyService currencyService)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _serviceRequestService = serviceRequestService;
            _currencyService = currencyService;
        }

        // GET: api/servicerequests
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var serviceRequests = await _serviceRequestRepository.GetAllAsync();
            return Ok(serviceRequests);
        }

        // GET: api/servicerequests/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(id);
            if (serviceRequest == null) return NotFound(new { message = "Service Request not found" });
            return Ok(serviceRequest);
        }

        // GET: api/servicerequests/contract/5
        [HttpGet("contract/{contractId}")]
        public async Task<IActionResult> GetByContractId(int contractId)
        {
            var serviceRequests = await _serviceRequestRepository.GetByContractIdAsync(contractId);
            return Ok(serviceRequests);
        }

        // POST: api/servicerequests
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ServiceRequest serviceRequest)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Currency conversion
            var rate = await _currencyService.GetUsdToZarRateAsync();
            serviceRequest.CostZAR = _currencyService.ConvertUsdToZar(serviceRequest.CostUSD, rate);
            serviceRequest.ExchangeRateUsed = rate;
            serviceRequest.CreatedAt = DateTime.UtcNow;

            // Business logic check
            var (success, message) = await _serviceRequestService.CreateServiceRequestAsync(serviceRequest);
            if (!success) return BadRequest(new { message });

            return CreatedAtAction(nameof(GetById), new { id = serviceRequest.Id }, serviceRequest);
        }

        // PUT: api/servicerequests/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.Id) return BadRequest(new { message = "ID mismatch" });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var rate = await _currencyService.GetUsdToZarRateAsync();
            serviceRequest.CostZAR = _currencyService.ConvertUsdToZar(serviceRequest.CostUSD, rate);
            serviceRequest.ExchangeRateUsed = rate;

            await _serviceRequestRepository.UpdateAsync(serviceRequest);
            return Ok(serviceRequest);
        }

        // DELETE: api/servicerequests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(id);
            if (serviceRequest == null) return NotFound(new { message = "Service Request not found" });

            await _serviceRequestRepository.DeleteAsync(id);
            return Ok(new { message = "Service Request deleted successfully" });
        }

        // GET: api/servicerequests/exchangerate
        [HttpGet("exchangerate")]
        public async Task<IActionResult> GetExchangeRate()
        {
            var rate = await _currencyService.GetUsdToZarRateAsync();
            return Ok(new { rate });
        }
    }
}