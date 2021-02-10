using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SharedKernel.Identity.Security
{
    public class ResetTokenService : IResetTokenService
    {
        private readonly ResetTokenSettings _settings;

        public ResetTokenService(ResetTokenSettings settings)
        {
            _settings = settings;
        }

        public string GenerateResetTokenHash(out string token)
        {
            var rawToken = new byte[_settings.TokenLength];

            var random = new Random();

            for (var i = 0; i < rawToken.Length; i++)
            {
                rawToken[i] = random.Next(1, 4) switch
                {
                    1 => (byte) random.Next(48, 58),    //Generate a number
                    2 => (byte) random.Next(65, 91),    //Generate an uppercase letter
                    3 => (byte) random.Next(97, 123),   //Generate a lowercase letter
                    _ => rawToken[i]
                };
            }

            token = Encoding.ASCII.GetString(rawToken);

            var hasherKey = Convert.FromBase64String(_settings.Secret);

            using var hasher = new HMACSHA512(hasherKey);
            var hash = hasher.ComputeHash(rawToken);

            return Convert.ToBase64String(hash);
        }

        public bool VerifyResetToken(string attempt, string storedHash)
        {
            var hasherKey = Convert.FromBase64String(_settings.Secret);

            var rawAttempt = Encoding.ASCII.GetBytes(attempt);

            using var hasher = new HMACSHA512(hasherKey);
            var attemptHash = hasher.ComputeHash(rawAttempt);

            var rawStoredHash = Convert.FromBase64String(storedHash);

            return attemptHash.SequenceEqual(rawStoredHash);
        }
    }
}