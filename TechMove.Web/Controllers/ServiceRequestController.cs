using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMove.Web.Models;
using TechMove.Web.Repositories;
using TechMove.Web.Services; 

namespace TechMove.Web.Controllers
{
    public class ServiceRequestController : Controller
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IServiceRequestService _serviceRequestService;
        private readonly ICurrencyService _currencyService;

        public ServiceRequestController(
            IServiceRequestRepository serviceRequestRepository,
            IContractRepository contractRepository,
            IServiceRequestService serviceRequestService,
            ICurrencyService currencyService)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _contractRepository = contractRepository;
            _serviceRequestService = serviceRequestService;
            _currencyService = currencyService;
        }

        // GET: ServiceRequest
        public async Task<IActionResult> Index()
        {
            var serviceRequests = await _serviceRequestRepository.GetAllAsync();
            return View(serviceRequests);
        }

        // GET: ServiceRequest/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(id);
            if (serviceRequest == null) return NotFound();
            return View(serviceRequest);
        }

        // GET: ServiceRequest/Create
        public async Task<IActionResult> Create()
        {
            // Get current exchange rate from API
            var rate = await _currencyService.GetUsdToZarRateAsync();
            ViewBag.ExchangeRate = rate;

            await PopulateContractsDropdown();
            return View();
        }

        // POST: ServiceRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest serviceRequest)
        {
            if (ModelState.IsValid)
            {
                // Get exchange rate and calculate ZAR
                var rate = await _currencyService.GetUsdToZarRateAsync();
                serviceRequest.CostZAR = _currencyService.ConvertUsdToZar(serviceRequest.CostUSD, rate);
                serviceRequest.ExchangeRateUsed = rate;
                serviceRequest.CreatedAt = DateTime.UtcNow;

                // Business logic check - blocks expired/onhold contracts
                var (success, message) = await _serviceRequestService.CreateServiceRequestAsync(serviceRequest);

                if (!success)
                {
                    ModelState.AddModelError("", message);
                    ViewBag.ExchangeRate = rate;
                    await PopulateContractsDropdown();
                    return View(serviceRequest);
                }

                return RedirectToAction(nameof(Index));
            }

            var currentRate = await _currencyService.GetUsdToZarRateAsync();
            ViewBag.ExchangeRate = currentRate;
            await PopulateContractsDropdown();
            return View(serviceRequest);
        }

        // GET: ServiceRequest/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(id);
            if (serviceRequest == null) return NotFound();

            var rate = await _currencyService.GetUsdToZarRateAsync();
            ViewBag.ExchangeRate = rate;

            await PopulateContractsDropdown();
            return View(serviceRequest);
        }

        // POST: ServiceRequest/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var rate = await _currencyService.GetUsdToZarRateAsync();
                serviceRequest.CostZAR = _currencyService.ConvertUsdToZar(serviceRequest.CostUSD, rate);
                serviceRequest.ExchangeRateUsed = rate;

                await _serviceRequestRepository.UpdateAsync(serviceRequest);
                return RedirectToAction(nameof(Index));
            }

            await PopulateContractsDropdown();
            return View(serviceRequest);
        }

        // GET: ServiceRequest/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(id);
            if (serviceRequest == null) return NotFound();
            return View(serviceRequest);
        }

        // POST: ServiceRequest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _serviceRequestRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // API endpoint - get exchange rate for live conversion on page
        [HttpGet]
        public async Task<IActionResult> GetExchangeRate()
        {
            var rate = await _currencyService.GetUsdToZarRateAsync();
            return Json(new { rate });
        }

        private async Task PopulateContractsDropdown()
        {
            var contracts = await _contractRepository.GetAllAsync();
            ViewBag.Contracts = new SelectList(contracts, "Id", "ServiceLevel");
        }
    }
}
