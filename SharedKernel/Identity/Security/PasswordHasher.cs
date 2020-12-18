using System;
using System.Security.Cryptography;

namespace SharedKernel.Identity.Security
{
    public class PasswordHasherOptions
    {
        public int SaltLength { get; set; } = 16;

        public int IterationCount { get; set; } = 10000;

        public int KeyLength { get; set; } = 32;
    }

    public class PasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasherOptions _options;

        public PasswordHasher(PasswordHasherOptions options)
        {
            _options = options;
        }

        private static byte[] GenerateKey(string password, byte[] salt, int iterationCount, int keyLength)
        {
            var hasher = new Rfc2898DeriveBytes(
                password,
                salt,
                iterationCount,
                HashAlgorithmName.SHA512);

            return hasher.GetBytes(keyLength);
        }

        public string HashPassword(string password)
        {
            var salt = new byte[_options.SaltLength];

            var key = GenerateKey(
                password,
                salt,
                _options.IterationCount,
                _options.KeyLength);

            var hash = $"{Convert.ToBase64String(salt)}.{_options.IterationCount}.{Convert.ToBase64String(key)}";

            return hash;
        }

        public bool VerifyPassword(string password, string passwordAttempt)
        {
            var parts = password.Split(".");

            if (parts.Length != 3)
                return false;

            var salt = Convert.FromBase64String(parts[0]);

            var iterationCount = int.Parse(parts[1]);

            var trueKey = Convert.FromBase64String(parts[2]);

            var testKey = GenerateKey(
                passwordAttempt,
                salt,
                iterationCount,
                trueKey.Length);

            return trueKey.Equals(testKey);
        }
    }
}