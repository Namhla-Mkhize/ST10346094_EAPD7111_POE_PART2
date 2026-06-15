using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechMove.Web.Models;
using TechMove.Web.Repositories;

namespace TechMove.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;

        public ClientsController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        // GET: api/clients
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var clients = await _clientRepository.GetAllAsync();
            return Ok(clients);
        }

        // GET: api/clients/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null) return NotFound(new { message = "Client not found" });
            return Ok(client);
        }

        // POST: api/clients
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Client client)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _clientRepository.AddAsync(client);
            return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
        }

        // PUT: api/clients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Client client)
        {
            if (id != client.Id) return BadRequest(new { message = "ID mismatch" });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _clientRepository.UpdateAsync(client);
            return Ok(client);
        }

        // DELETE: api/clients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null) return NotFound(new { message = "Client not found" });

            await _clientRepository.DeleteAsync(id);
            return Ok(new { message = "Client deleted successfully" });
        }
    }
}