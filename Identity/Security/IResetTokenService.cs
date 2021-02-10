namespace SharedKernel.Identity.Security
{
    public interface IResetTokenService
    {
        string GenerateResetTokenHash(out string token);

        bool VerifyResetToken(string attempt, string storedHash);
    }
}