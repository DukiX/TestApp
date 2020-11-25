using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TestApp.Models;
using TestApp.Services;

namespace TestApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] AuthenticateRequest model)
        {
            var authData = await _userService.Authenticate(model);

            if (authData == null)
                return BadRequest();

            return Ok(authData);
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest token)
        {
            var authData = await _userService.RefreshToken(token.Token);

            if (authData == null)
                return BadRequest();

            return Ok(authData);
        }

        [HttpGet]
        [Authorize]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            var account = await _userService.Get(HttpContext);

            if (account == null)
                return BadRequest();

            return Ok(account);
        }

        [HttpPut]
        [Authorize]
        [Route("")]
        public async Task<IActionResult> Update(Update model)
        {
            var account = await _userService.Update(HttpContext, model);

            if (account == null)
                return BadRequest();

            return Ok(account);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            var authData = await _userService.Register(model);

            if (authData == null)
                return BadRequest();

            return Ok(authData);
        }

        [HttpDelete]
        [Authorize]
        [Route("")]
        public async Task<IActionResult> Delete()
        {
            var success = await _userService.Delete(HttpContext);

            if (!success)
                return BadRequest();

            return Ok(success);
        }

        [HttpPut]
        [Authorize]
        [Route("image")]
        public async Task<IActionResult> UploadImage()
        {
            var success = await _userService.SaveImage(HttpContext);

            if (!success)
                return BadRequest();

            return Ok(success);
        }

        [HttpGet]
        [Authorize]
        [Route("image")]
        public async Task<IActionResult> GetImage()
        {
            var memory = await _userService.GetImage(HttpContext);

            if (memory == null)
                return BadRequest("Image not found");

            return Ok(Convert.ToBase64String(memory.GetBuffer()));
        }

        [HttpDelete]
        [Authorize]
        [Route("image")]
        public IActionResult DeleteImage()
        {
            var success = _userService.DeleteImage(HttpContext);

            if (!success)
                return BadRequest();

            return Ok(success);
        }

        [HttpPost]
        [Authorize]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePassword model)
        {
            var authData = await _userService.ChangePassword(HttpContext, model);

            if (authData == null)
                return BadRequest();

            return Ok(authData);
        }

        //[HttpPost]
        //[Route("register-admin")]
        //public async Task<IActionResult> RegisterAdmin([FromBody] Register model)
        //{
        //    var userExists = await _userManager.FindByNameAsync(model.Username);
        //    if (userExists != null)
        //        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

        //    ApplicationUser user = new ApplicationUser()
        //    {
        //        Email = model.Email,
        //        SecurityStamp = Guid.NewGuid().ToString(),
        //        UserName = model.Username
        //    };
        //    var result = await _userManager.CreateAsync(user, model.Password);
        //    if (!result.Succeeded)
        //        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

        //    if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
        //        await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        //    if (!await _roleManager.RoleExistsAsync(UserRoles.User))
        //        await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

        //    if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
        //    {
        //        await _userManager.AddToRoleAsync(user, UserRoles.Admin);
        //    }

        //    return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        //}
    }
}
