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
            var product = await _productsService.Add(model, HttpContext);

            if (product != null)
                return Ok(product);
            else
                return BadRequest(product);
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = UserRoles.Prodavac)]
        public async Task<IActionResult> Update(Guid id, [FromBody] InProizvodDTO model)
        {
            var product = await _productsService.Update(id, model, HttpContext);

            if (product != null)
                return Ok(product);
            else
                return BadRequest(product);
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productsService.GetAll();

            if (products != null)
                return Ok(products);
            else
                return BadRequest();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var product = await _productsService.Get(id);

            if (product != null)
                return Ok(product);
            else
                return BadRequest();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _productsService.Delete(id);

            if (success)
                return Ok(success);
            else
                return BadRequest(success);
        }

        [HttpPut]
        [Route("UploadImage/{id}")]
        public async Task<IActionResult> UploadImage(Guid id)
        {
            var success = await _productsService.SaveImage(HttpContext, id);

            if (success)
                return Ok(success);
            else
                return BadRequest(success);
        }
    }
}
