using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UF5423_Aguas.Data;
using UF5423_Aguas.Data.API;
using UF5423_Aguas.Data.Entities;
using UF5423_Aguas.Models;

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
        public async Task<IActionResult> GetMeters()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return NotFound("User not found.");
            }

            var meters = await _meterRepository.GetMetersAsync(userEmail);
            var meterDtos = _meterRepository.ConvertToMeterDto(meters);

            if (meterDtos == null)
            {
                return NotFound("Meters not found.");
            }

            return Ok(meterDtos);
        }

        [HttpGet("getmeterdetails/{id}")]
        public async Task<IActionResult> GetMeterDetails(int id)
        {
            var meter = await _meterRepository.GetMeterWithConsumptionsAsync(id);
            if (meter == null)
            {
                return NotFound($"Meter not found.");
            }

            var meterDetailsDto = new MeterDetailsDto
            {
                Id = meter.Id,
                SerialNumber = meter.SerialNumber,
                Address = meter.Address,
                Consumptions = meter.Consumptions.Select(c => new ConsumptionDto
                {
                    Id = c.Id,
                    Date = c.Date.ToString("yyyy-MM-dd"),
                    Volume = c.Volume,
                    Status = c.Status,
                }).ToList()
            };

            return Ok(meterDetailsDto);
        }

        [HttpGet("getconsumptions")]
        public async Task<IActionResult> GetConsumptions()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return NotFound("User not found.");
            }

            var consumptions = await _meterRepository.GetConsumptionsAsync(userEmail);
            var consumptionDtos = _meterRepository.ConvertToConsumptionDto(consumptions);

            if (consumptionDtos == null)
            {
                return NotFound("Consumptions not found.");
            }

            return Ok(consumptionDtos);
        }

        [HttpGet("getconsumptiondetails/{id}")]
        public async Task<IActionResult> GetConsumptionDetails(int id)
        {
            var consumption = await _meterRepository.GetConsumptionByIdAsync(id);
            if (consumption == null)
            {
                return NotFound($"Consumption not found.");
            }

            var consumptionDetailsDto = new ConsumptionDto
            {
                Id = consumption.Id,
                Date = consumption.Date.ToString("yyyy-MM-dd"),
                Volume = consumption.Volume,
                Status = consumption.Status,
            };

            return Ok(consumptionDetailsDto);
        }

        [HttpGet("getinvoicedetails/{consumptionId}")]
        public async Task<IActionResult> GetInvoiceDetails(int consumptionId)
        {
            var invoice = await _meterRepository.GetInvoiceByConsumptionIdAsync(consumptionId);
            if (invoice == null)
            {
                return NotFound($"Invoice not found.");
            }

            var invoiceDto = new InvoiceDto
            {
                Id = invoice.Id,
                Price = invoice.Price,
                Consumption = new ConsumptionDto
                {
                    Id = invoice.Consumption.Id,
                    Date = invoice.Consumption.Date.ToString("yyyy-MM-dd"),
                    Volume = invoice.Consumption.Volume,
                    Status = invoice.Consumption.Status,
                },
                Meter = new MeterDto
                {
                    Id = invoice.Consumption.Meter.Id,
                    SerialNumber = invoice.Consumption.Meter.SerialNumber,
                    Address = invoice.Consumption.Meter.Address,
                },
            };

            return Ok(invoiceDto);
        }

        [HttpPut("buyconsumption/{id}")]
        public async Task<IActionResult> BuyConsumption(int id)
        {
            var consumption = await _meterRepository.GetConsumptionByIdAsync(id);
            if (consumption == null)
            {
                return NotFound($"Consumption not found.");
            }

            consumption.Status = "Payment confirmed";
            await _meterRepository.UpdateConsumptionAsync(consumption);

            return Ok("Consumption payment successfully confirmed.");
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

        [HttpPost("addconsumption")]
        [AllowAnonymous]
        public async Task<IActionResult> AddConsumption([FromBody] ConsumptionDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Could not process request.");
            }

            var meter = await _meterRepository.GetByIdAsync(model.MeterId);
            if (meter == null)
            {
                return NotFound($"Meter not found.");
            }

            var consumption = new ConsumptionViewModel
            {
                Volume = model.Volume,
                Date = DateTime.Parse(model.Date),
                MeterId = model.MeterId,
                Meter = meter,
            };

            await _meterRepository.AddConsumptionAsync(consumption);

            var meterUrl = Url.Action
            (
                "Details",
                "Meters",
                new { id = model.MeterId },
                protocol: HttpContext.Request.Scheme,
                host: AppConfig.Host
            );

            var notification = new Notification
            {
                Title = "Consumption awaiting approval.",
                Action = $"<a href=\"{meterUrl}\" class=\"btn btn-primary\">Go to meter</a>",
                ReceiverRole = "Employee"
            };

            await _notificationRepository.CreateAsync(notification);

            return Ok("Consumption successfully added.");
        }
    }
}
