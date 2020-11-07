using System.Threading.Tasks;

namespace SharedKernel.Identity
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);

        bool VerifyPassword(string password, string passwordAttempt);
    }
}