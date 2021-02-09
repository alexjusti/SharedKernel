namespace SharedKernel.Identity.Security
{
    public interface IPasswordService
    {
        string HashPassword(string password);

        bool VerifyPassword(string attempt, string storedHash);
    }
}