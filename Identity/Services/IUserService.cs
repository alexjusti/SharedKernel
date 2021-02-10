using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SharedKernel.Identity.Entities;
using SharedKernel.Shared;

namespace SharedKernel.Identity.Services
{
    public interface IUserService<TUser>
        where TUser : User
    {
        Task<Result<TUser>> CreateUserAsync(
            TUser user,
            CancellationToken cancellation = default);

        Task<Result<IEnumerable<TUser>>> GetUsersAsync(
            Expression<Func<TUser, bool>> expression,
            CancellationToken cancellation = default);

        Task<Result<TUser>> GetUserByIdAsync(
            string id,
            CancellationToken cancellation = default);

        Task<Result<TUser>> GetUserByEmailAsync(
            string email,
            CancellationToken cancellation = default);

        Task<Result<TUser>> GetUserByUsernameAsync(
            string username,
            CancellationToken cancellation = default);

        Task<Result<TUser>> UpdateUserAsync(
            TUser user,
            CancellationToken cancellation = default);

        Task<Result> DeleteUserAsync(
            string id,
            CancellationToken cancellation = default);
    }
}