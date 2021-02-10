using System.Threading;
using System.Threading.Tasks;
using SharedKernel.Identity.Entities;
using SharedKernel.Shared;

namespace SharedKernel.Identity.Security
{
    public interface ITokenAuthenticationService<TUser>
        where TUser : User

    {
        public Task<Result<string>> AuthenticateAsync(
            string username,
            string password,
            CancellationToken cancellation = default);

        public Task<Result<string>> RefreshAsync(
            string oldToken,
            CancellationToken cancellation = default);
    }
}