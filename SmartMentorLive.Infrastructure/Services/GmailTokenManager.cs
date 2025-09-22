using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Options;
using SmartMentorLive.Application.Interfaces.Services;
using SmartMentorLive.Infrastructure.Options;
using SmartMentorLive.Infrastructure.Persistence.Context;
using SmartMentorLive.Infrastructure.Persistence.Repositories;

namespace SmartMentorLive.Infrastructure.Services
{
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
            var secrets = new ClientSecrets
            {
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret,
            };

            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                secrets,
                new[] { "https://mail.google.com/" },
                _options.UserEmail,
                CancellationToken.None,
                _tokenStore
                //new FileDataStore("GmailTokenStore", true)
            );

            if (credential.Token.IsStale)
            {
                await credential.RefreshTokenAsync(CancellationToken.None);
            }

            return await credential.GetAccessTokenForRequestAsync();
        }
    }
}
