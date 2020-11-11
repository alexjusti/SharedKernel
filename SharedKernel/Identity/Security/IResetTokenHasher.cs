using System;

namespace SharedKernel.Identity.Security
{
    public interface IResetTokenHasher
    {
        string GenerateHash();

        bool VerifyHash(string resetToken, string resetTokenAttempt);
    }
}