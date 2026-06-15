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
    public class ContractsController : ControllerBase
    {
        private readonly IContractRepository _contractRepository;
        private readonly IFileService _fileService;

        public ContractsController(
            IContractRepository contractRepository,
            IFileService fileService)
        {
            _contractRepository = contractRepository;
            _fileService = fileService;
        }

        // GET: api/contracts
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] ContractStatus? status)
        {
            var contracts = await _contractRepository.SearchAsync(startDate, endDate, status);
            return Ok(contracts);
        }

        // GET: api/contracts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null) return NotFound(new { message = "Contract not found" });
            return Ok(contract);
        }

        // POST: api/contracts
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Contract contract)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _contractRepository.AddAsync(contract);
            return CreatedAtAction(nameof(GetById), new { id = contract.Id }, contract);
        }

        // PUT: api/contracts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Contract contract)
        {
            if (id != contract.Id) return BadRequest(new { message = "ID mismatch" });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _contractRepository.UpdateAsync(contract);
            return Ok(contract);
        }

        // PATCH: api/contracts/5/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] ContractStatus status)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null) return NotFound(new { message = "Contract not found" });

            contract.Status = status;
            await _contractRepository.UpdateAsync(contract);
            return Ok(new { message = "Contract status updated", status = contract.Status });
        }

        // DELETE: api/contracts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null) return NotFound(new { message = "Contract not found" });

            if (!string.IsNullOrEmpty(contract.SignedAgreementPath))
                _fileService.DeleteFile(contract.SignedAgreementPath);

            await _contractRepository.DeleteAsync(id);
            return Ok(new { message = "Contract deleted successfully" });
        }
    }
}