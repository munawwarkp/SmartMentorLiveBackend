using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Util.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Parameters;
using SmartMentorLive.Domain.Entities.Oauth;
using SmartMentorLive.Infrastructure.Persistence.Context;

namespace SmartMentorLive.Infrastructure.Persistence.TokenStore
{
    public class DbTokenStore:IDataStore
    {
        private readonly AppDbContext _context;
        private readonly byte[] _encryptionKey;
        private readonly ILogger<DbTokenStore> _logger;

        public DbTokenStore(AppDbContext appDbContext, IConfiguration configuration, ILogger<DbTokenStore> logger)
        {
            _context = appDbContext;
            _logger = logger;

            var keyBase64 = configuration["AES:Key"]; // store base64 key in secrets/env
            if (string.IsNullOrWhiteSpace(keyBase64))
                throw new Exception("AES key missing in configuration (AES:Key).");

            _encryptionKey = Convert.FromBase64String(keyBase64);
            if (_encryptionKey.Length != 32)
                throw new Exception("AES key must be 32 bytes (base64).");

        }

        public async Task StoreAsync<T>(string key, T value)
        {
            _logger.LogInformation("Storing token for key: {Key}", key);

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty", nameof(key));

            if (value == null)
                throw new ArgumentNullException(nameof(value), "Cannot store null token");

            //validate and parse token
            var json = JsonConvert.SerializeObject(value);
            var googleToken = JsonConvert.DeserializeObject<Google.Apis.Auth.OAuth2.Responses.TokenResponse>(json);
            //var encryptedToken = Encrypt(json, _encryptionKey);

            // DEBUG: Log token details
            _logger.LogInformation("Token details - AccessToken: {HasAccess}, RefreshToken: {HasRefresh}, Issued: {Issued}",
                !string.IsNullOrEmpty(googleToken?.AccessToken),
                !string.IsNullOrEmpty(googleToken?.RefreshToken),
                googleToken?.IssuedUtc);

            if (googleToken == null)
                throw new InvalidOperationException("Invalid token response format");

            if (string.IsNullOrEmpty(googleToken.AccessToken))
                throw new InvalidOperationException("Access token is missing in the token response(google response)");

            if (googleToken.IssuedUtc == null || googleToken.ExpiresInSeconds == null)
                throw new InvalidOperationException("Token issue time or expiry is missing in the token response");

            var expiredIn = googleToken.ExpiresInSeconds ?? 3600;
            var expiryDate = googleToken.IssuedUtc.AddSeconds(expiredIn);

            //encrypt token
            var encryptedAccessToken = Encrypt(googleToken.AccessToken, _encryptionKey);
            // Refresh token might be null on subsequent auths, handle it properly
            string encryptedRefreshToken = null;


            if (!string.IsNullOrEmpty(googleToken.RefreshToken))
            {
                encryptedRefreshToken = Encrypt(googleToken.RefreshToken, _encryptionKey);
                _logger.LogInformation("Refresh token included and encrypted");
            }

            // Use transaction for data consistency (let any exceptions bubble up)
            using var transaction = await _context.Database.BeginTransactionAsync();

            var existing = await _context.OAuthTokens
                .FirstOrDefaultAsync(t => t.UserEmail == key && t.Provider=="Gmail");

            if (existing == null)
            {
                _context.OAuthTokens.Add(new OAuthToken
                {
                    Provider = "Gmail",
                    UserEmail = key,
                    AccessTokenEncrypted = encryptedAccessToken,
                    RefreshTokenEncrypted = encryptedRefreshToken,
                    ExpiryDate = expiryDate,
                    CreatedAt = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow
                });

               await _context.SaveChangesAsync();
                _logger.LogInformation("New token record created for {Key}", key);
            }
            else
            {
                //update existing token
                existing.AccessTokenEncrypted = encryptedAccessToken;

                // Only update refresh token if we have a new one
                if (!string.IsNullOrEmpty(encryptedRefreshToken))
                {
                    existing.RefreshTokenEncrypted = encryptedRefreshToken;
                    _logger.LogInformation("Refresh token updated");

                }
                existing.ExpiryDate = expiryDate;
                existing.LastModifiedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Existing OAuth token updated for: {Key}", key);

            }

            //commit transaction
            await transaction.CommitAsync();
            _logger.LogInformation("✅ Token storage operation completed for key: {Key}", key);

        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var token = await _context.OAuthTokens
                .FirstOrDefaultAsync(t => t.UserEmail == key && t.Provider == "Gmail");

            if (token == null) return default;

            _logger.LogInformation("Retrieving token for key: {Key}", key);

            // Decrypt access token (required) - let exceptions bubble up
            var accessToken = Decrypt(token.AccessTokenEncrypted, _encryptionKey);

            //Decrypt refresh token only if exists
            string refreshToken = null;

            if (!string.IsNullOrEmpty(token.RefreshTokenEncrypted))
            {
                refreshToken = Decrypt(token.RefreshTokenEncrypted, _encryptionKey);
            }

            var remaining = (long)(token.ExpiryDate - DateTime.UtcNow).TotalSeconds;

            //calculate issued utc correctly
            var issuedUtc = token.ExpiryDate.AddSeconds(-(token.ExpiryDate - token.CreatedAt).TotalSeconds);

            var tokenResponse = new Google.Apis.Auth.OAuth2.Responses.TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                IssuedUtc = issuedUtc, // original issue time
                ExpiresInSeconds = remaining > 0 ? remaining : 0
            };

