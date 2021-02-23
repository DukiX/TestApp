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
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = UserRoles.Kupac)]
        public async Task<IActionResult> Add([FromBody] InNarudzbinaDTO model)
        {
            var outNarudzbinaDTOs = await _orderService.Add(model, HttpContext);

            if (outNarudzbinaDTOs == null)
                return BadRequest();

            return Ok(outNarudzbinaDTOs);
        }

        [HttpGet]
        [Route("for-seller")]
        [Authorize(Roles = UserRoles.Prodavac)]
        public async Task<IActionResult> GetAllSeller()
        {
            var outNarudzbinaDTOs = await _orderService.GetAllForSeller(HttpContext);

            if (outNarudzbinaDTOs == null)
                return BadRequest();

            return Ok(outNarudzbinaDTOs);
        }

        [HttpGet]
        [Route("for-buyer")]
        [Authorize(Roles = UserRoles.Kupac)]
        public async Task<IActionResult> GetAllBuyer()
        {
            var outNarudzbinaDTOs = await _orderService.GetAllForBuyer(HttpContext);

            if (outNarudzbinaDTOs == null)
                return BadRequest();

            return Ok(outNarudzbinaDTOs);
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = UserRoles.Prodavac)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNarudzbinaDTO model)
        {
            var outNarudzbinaDTO = await _orderService.Update(id, model, HttpContext);

            if (outNarudzbinaDTO == null)
                return BadRequest();

            return Ok(outNarudzbinaDTO);
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(Guid id)
        {
            var outNarudzbinaDTO = await _orderService.Get(id, HttpContext);

            if (outNarudzbinaDTO == null)
                return BadRequest();

            return Ok(outNarudzbinaDTO);
        }
    }
}
