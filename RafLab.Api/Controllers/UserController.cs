using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RafLab.Application.Services;
using RafLab.Application.Services._i;

namespace RafLab.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IExternalUserService _userService;
        public UserController(IExternalUserService userservice)
        {
            _userService = userservice;
        }

        [HttpGet]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            return Ok(user);
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var user = await _userService.GetAllUsersAsync();

            return Ok(user);
        }

        [HttpGet("GetCaheUsers")]
        public async Task<IActionResult> GetCaheUsers()
        {
            var user = await _userService.GetCacheUser();

            return Ok(user);
        }
    }
}
