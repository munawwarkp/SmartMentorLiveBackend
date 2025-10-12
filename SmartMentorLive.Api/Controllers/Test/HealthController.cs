using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace SmartMentorLive.Api.Controllers.Test
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly IDistributedCache _cache;

        public HealthController(IDistributedCache cache)
        {
            _cache = cache;
        }

        [HttpGet("redis")]
        public async Task<IActionResult> TestRedis()
        {
            try
            {
                var testKey = "test_key";
                var testValue = "test_value";

                await _cache.SetStringAsync(testKey, testValue, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

                var retrievedValue = await _cache.GetStringAsync(testKey);

                return Ok(new
                {
                    status = "redis is working",
                    stored = testValue,
                    retrieved = retrievedValue
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "Redis connection failed",
                    error = ex.Message
                });
            }
        }
    }

}
