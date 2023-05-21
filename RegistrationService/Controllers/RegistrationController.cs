using Microsoft.AspNetCore.Mvc;
using RegistrationService.Models;
using RegistrationService.Services;
using Microsoft.Extensions.Logging;

namespace RegistrationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;
        private readonly ILogger<RegistrationController> _logger;

        public RegistrationController(IRegistrationService registrationService, ILogger<RegistrationController> logger)
        {
            _registrationService = registrationService;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Register(Registration registration)
        {
            try
            {
                // Викликати методи сервісу для обробки реєстрації клієнта
                _registrationService.Register(registration);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering client");
                return StatusCode(500, "An error occurred while processing the registration.");
            }
        }
        
        // get all registrations and show them in json format
        [HttpGet]
        public IActionResult GetAllRegistrations()
        {
            try
            {
                var registrations = _registrationService.GetAllRegistrations();
                
                return new JsonResult(registrations);
                
                //return Ok(registrations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all registrations");
                return StatusCode(500, "An error occurred while retrieving the registrations.");
            }
        }
    }
}