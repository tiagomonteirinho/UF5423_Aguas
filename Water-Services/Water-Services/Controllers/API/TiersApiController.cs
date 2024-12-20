using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Water_Services.Data;

namespace Water_Services.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TiersApiController : Controller
    {
        private readonly ITierRepository _tierRepository;

        public TiersApiController(ITierRepository tierRepository)
        {
            _tierRepository = tierRepository;
        }

        [AllowAnonymous]
        [HttpGet("gettiers")]
        public async Task<IActionResult> GetTiersAsync()
        {
            var tiers = await _tierRepository.GetTiersAsync();
            var tierDtos = _tierRepository.ConvertToTierDto(tiers);

            if (tierDtos == null || !tierDtos.Any())
            {
                return NotFound("Tiers not found.");
            }

            return Ok(tierDtos);
        }
    }
}
