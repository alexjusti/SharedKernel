using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Entities;
using SharedKernel.Common;
using SharedKernel.Data;

namespace SharedKernel.Identity
{
    public class UserManager<TUser>
        : IUserManager<TUser>
        where TUser : User
    {
        private readonly IRepository<TUser> _userRepository;

        private readonly IPasswordHasher _passwordHasher;

        public UserManager(IRepository<TUser> userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<ActionResult<TUser>> AddUserAsync(TUser user)
        {
            var emailTaken = await _userRepository.ExistsAsync(u => u.Email == user.Email);

            if (emailTaken)
                return (ActionResult<TUser>) ActionResult.ApplicationFailureResult(UserManagerErrors.EmailTaken);

            var usernameTaken = await _userRepository.ExistsAsync(u => u.Username == user.Username);

            if (usernameTaken)
                return (ActionResult<TUser>) ActionResult.ApplicationFailureResult(UserManagerErrors.UsernameTaken);

            var add = await _userRepository.AddAsync(user);

            return add;
        }

        public async Task<ActionResult<TUser>> GetUserByIdAsync(string id)
        {
            var user = await _userRepository.GetAsync(id);

            return (ActionResult<TUser>) (user == null
                ? ActionResult.ApplicationFailureResult(UserManagerErrors.UserNotFound)
                : ActionResult<TUser>.SuccessResult(user));
        }

        public async Task<ActionResult<TUser>> GetUserByEmailAsync(string email)
        {
            var user = (await _userRepository.GetAsync(u => u.Email == email))
                .FirstOrDefault();

            return (ActionResult<TUser>)(user == null
                ? ActionResult.ApplicationFailureResult(UserManagerErrors.UserNotFound)
                : ActionResult<TUser>.SuccessResult(user));
        }

        public async Task<ActionResult<TUser>> GetUserByUsernameAsync(string username)
        {
            var user = (await _userRepository.GetAsync(u => u.Username == username))
                .FirstOrDefault();

            return (ActionResult<TUser>) (user == null
                ? ActionResult.ApplicationFailureResult(UserManagerErrors.UserNotFound)
                : ActionResult<TUser>.SuccessResult(user));
        }

        public async Task<ActionResult> VerifyUserPasswordByEmailAsync(string email, string passwordAttempt)
        {
            var getUser = await GetUserByEmailAsync(email);

            if (!getUser.Success)
                return getUser;

            var verifyPassword = _passwordHasher.VerifyPassword(getUser.Result.Password, passwordAttempt);

            return verifyPassword
                ? ActionResult.SuccessResult()
                : ActionResult.ApplicationFailureResult(UserManagerErrors.PasswordInvalid);
        }

        public async Task<ActionResult> VerifyUserPasswordByUsernameAsync(string username, string passwordAttempt)
        {
            var getUser = await GetUserByUsernameAsync(username);

            if (!getUser.Success)
                return getUser;

            var verifyPassowrd = _passwordHasher.VerifyPassword(getUser.Result.Password, passwordAttempt);

            return verifyPassowrd
                ? ActionResult.SuccessResult()
                : ActionResult.ApplicationFailureResult(UserManagerErrors.PasswordInvalid);
        }

        public async Task<ActionResult<TUser>> UpdateUserAsync(TUser user)
        {
            var update = await _userRepository.UpdateAsync(user);

            return update;
        }

        public async Task<ActionResult> DeleteUserAsync(string id)
        {
            var delete = await _userRepository.DeleteAsync(id);

            return delete;
        }
    }

    internal sealed class UserManagerErrors
    {
        public static ApplicationError EmailTaken => new ApplicationError
        {
            Code = "EmailTaken",
            Message = "The email provided has already been taken."
        };

        public static ApplicationError UsernameTaken => new ApplicationError
        {
            Code = "UsernameTaken",
            Message = "The username provided has already been taken."
        };

        public static ApplicationError UserNotFound => new ApplicationError
        {
            Code = "UserNotFound",
            Message = "The specified user was not found."
        };

        public static ApplicationError PasswordInvalid => new ApplicationError
        {
            Code = "PasswordInvalid",
            Message = "The password provided is invalid."
        };
    }
}