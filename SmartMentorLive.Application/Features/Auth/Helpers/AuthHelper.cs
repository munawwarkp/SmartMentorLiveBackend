using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SmartMentorLive.Domain.Entities.Users;

namespace SmartMentorLive.Application.Features.Auth.Helpers
{
    public class AuthHelper
    {
        public static void VerifyPassword(User user, string password, IPasswordHasher<User> hasher)
        {
            var verifyPassword = hasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (verifyPassword == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }
        }

        public static string HashToken(string token)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }
    }
}
