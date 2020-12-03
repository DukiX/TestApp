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
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;

        public ProductsController(IProductsService productsService)
        {
            _productsService = productsService;
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = UserRoles.Prodavac)]
        public async Task<IActionResult> Add([FromBody] InProizvodDTO model)
        {
            var success = await _productsService.Add(model, HttpContext);

            if (success)
                return Ok(success);
            else
                return BadRequest(success);
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            var products = await _productsService.Get();

            if (products != null)
                return Ok(products);
            else
                return BadRequest();
        }
    }
}
