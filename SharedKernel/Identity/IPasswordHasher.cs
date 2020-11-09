using System.Threading.Tasks;

namespace SharedKernel.Identity
{
    public interface IPasswordHasher
    {
        /// <summary>
        /// Generate a storable hash derived from a password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        string HashPassword(string password);

        /// <summary>
        /// Verify a password attempt against a stored hash
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordAttempt"></param>
        /// <returns></returns>
        bool VerifyPassword(string password, string passwordAttempt);
    }
}