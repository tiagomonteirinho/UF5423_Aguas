using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UF5423_Aguas.Data;
using UF5423_Aguas.Data.API;
using UF5423_Aguas.Data.Entities;

namespace UF5423_Aguas.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MetersApiController : Controller
    {
        private readonly IMeterRepository _meterRepository;
        private readonly INotificationRepository _notificationRepository;

        public MetersApiController(IMeterRepository meterRepository, INotificationRepository notificationRepository)
        {
            _meterRepository = meterRepository;
            _notificationRepository = notificationRepository;
        }

        [HttpGet("getmeters")]
        public async Task<IActionResult> Get()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return NotFound("User not found.");
            }

            var meters = await _meterRepository.GetMetersAsync(userEmail);
            var meterDtos = _meterRepository.ConvertToMeterDtoAsync(meters);

            if (meterDtos == null || !meterDtos.Any())
            {
                return NotFound("Meters not found.");
            }

            return Ok(meterDtos);
        }

        [HttpPost("requestwatermeter")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestWaterMeter([FromBody] RequestWaterMeterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Could not process request.");
            }

            var notification = new Notification
            {
                Title = "New contract awaiting approval.",
                Message =
                $"<h5>Contract holder</h5>" +
                $"<p>{model.FullName}</p>" +
                $"<h5>Email</h5>" +
                $"<p>{model.Email}</p>" +
                $"<h5>Phone number</h5>" +
                $"<p>{model.PhoneNumber}</p>" +
                $"<h5>Address</h5>" +
                $"<p>{model.Address}</p>" +
                $"<h5>Postal code</h5>" +
                $"<p>{model.PostalCode}</p>" +
                $"<h5>Serial number</h5>" +
                $"<p>{model.SerialNumber}</p>",
                ReceiverRole = "Employee",
                NewAccountEmail = model.Email,
            };

            await _notificationRepository.CreateAsync(notification);
            notification.Action = $"<a href=\"{Url.Action("ForwardNotificationToAdmin", "Meters", new { id = notification.Id })}\" class=\"btn btn-primary\">Forward to administrator</a>";
            await _notificationRepository.UpdateAsync(notification);

            return Ok("Water meter request successfully submitted.");
        }
    }
}
