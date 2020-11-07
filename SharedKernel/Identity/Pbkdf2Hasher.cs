﻿using System;
using System.Buffers.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SharedKernel.Identity
{
    public class Pbkdf2Hasher : IPasswordHasher
    {
        private int SaltLength { get; set; }

        private int IterationCount { get; set; }

        private int KeyLength { get; set; }

        public Pbkdf2Hasher(int saltLength, int iterationCount, int keyLength)
        {
            SaltLength = saltLength;
            IterationCount = iterationCount;
            KeyLength = keyLength;
        }

        private byte[] GenerateKey(string password, byte[] salt, int iterationCount, int keyLength)
        {
            var hasher = new Rfc2898DeriveBytes(password, salt, iterationCount, HashAlgorithmName.SHA512);

            return hasher.GetBytes(keyLength);
        }

        public string HashPassword(string password)
        {
            var salt = new byte[16];

            var key = GenerateKey(password, salt, IterationCount, KeyLength);

            var hash = $"{Convert.ToBase64String(salt)}.{IterationCount}.{Convert.ToBase64String(key)}";

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

            var testKey = GenerateKey(passwordAttempt, salt, iterationCount, trueKey.Length);

            return trueKey.Equals(testKey);
        }
    }
}