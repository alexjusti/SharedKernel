using MongoDB.Entities;

namespace SharedKernel.Identity.Entities
{
    public class User : Entity
    {
        public string Email { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}