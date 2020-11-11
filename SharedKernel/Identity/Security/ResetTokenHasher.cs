using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using SharedKernel.Common;
using SharedKernel.Common.Clock;

namespace SharedKernel.Identity.Security
{
    public class ResetTokenHasher : IResetTokenHasher
    {
        private int TokenLength { get; }

        private TimeSpan ValidDuration { get; }

        public ResetTokenHasher(int tokenLength, TimeSpan validDuration)
        {
            TokenLength = tokenLength;
            ValidDuration = validDuration;
        }

        public string GenerateHash()
        {
            var expiration = DateTime.UtcNow.Add(ValidDuration);

            var key = new byte[TokenLength];
            new Random().NextBytes(key);

            var tokenHash = SHA512.Create().ComputeHash(key);

            return $"{expiration}.{Convert.ToBase64String(tokenHash)}";
        }

        public bool VerifyHash(string resetToken, string resetTokenAttempt)
        {
            var parts = resetToken.Split(".");

            if (parts.Length != 2)
                return false;

            var parse = DateTime.TryParse(parts[0], null, DateTimeStyles.AssumeUniversal, out var expiration);

            if (!parse)
                return false;

            var trueTokenHash = new byte[Base64Helper.DataLength(parts[1])];
            parse = Convert.TryFromBase64String(parts[1], trueTokenHash, out var bytesWritten);

            if (!parse)
                return false;

            var attemptTokenHash = new byte[Base64Helper.DataLength(resetTokenAttempt)];
            parse = Convert.TryFromBase64String(resetTokenAttempt, attemptTokenHash, out bytesWritten);

            if (!parse)
                return false;

            var expired = DateTime.Compare(DateTime.UtcNow, expiration) >= 0;

            return !expired && trueTokenHash.SequenceEqual(attemptTokenHash);
        }
    }
}