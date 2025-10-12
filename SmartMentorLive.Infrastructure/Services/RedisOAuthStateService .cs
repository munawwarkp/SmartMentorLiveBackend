using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SmartMentorLive.Application.Interfaces.Services;

namespace SmartMentorLive.Infrastructure.Services
{
    public class RedisOAuthStateService : IOAuthStateService
    {
        private readonly IDistributedCache _cache;
        private const string Prefix = "oauth_state:";
        private readonly ILogger<RedisOAuthStateService> _logger;
        public RedisOAuthStateService(IDistributedCache cache, ILogger<RedisOAuthStateService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task StoreStateAsync(string state, TimeSpan? expiration = null)
        {
            _logger.LogInformation("Storing OAuth state: {State}", state);

            if (string.IsNullOrWhiteSpace(state))
                throw new ArgumentException("State cannot be null or empty.", nameof(state));
            
            // Default expiration = 5 minutes if not provided
            var ttl = expiration ?? TimeSpan.FromSeconds(5);

            _logger.LogDebug("Settings redis key : {Key} with TTL : {TTL}", $"{Prefix}{state}", ttl);

            //store it in redis with a short expiration
            await _cache.SetStringAsync(
                $"{Prefix}{state}",
                "valid",
                //sets time to live (TTL) for the state
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = ttl
                });

            _logger.LogInformation("✅ Successfully stored OAuth state: {State}", state);

        }

        public async Task<bool> ValidateStateAsync(string state)
        {
            _logger.LogInformation("Validating OAuth state : {State}", state);

            if (string.IsNullOrWhiteSpace(state))
            {
                _logger.LogWarning(" Empty state provided for validation");
                return false; // invalid input

            }

            var key = $"{Prefix}{state}";
            _logger.LogDebug("Checking redis key : {Key}", key);

            //validate the state from redis
            // Let exceptions bubble up - global middleware will handle them
            var cachedState = await _cache.GetStringAsync(key);
            if (cachedState == null)
            {
                _logger.LogWarning("state not found or expired: {State}", state);
                return false;
            }

            _logger.LogInformation("state validation successful: {State}", state);  
            //remove the state after validation
            await _cache.RemoveAsync(key);
            return true;
        }
    }
}
