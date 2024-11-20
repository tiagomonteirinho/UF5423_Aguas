using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UF5423_Aguas.Data;

namespace UF5423_Aguas.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MetersApiController : Controller
    {
        private readonly IMeterRepository _meterRepository;

        public MetersApiController(IMeterRepository meterRepository)
        {
            _meterRepository = meterRepository;
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

            if (meterDtos is null || !meterDtos.Any())
            {
                return NotFound("Meters not found.");
            }

            return Ok(meterDtos);
        }
    }
}