            var json = JsonConvert.SerializeObject(tokenResponse);
            return JsonConvert.DeserializeObject<T>(json);

            //var json = Decrypt(token.AccessToken, _encryptionKey); // decrypt first
            //return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task DeleteAsync<T>(string key)
        {
            var token = await _context.OAuthTokens
                .FirstOrDefaultAsync(t => t.UserEmail == key && t.Provider == "Gmail");

            if (token != null)
            {
                _context.OAuthTokens.Remove(token);
                await _context.SaveChangesAsync();
            }
        }

        public Task ClearAsync()
        {
            _context.OAuthTokens.RemoveRange(_context.OAuthTokens);
            return _context.SaveChangesAsync();
        }

        public static string Encrypt(string plaintext, byte[] key)
        {
            if(string.IsNullOrWhiteSpace(plaintext))
                throw new ArgumentNullException(nameof(plaintext), "Plaintext cannot be null or empty for encryption");

            if (key == null || key.Length != 32)
                throw new ArgumentException("Key must be 256 bits (32 bytes).", nameof(key));

            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
            // Nonce (aka IV) should be 12 bytes for AES-GCM
            byte[] nonce = RandomNumberGenerator.GetBytes(12);
            byte[] tag = new byte[16];
            byte[] ciphertext = new byte[plaintextBytes.Length];

            using var aesGcm = new AesGcm(key);
            aesGcm.Encrypt(nonce, plaintextBytes, ciphertext, tag);

            // Combine nonce + tag + ciphertext
            byte[] output = new byte[nonce.Length + tag.Length + ciphertext.Length];
            Buffer.BlockCopy(nonce, 0, output, 0, nonce.Length);
            Buffer.BlockCopy(tag, 0, output, nonce.Length, tag.Length);
            Buffer.BlockCopy(ciphertext, 0, output, nonce.Length + tag.Length, ciphertext.Length);

            return Convert.ToBase64String(output);
        }

        public static string Decrypt(string cipherTextBase64, byte[] key)
        {
            if (key.Length != 32)
                throw new ArgumentException("Key must be 256 bits (32 bytes).", nameof(key));

            byte[] fullCipher = Convert.FromBase64String(cipherTextBase64);

            byte[] nonce = new byte[12];
            byte[] tag = new byte[16];
            byte[] ciphertext = new byte[fullCipher.Length - nonce.Length - tag.Length];

            Buffer.BlockCopy(fullCipher, 0, nonce, 0, nonce.Length);
            Buffer.BlockCopy(fullCipher, nonce.Length, tag, 0, tag.Length);
            Buffer.BlockCopy(fullCipher, nonce.Length + tag.Length, ciphertext, 0, ciphertext.Length);

            byte[] plaintextBytes = new byte[ciphertext.Length];

            using var aesGcm = new AesGcm(key);
            aesGcm.Decrypt(nonce, ciphertext, tag, plaintextBytes);

            return Encoding.UTF8.GetString(plaintextBytes);
        }

    }

   
    }
