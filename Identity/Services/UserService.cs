using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Entities;
using SharedKernel.Identity.Entities;
using SharedKernel.Identity.Security;
using SharedKernel.Shared;

namespace SharedKernel.Identity.Services
{
    public class UserService<TUser> : IUserService<TUser>
        where TUser : User
    {
        private readonly IPasswordService _passwordService;
        private readonly IResetTokenService _resetTokenService;

        public UserService(IPasswordService passwordService, IResetTokenService resetTokenService)
        {
            _passwordService = passwordService;
            _resetTokenService = resetTokenService;
        }

        public async Task<Result<TUser>> CreateUserAsync(TUser user, CancellationToken cancellation = default)
        {
            user.Password = _passwordService.HashPassword(user.Password);

            await user.SaveAsync(cancellation: cancellation);

            return user;
        }

        public async Task<Result<IEnumerable<TUser>>> GetUsersAsync(Expression<Func<TUser, bool>> expression,
            CancellationToken cancellation = default)
        {
            return await DB.Find<TUser>()
                .ManyAsync(expression, cancellation);
        }

        public async Task<Result<TUser>> GetUserByIdAsync(string id, CancellationToken cancellation = default)
        {
            return await DB.Find<TUser>()
                .OneAsync(id, cancellation);
        }

        public async Task<Result<TUser>> GetUserByEmailAsync(string email, CancellationToken cancellation = default)
        {
            return await DB.Find<TUser>()
                .Match(u => u.Email == email)
                .ExecuteFirstAsync(cancellation);
        }

        public async Task<Result<TUser>> GetUserByUsernameAsync(string username,
            CancellationToken cancellation = default)
        {
            return await DB.Find<TUser>()
                .Match(u => u.Username == username)
                .ExecuteFirstAsync(cancellation);
        }

        public async Task<Result<TUser>> UpdateUserAsync(TUser user, CancellationToken cancellation = default)
        {
            await user.SaveAsync(cancellation: cancellation);

            return user;
        }

        public async Task<Result> DeleteUserAsync(string id, CancellationToken cancellation = default)
        {
            await DB.DeleteAsync<TUser>(id, cancellation: cancellation);

            return Result.Ok();
        }
    }
}