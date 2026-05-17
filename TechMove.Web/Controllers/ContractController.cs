using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMove.Web.Models;
using TechMove.Web.Repositories;
using TechMove.Web.Services;

namespace TechMove.Web.Controllers
{
    public class ContractController : Controller
    {
        private readonly IContractRepository _contractRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IFileService _fileService;

        public ContractController(
            IContractRepository contractRepository,
            IClientRepository clientRepository,
            IFileService fileService)
        {
            _contractRepository = contractRepository;
            _clientRepository = clientRepository;
            _fileService = fileService;
        }

        // GET: Contract
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, ContractStatus? status)
        {
            var contracts = await _contractRepository.SearchAsync(startDate, endDate, status);

            // Pass filter values back to view
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.Status = status;
            ViewBag.StatusList = new SelectList(Enum.GetValues(typeof(ContractStatus)));

            return View(contracts);
        }

        // GET: Contract/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        // GET: Contract/Create
        public async Task<IActionResult> Create()
        {
            await PopulateClientsDropdown();
            return View();
        }

        // POST: Contract/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract, IFormFile? signedAgreement)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload
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

                await _contractRepository.AddAsync(contract);
                return RedirectToAction(nameof(Index));
            }

            await PopulateClientsDropdown();
            return View(contract);
        }

        // GET: Contract/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null) return NotFound();
            await PopulateClientsDropdown();
            return View(contract);
        }

        // POST: Contract/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contract contract, IFormFile? signedAgreement)
        {
            if (id != contract.Id) return NotFound();

            if (ModelState.IsValid)
            {
                // Handle new file upload
                if (signedAgreement != null && signedAgreement.Length > 0)
                {
                    if (!_fileService.IsValidPdf(signedAgreement))
                    {
                        ModelState.AddModelError("SignedAgreement", "Only PDF files are allowed.");
                        await PopulateClientsDropdown();
                        return View(contract);
                    }

                    // Delete old file if exists
                    if (!string.IsNullOrEmpty(contract.SignedAgreementPath))
                        _fileService.DeleteFile(contract.SignedAgreementPath);

                    var (filePath, fileName) = await _fileService.SaveFileAsync(signedAgreement);
                    contract.SignedAgreementPath = filePath;
                    contract.SignedAgreementFileName = fileName;
                }

                await _contractRepository.UpdateAsync(contract);
                return RedirectToAction(nameof(Index));
            }

            await PopulateClientsDropdown();
            return View(contract);
        }

        // GET: Contract/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        // POST: Contract/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract != null && !string.IsNullOrEmpty(contract.SignedAgreementPath))
                _fileService.DeleteFile(contract.SignedAgreementPath);

            await _contractRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Contract/Download/5
        public async Task<IActionResult> Download(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null || string.IsNullOrEmpty(contract.SignedAgreementPath))
                return NotFound();

            var fileBytes = await System.IO.File.ReadAllBytesAsync(contract.SignedAgreementPath);
            return File(fileBytes, "application/pdf", contract.SignedAgreementFileName);
        }

        // Helper to populate clients dropdown
        private async Task PopulateClientsDropdown()
        {
            var clients = await _clientRepository.GetAllAsync();
            ViewBag.Clients = new SelectList(clients, "Id", "Name");
        }
    }
}