using SharedKernel.Shared;

namespace SharedKernel.Identity.Errors
{
    public static class IdentityErrors
    {
        public static Error UserNotFound(string userIdentifier)
        {
            return new Error
            {
                Code = "UserNotFound",
                Message = $"User {userIdentifier} not found"
            };
        }

        public static Error InvalidPassword = new Error
        {
            Code = "InvalidPassword",
            Message = "The password provided is invalid."
        };
    }
}