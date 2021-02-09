using System;
using System.Linq;
using System.Security.Cryptography;

namespace SharedKernel.Identity.Security
{
    public class Pbkdf2PasswordService : IPasswordService
    {
        private readonly Pbkdf2Settings _settings;

        public Pbkdf2PasswordService(Pbkdf2Settings settings = default)
        {
            _settings = settings;
        }

        private static byte[] DeriveKey(string password, int iterations, byte[] salt, int keyLength)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA512);
            return pbkdf2.GetBytes(keyLength);
        }

        public string HashPassword(string password)
        {
            var salt = new byte[_settings.SaltLength];
            new Random().NextBytes(salt);

            var key = DeriveKey(password, _settings.IterationCount, salt, _settings.KeyLength);

            return $"{_settings.IterationCount}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        }

        public bool VerifyPassword(string attempt, string storedHash)
        {
            var parts = storedHash.Split('.');

            if (parts.Length != 3)
                return false;

            var iterations = int.Parse(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);

            var attemptKey = DeriveKey(attempt, iterations, salt, key.Length);

            return attemptKey.SequenceEqual(key);
        }
    }
}