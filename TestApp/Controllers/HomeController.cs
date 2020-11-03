using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TestApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public string Get()
        {
            return "hello";
        }

        [HttpGet]
        [Authorize]
        [Route("authorized")]
        public string GetAuthorized()
        {
            return "authorized hello";
        }
    }
}
