using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Options;
using SmartMentorLive.Application.Interfaces.Services;
using SmartMentorLive.Infrastructure.Options;
using SmartMentorLive.Infrastructure.Persistence.Context;
using SmartMentorLive.Infrastructure.Persistence.Repositories;

namespace SmartMentorLive.Infrastructure.Services
{
    //responsible for managing Gmail OAuth2 tokens
    public class GmailTokenManager:ITokenManager
    {
        private readonly GmailOptions _options;
        private readonly AppDbContext _context;
        private readonly IDataStore _tokenStore;

        public GmailTokenManager(IOptions<GmailOptions> options, IDataStore tokenStore)
        {
            _options = options.Value;
            _tokenStore = tokenStore;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            //app credentials
            var secrets = new ClientSecrets
            {
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret,
            };

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = secrets,
                Scopes = new[] { "https://www.googleapis.com/auth/gmail.send" },
                DataStore = _tokenStore // your DbTokenStore
            });

            // UserCredential linked to your flow and email
            var credential = new UserCredential(flow, _options.UserEmail, null);

            // Refresh token automatically if stale
            if (credential.Token.IsStale)
                await credential.RefreshTokenAsync(CancellationToken.None);

            return await credential.GetAccessTokenForRequestAsync();
        }
    }
}
