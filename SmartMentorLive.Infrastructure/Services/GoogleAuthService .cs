using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using SmartMentorLive.Application.Interfaces.Services;
using SmartMentorLive.Infrastructure.Options;
using Org.BouncyCastle.Utilities;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace SmartMentorLive.Infrastructure.Services
{
    public class GoogleAuthService:IGoogleAuthService
    {
        private readonly GmailOptions _options;
        private readonly IDataStore _tokenStore;
        private readonly IOAuthStateService _stateService;
        private readonly ILogger<GoogleAuthService> _logger;    

        public GoogleAuthService(IOptions<GmailOptions> options, IDataStore tokenStore, IOAuthStateService stateService, ILogger<GoogleAuthService> logger)
        {
            _options = options.Value;
            _tokenStore = tokenStore;
            _stateService = stateService;
            _logger = logger;

            // DEBUG: Log the configuration
            _logger.LogInformation("Gmail Configuration - UserEmail: {UserEmail}, Scopes: {Scopes}",
                _options.UserEmail,
                string.Join(", ", _options.Scopes ?? new string[0]));
        }

        public async Task<string> GenerateAuthorizationUrlAsync()
        {
            _logger.LogInformation("Generating Google OAuth authorization URL");

            var flow = CreateGoogleFlow();
            //Genrate a random state (can be any string, ideally stored to validate later)
            var state = Guid.NewGuid().ToString("N");
            //store state with 5 min expiration


            _logger.LogInformation("Creating Oauth state : {State}", state);    
            await _stateService.StoreStateAsync(state, TimeSpan.FromMinutes(5));

            var request = flow.CreateAuthorizationCodeRequest(_options.RedirectUri);
            request.State = state;

            _logger.LogInformation("✅ Generated auth URL with state: {State}", state);
            return request.Build().AbsoluteUri;
        }
        public async Task HandleOAuthCallbackAsync(string code, string state)
        {
            _logger.LogInformation("🔄 Handling OAuth callback with state: {State}", state);
            // 1. Validate state
            if (!await _stateService.ValidateStateAsync(state))
                throw new UnauthorizedAccessException("Invalid or expired OAuth state.");

            _logger.LogInformation("✅ State validation passed");
            // Exchange code for token - let exceptions bubble up
            _logger.LogInformation("🔄 Exchanging authorization code for token");

            //proceed with token exchange
            var flow = CreateGoogleFlow();

            //exchange code for token
            var token = await flow.ExchangeCodeForTokenAsync(
                userId: _options.UserEmail,
                code: code,
                redirectUri: _options.RedirectUri,
                taskCancellationToken: CancellationToken.None);

            if(token == null)
            {
                _logger.LogError("❌ Failed to obtain token from Google");
                throw new UnauthorizedAccessException("Failed to obtain token from Google.");
            }
            _logger.LogInformation("✅ Successfully obtained token from Google");


            // Store token - let exceptions bubble up
            _logger.LogInformation("💾 Storing token for user: {UserEmail}", _options.UserEmail);

            //Store token securely
            await _tokenStore.StoreAsync(_options.UserEmail, token);

            _logger.LogInformation("✅ OAuth callback completed successfully");

        }


        private GoogleAuthorizationCodeFlow CreateGoogleFlow()
        {
            var secrets = new ClientSecrets
            {
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret,
            };

            return new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = secrets,
                Scopes = _options.Scopes ?? new[] { "https://mail.google.com/" },
                DataStore = _tokenStore
            });
        }

    }
}
