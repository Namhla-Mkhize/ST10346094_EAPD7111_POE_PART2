using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMove.Web.Models;
using TechMove.Web.Services;

namespace TechMove.Web.Controllers
{
    public class ServiceRequestController : Controller
    {
        private readonly ApiService _apiService;

        public ServiceRequestController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var serviceRequests = await _apiService.GetServiceRequestsAsync();
            return View(serviceRequests);
        }

        public async Task<IActionResult> Details(int id)
        {
            var serviceRequest = await _apiService.GetServiceRequestAsync(id);
            if (serviceRequest == null) return NotFound();
            return View(serviceRequest);
        }

        public async Task<IActionResult> Create()
        {
            var rate = await _apiService.GetExchangeRateAsync();
            ViewBag.ExchangeRate = rate;
            await PopulateContractsDropdown();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest serviceRequest)
        {
            if (ModelState.IsValid)
            {
                var (success, message) = await _apiService.CreateServiceRequestAsync(serviceRequest);

                if (!success)
                {
                    ModelState.AddModelError("", message);
                    var rate = await _apiService.GetExchangeRateAsync();
                    ViewBag.ExchangeRate = rate;
                    await PopulateContractsDropdown();
                    return View(serviceRequest);
                }

                return RedirectToAction(nameof(Index));
            }

            var currentRate = await _apiService.GetExchangeRateAsync();
            ViewBag.ExchangeRate = currentRate;
            await PopulateContractsDropdown();
            return View(serviceRequest);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var serviceRequest = await _apiService.GetServiceRequestAsync(id);
            if (serviceRequest == null) return NotFound();

            var rate = await _apiService.GetExchangeRateAsync();
            ViewBag.ExchangeRate = rate;

            await PopulateContractsDropdown();
            return View(serviceRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _apiService.UpdateServiceRequestAsync(serviceRequest);
                return RedirectToAction(nameof(Index));
            }

            await PopulateContractsDropdown();
            return View(serviceRequest);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var serviceRequest = await _apiService.GetServiceRequestAsync(id);
            if (serviceRequest == null) return NotFound();
            return View(serviceRequest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiService.DeleteServiceRequestAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetExchangeRate()
        {
            var rate = await _apiService.GetExchangeRateAsync();
            return Json(new { rate });
        }

        private async Task PopulateContractsDropdown()
        {
            var contracts = await _apiService.GetContractsAsync();
            ViewBag.Contracts = new SelectList(contracts, "Id", "ServiceLevel");
        }
    }
}