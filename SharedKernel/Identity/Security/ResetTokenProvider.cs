using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SharedKernel.Common;

namespace SharedKernel.Identity.Security
{
    public class ResetTokenProvider : IResetTokenProvider
    {
        private int TokenLength { get; }

        private TimeSpan ValidDuration { get; }

        public ResetTokenProvider(int tokenLength, TimeSpan validDuration)
        {
            TokenLength = tokenLength;
            ValidDuration = validDuration;
        }

        public string GenerateToken()
        {
            var token = new byte[TokenLength];

            var random = new Random(DateTime.UtcNow.Millisecond);

            for (var i = 0; i < token.Length; i++)
            {
                var charType = random.Next(1, 3);

                token[i] = charType switch
                {
                    1 => (byte) random.Next(48, 57),    //Generate numerical character
                    2 => (byte) random.Next(65, 90),    //Generate uppercase character
                    3 => (byte) random.Next(97, 122),   //Generate lowercase character
                    _ => token[i]
                };
            }

            return Encoding.ASCII.GetString(token);
        }

        public string GenerateHash(string token)
        {
            var expiration = DateTime.UtcNow.Add(ValidDuration);

            var binaryToken = Encoding.ASCII.GetBytes(token);

            var tokenHash = new SHA512Managed().ComputeHash(binaryToken);

            return $"{expiration}.{Convert.ToBase64String(tokenHash)}";
        }

        public bool VerifyHash(string storedHash, string resetTokenAttempt)
        {
            var parts = storedHash.Split(".");

            if (parts.Length != 2)
                return false;

            var parse = DateTime.TryParse(parts[0], null, DateTimeStyles.AssumeUniversal, out var expiration);

            if (!parse)
                return false;

            byte[] trueTokenHash;

            try
            {
                trueTokenHash = Convert.FromBase64String(parts[1]);
            }
            catch
            {
                //Conversion from base64 string failed
                return false;
            }

            var binaryResetTokenAttempt = Encoding.ASCII.GetBytes(resetTokenAttempt);

            var resetTokenAttemptHash = new SHA512Managed().ComputeHash(binaryResetTokenAttempt);

            var expired = DateTime.Compare(DateTime.UtcNow, expiration) >= 0;

            return !expired && trueTokenHash.SequenceEqual(resetTokenAttemptHash);
        }
    }
}