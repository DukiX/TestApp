using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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

        [HttpGet]
        [Route("")]
        [Authorize(Roles = UserRoles.Prodavac)]
        public async Task<IActionResult> GetAll()
        {
            var outNarudzbinaDTOs = await _narudzbinaService.GetAllNarudzbina(HttpContext);

            if (outNarudzbinaDTOs == null)
                return BadRequest();

            return Ok(outNarudzbinaDTOs);
        }

        [HttpPut]
        [Route("")]
        [Authorize(Roles = UserRoles.Prodavac)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNarudzbinaDTO model)
        {
            var outNarudzbinaDTO = await _narudzbinaService.UpdateNarudzbina(id, model);

            if (outNarudzbinaDTO == null)
                return BadRequest();

            return Ok(outNarudzbinaDTO);
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(Roles = UserRoles.Prodavac)]
        public async Task<IActionResult> Get(Guid id)
        {
            var outNarudzbinaDTO = await _narudzbinaService.GetNarudzbina(id);

            if (outNarudzbinaDTO == null)
                return BadRequest();

            return Ok(outNarudzbinaDTO);
        }
    }
}
