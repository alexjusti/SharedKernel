using MongoDB.Entities;
using SharedKernel.Data;

namespace SharedKernel.Identity
{
    public abstract class User : Entity
    {
        public string Email { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}