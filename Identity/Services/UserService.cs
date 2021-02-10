using MongoDB.Entities;
using SharedKernel.Identity.Entities;
using SharedKernel.Identity.Errors;
using SharedKernel.Identity.Security;
using SharedKernel.Shared;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

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
            var user = await DB.Find<TUser>()
                .OneAsync(id, cancellation);

            if (user == null)
                return Result.InputFailure(IdentityErrors.UserNotFound(id)) as Result<TUser>;

            return user;
        }

        public async Task<Result<TUser>> GetUserByEmailAsync(string email, CancellationToken cancellation = default)
        {
            var user = await DB.Find<TUser>()
                .Match(u => u.Email == email)
                .ExecuteFirstAsync(cancellation);

            if (user == null)
                return Result.InputFailure(IdentityErrors.UserNotFound(email)) as Result<TUser>;

            return user;
        }

        public async Task<Result<TUser>> GetUserByUsernameAsync(string username,
            CancellationToken cancellation = default)
        {
            var user = await DB.Find<TUser>()
                .Match(u => u.Username == username)
                .ExecuteFirstAsync(cancellation);

            if (user == null)
                return Result<TUser>.InputFailure(IdentityErrors.UserNotFound(username));

            return Result<TUser>.Ok(user);
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

        private async Task<Result> VerifyPasswordAsync(TUser user, string password)
        {
            var verify = _passwordService.VerifyPassword(password, user.Password);

            if (!verify)
                return Result.InputFailure(IdentityErrors.InvalidPassword);

            return await Task.FromResult(Result.Ok());
        }
    }
}