using MongoDB.Entities;

namespace SharedKernel.Identity
{
    public abstract class User : Entity
    {
        public string Email { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string PasswordResetToken { get; set; }

        public bool IsActive { get; set; }
    }
}