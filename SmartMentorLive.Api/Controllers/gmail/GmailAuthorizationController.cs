using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartMentorLive.Infrastructure.Options;

namespace SmartMentorLive.Api.Controllers.gmail
{
    [Route("api/[controller]")]
    [ApiController]
    public class GmailAuthorizationController : ControllerBase
    {
        private readonly GmailOptions _options;
        private readonly IDataStore _tokenStore;

        public GmailAuthorizationController(IOptions<GmailOptions> options, IDataStore tokenStore)
        {
            _options = options.Value;
            _tokenStore = tokenStore;
        }

        [HttpGet("authorize")]
        public IActionResult Authorize()
        {
            var secrets = new ClientSecrets
            {
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret,
            };

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = secrets,
                Scopes = new[] { "https://www.googleapis.com/auth/gmail.send" },
                DataStore = _tokenStore
            });

            //Genrate a random state (can be any string, ideally stored to validate later)
            var state = Guid.NewGuid().ToString("N");

            var authUrl = flow.CreateAuthorizationCodeRequest(_options.RedirectUri);
            authUrl.State = state;
            return Redirect(authUrl.Build().AbsoluteUri);
        }

        [HttpGet("oauth2callback")]
        public async Task<IActionResult> Oauth2CallBack([FromQuery] string code, [FromQuery] string state)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Missing authorization code.");

            var secrets = new ClientSecrets
            {
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret,
            };

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = secrets,
                Scopes = new[] { "https://www.googleapis.com/auth/gmail.send" },
                DataStore = _tokenStore
            });

            var token = await flow.ExchangeCodeForTokenAsync(
                userId: _options.UserEmail,
                code: code,
                redirectUri: _options.RedirectUri,
                taskCancellationToken: CancellationToken.None);

            await _tokenStore.StoreAsync(_options.UserEmail, token);
            return Ok("Gmail account authorized succesfully. Token saved securely");
        }
    }
}
