using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TestApp.DB;
using TestApp.Models;
using TestApp.Services;

namespace TestApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NarudzbinaController : ControllerBase
    {
        private readonly INarudzbinaService _narudzbinaService;

        public NarudzbinaController(INarudzbinaService narudzbinaService)
        {
            _narudzbinaService = narudzbinaService;
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = UserRoles.Kupac)]
        public async Task<IActionResult> Add([FromBody] InNarudzbinaDTO model)
        {
            var outNarudzbinaDTOs = await _narudzbinaService.AddNarudzbina(model, HttpContext);

            if (outNarudzbinaDTOs == null)
                return BadRequest();

            return Ok(outNarudzbinaDTOs);
        }
    }
}
