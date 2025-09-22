using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Util.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SmartMentorLive.Domain.Entities.Oauth;
using SmartMentorLive.Infrastructure.Persistence.Context;

namespace SmartMentorLive.Infrastructure.Persistence.TokenStore
{
    public class DbTokenStore:IDataStore
    {
        private readonly AppDbContext _context;
        private readonly byte[] _encryptionKey;

        public DbTokenStore(AppDbContext appDbContext, IConfiguration configuration)
        {
            _context = appDbContext;

            var keyBase64 = configuration["AES:Key"]; // store base64 key in secrets/env
            if (string.IsNullOrWhiteSpace(keyBase64))
                throw new Exception("AES key missing in configuration (AES:Key).");

            _encryptionKey = Convert.FromBase64String(keyBase64);
            if (_encryptionKey.Length != 32)
                throw new Exception("AES key must be 32 bytes (base64).");

        }

        public async Task StoreAsync<T>(string key, T value)
        {
            var json = JsonConvert.SerializeObject(value);
            var encryptedToken = Encrypt(json, _encryptionKey);

            var existing = await _context.OAuthTokens
                .FirstOrDefaultAsync(t => t.UserEmail == key);

            if (existing == null)
            {
                _context.OAuthTokens.Add(new OAuthToken
                {
                    UserEmail = key,
                    TokenJson = encryptedToken,
                    CreatedAt = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow
                });
            }
            else
            {
                existing.TokenJson = encryptedToken;
                existing.LastModifiedDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var token = await _context.OAuthTokens
                .FirstOrDefaultAsync(t => t.UserEmail == key);

            if (token == null) return default;

            var json = Decrypt(token.TokenJson, _encryptionKey); // decrypt first
            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task DeleteAsync<T>(string key)
        {
            var token = await _context.OAuthTokens
                .FirstOrDefaultAsync(t => t.UserEmail == key);

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
            if (key.Length != 32)
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
