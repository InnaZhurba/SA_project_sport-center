using Microsoft.AspNetCore.Mvc;
using RegistrationService.Models;
using RegistrationService.Services;

namespace RegistrationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;

        public RegistrationController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpPost]
        public IActionResult Register(Registration registration)
        {
            // Викликати методи сервісу для обробки реєстрації клієнта
            _registrationService.Register(registration);

            return Ok();
        }
    }
}


/*using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RegistrationService.Models;
using RegistrationService.Services;

namespace RegistrationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;

        public RegistrationController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Registration>> GetRegistrationById(string id)
        {
            var registration = await _registrationService.GetRegistrationByIdAsync(id);
            if (registration == null)
            {
                return NotFound();
            }
            return registration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Registration>>> GetAllRegistrations()
        {
            var registrations = await _registrationService.GetAllRegistrationsAsync();
            return Ok(registrations);
        }

        [HttpPost]
        public async Task<ActionResult> CreateRegistration(Registration registration)
        {
            await _registrationService.CreateRegistrationAsync(registration);
            return CreatedAtAction(nameof(GetRegistrationById), new { id = registration.Id }, registration);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRegistration(string id, Registration registration)
        {
            if (id != registration.Id)
            {
                return BadRequest();
            }

            var existingRegistration = await _registrationService.GetRegistrationByIdAsync(id);
            if (existingRegistration == null)
            {
                return NotFound();
            }

            await _registrationService.UpdateRegistrationAsync(registration);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRegistration(string id)
        {
            var existingRegistration = await _registrationService.GetRegistrationByIdAsync(id);
            if (existingRegistration == null)
            {
                return NotFound();
            }

            await _registrationService.DeleteRegistrationAsync(id);
            return NoContent();
        }
    }
}*/
