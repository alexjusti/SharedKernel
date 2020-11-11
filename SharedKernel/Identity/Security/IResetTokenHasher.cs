using System;

namespace SharedKernel.Identity.Security
{
    public interface IResetTokenHasher
    {
        string GenerateHash(TimeSpan duration);

        bool VerifyHash(string resetToken, string resetTokenAttempt);
    }
}