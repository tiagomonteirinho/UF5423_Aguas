using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UF5423_Aguas.Data;

namespace UF5423_Aguas.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
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
