using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMove.Web.Models;
using TechMove.Web.Services;

namespace TechMove.Web.Controllers
{
    public class ContractController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IFileService _fileService;

        public ContractController(ApiService apiService, IFileService fileService)
        {
            _apiService = apiService;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, ContractStatus? status)
        {
            var contracts = await _apiService.GetContractsAsync(startDate, endDate, status);

            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.Status = status;
            ViewBag.StatusList = new SelectList(Enum.GetValues(typeof(ContractStatus)));

            return View(contracts);
        }

        public async Task<IActionResult> Details(int id)
        {
            var contract = await _apiService.GetContractAsync(id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateClientsDropdown();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract, IFormFile? signedAgreement)
        {
            if (ModelState.IsValid)
            {
                if (signedAgreement != null && signedAgreement.Length > 0)
                {
                    if (!_fileService.IsValidPdf(signedAgreement))
                    {
                        ModelState.AddModelError("SignedAgreement", "Only PDF files are allowed.");
                        await PopulateClientsDropdown();
                        return View(contract);
                    }

                    var (filePath, fileName) = await _fileService.SaveFileAsync(signedAgreement);
                    contract.SignedAgreementPath = filePath;
                    contract.SignedAgreementFileName = fileName;
                }

                await _apiService.CreateContractAsync(contract);
                return RedirectToAction(nameof(Index));
            }

            await PopulateClientsDropdown();
            return View(contract);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var contract = await _apiService.GetContractAsync(id);
            if (contract == null) return NotFound();
            await PopulateClientsDropdown();
            return View(contract);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contract contract, IFormFile? signedAgreement)
        {
            if (id != contract.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (signedAgreement != null && signedAgreement.Length > 0)
                {
                    if (!_fileService.IsValidPdf(signedAgreement))
                    {
                        ModelState.AddModelError("SignedAgreement", "Only PDF files are allowed.");
                        await PopulateClientsDropdown();
                        return View(contract);
                    }

                    if (!string.IsNullOrEmpty(contract.SignedAgreementPath))
                        _fileService.DeleteFile(contract.SignedAgreementPath);

                    var (filePath, fileName) = await _fileService.SaveFileAsync(signedAgreement);
                    contract.SignedAgreementPath = filePath;
                    contract.SignedAgreementFileName = fileName;
                }

                await _apiService.UpdateContractAsync(contract);
                return RedirectToAction(nameof(Index));
            }

            await PopulateClientsDropdown();
            return View(contract);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _apiService.GetContractAsync(id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiService.DeleteContractAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Download(int id)
        {
            var contract = await _apiService.GetContractAsync(id);
            if (contract == null || string.IsNullOrEmpty(contract.SignedAgreementPath))
                return NotFound();

            var fileBytes = await System.IO.File.ReadAllBytesAsync(contract.SignedAgreementPath);
            return File(fileBytes, "application/pdf", contract.SignedAgreementFileName);
        }

        private async Task PopulateClientsDropdown()
        {
            var clients = await _apiService.GetClientsAsync();
            ViewBag.Clients = new SelectList(clients, "Id", "Name");
        }
    }
}