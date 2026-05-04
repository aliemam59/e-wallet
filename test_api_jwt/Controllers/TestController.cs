using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace test_api_jwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [Authorize]
        [HttpGet("secure")]
        public IActionResult Secure()
        {
            return Ok("You are authenticated!");
        }
    }
}