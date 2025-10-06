using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartMentorLive.Domain.Entities.Users;
using SmartMentorLive.Infrastructure.Options;
using SmartMentorLive.Infrastructure.Persistence.Context;
using SmartMentorLive.Infrastructure.Persistence.TokenStore;
using SmartMentorLive.Tools.GmailAuth;

namespace Tools
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //create the host
            //give access to every thing( config, db ) same way api does, just in consoel tool
            using var host = Host.CreateDefaultBuilder(args).ConfigureAppConfiguration((context,config) =>
            {
                config.AddUserSecrets<Program>(); //read your secrets
            })
            .ConfigureServices((context,services) =>
            {
                //Register the thing we need from main project
                services.AddDbContext<AppDbContext>();
                services.AddScoped<IDataStore, DbTokenStore>(); //where token will be saved
                services.AddScoped<GmailAuthService>(); //register created service
            })
            .Build();

            using var scope = host.Services.CreateScope();
            var gmailService = scope.ServiceProvider.GetRequiredService<GmailAuthService>();
            await gmailService.AuthorizeAndStoreTokenAsync();
        }
    }
}
