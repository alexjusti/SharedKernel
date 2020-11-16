namespace SharedKernel.Identity.Security
{
    public interface IResetTokenProvider
    {
        string GenerateToken();

        string GenerateHash(string token);

        bool VerifyHash(string storedHash, string resetTokenAttempt);
    }
}