using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using UF5423_Aguas.Data;

namespace UF5423_Aguas.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MetersController : ControllerBase
    {
        private readonly IMeterRepository _meterRepository;

        public MetersController(IMeterRepository meterRepository)
        {
            _meterRepository = meterRepository;
        }

        [HttpGet]
        public IActionResult GetMeters()
        {
            return Ok(_meterRepository.GetAll());
        }
    }
}
