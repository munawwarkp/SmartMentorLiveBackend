using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SmartMentorLive.Application.Interfaces.Services;
using SmartMentorLive.Infrastructure.Options;

namespace SmartMentorLive.Api.Controllers.gmail
{
    [Route("api/[controller]")]
    [ApiController]
    public class GmailAuthorizationController : ControllerBase
    {
        private readonly IGoogleAuthService _googleAuthService;

        public GmailAuthorizationController(IGoogleAuthService googleAuthService)
        {
            _googleAuthService = googleAuthService;
        }

        [HttpGet("authorize")]
        public async Task<IActionResult> Authorize()
        {
            var authUrl = await _googleAuthService.GenerateAuthorizationUrlAsync();
            Console.WriteLine(authUrl);
            return Redirect(authUrl);
        }

        [HttpGet("oauth2callback")]
        public async Task<IActionResult> Oauth2CallBack([FromQuery] string code, [FromQuery] string state)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Missing authorization code.");
            if (string.IsNullOrEmpty(state))
                return BadRequest("Missing OAuth state.");

            await _googleAuthService.HandleOAuthCallbackAsync(code, state);
            return Ok("Gmail account authorized succesfully. Token saved securely");
        }
    }
}
