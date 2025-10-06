using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;
using SmartMentorLive.Infrastructure.Options;

namespace SmartMentorLive.Tools.GmailAuth
{
    public class GmailAuthService
    {
        private readonly IDataStore tokenStore;
        private readonly GmailOptions options;
        public GmailAuthService(IConfiguration configuration, IDataStore dataStore)
        {
            options = configuration.GetSection("Gmail").Get<GmailOptions>();
            tokenStore = dataStore;
        }


       public async Task AuthorizeAndStoreTokenAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // Google OAuth client secrets
                var secrets = new ClientSecrets
                {
                    ClientId = options.ClientId,
                    ClientSecret = options.ClientSecret
                };

                //OAuth flow
                var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = secrets,
                    Scopes = new[] { "https://www.googleapis.com/auth/gmail.send" },
                });

                var app = new AuthorizationCodeInstalledApp(flow, new LocalServerCodeReceiver());
                var credential = await app.AuthorizeAsync(options.UserEmail, cancellationToken);  //get user credentials

                //save token to db
                await tokenStore.StoreAsync(options.UserEmail, credential.Token);

                Console.WriteLine("✅ Gmail token stored successfully for " + options.UserEmail);

            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error during Gmail authorization: " + ex.Message);

            }

        }


    }
}
