using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SmartMentorLive.Api.Controllers.Test
{
    [Route("api/[controller]")]
    [ApiController]
    public class CorsTestController : ControllerBase
    {
        [HttpGet("test")]
        public IActionResult TestCors()
        {
            return Ok(new
            {
                message = "CORS test successful",
                timestamp = DateTime.UtcNow,
                origin = Request.Headers["Origin"].ToString()
            });
        }

        [HttpPost("test")]
        public IActionResult TestCorsPost([FromBody] object data)
        {
            return Ok(new
            {
                message = "CORS POST test successful",
                timestamp = DateTime.UtcNow,
                data = data
            });
        }
    }
}
