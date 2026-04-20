using Microsoft.AspNetCore.Mvc;

namespace DineOS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            Console.WriteLine($"USERNAME: '{request.Username}'");
            Console.WriteLine($"PASSWORD: '{request.Password}'");
            var token = await _authService.Login(request.Username, request.Password);

            if (token == null)
                return Unauthorized();

            return Ok(new
            {
                token,
                username = request.Username
            });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
