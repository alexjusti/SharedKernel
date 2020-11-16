using System;
using System.Linq;
using System.Threading.Tasks;
using SharedKernel.Common;
using SharedKernel.Data;
using SharedKernel.Identity.Dtos;
using SharedKernel.Identity.Security;

namespace SharedKernel.Identity
{
    public class UserManager<TUser>
        : IUserManager<TUser>
        where TUser : User
    {
        private readonly IRepository<TUser> _userRepository;

        private readonly IPasswordHasher _passwordHasher;

        private readonly IResetTokenProvider _resetTokenProvider;

        public UserManager(
            IRepository<TUser> userRepository,
            IPasswordHasher passwordHasher,
            IResetTokenProvider resetTokenProvider)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _resetTokenProvider = resetTokenProvider;
        }

        public async Task<ActionResult<TUser>> AddUserAsync(TUser user)
        {
            var emailTaken = await _userRepository.ExistsAsync(u => u.Email == user.Email);

            if (emailTaken)
                return (ActionResult<TUser>) ActionResult.ApplicationFailureResult(UserManagerErrors.EmailTaken);

            var usernameTaken = await _userRepository.ExistsAsync(u => u.Username == user.Username);

            if (usernameTaken)
                return (ActionResult<TUser>) ActionResult.ApplicationFailureResult(UserManagerErrors.UsernameTaken);

            user.Password = _passwordHasher.HashPassword(user.Password);

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

        protected ActionResult VerifyUserPassword(TUser user, string passwordAttempt)
        {
            var verifyPassword = _passwordHasher.VerifyPassword(user.Password, passwordAttempt);

            return verifyPassword
                ? ActionResult.SuccessResult()
                : ActionResult.ApplicationFailureResult(UserManagerErrors.PasswordInvalid);
        }

        public async Task<ActionResult> VerifyUserPasswordByIdAsync(string id, string passwordAttempt)
        {
            var getUser = await GetUserByIdAsync(id);

            return getUser.Success
                ? VerifyUserPassword(getUser.Result, passwordAttempt)
                : getUser;
        }

        public async Task<ActionResult> VerifyUserPasswordByEmailAsync(string email, string passwordAttempt)
        {
            var getUser = await GetUserByEmailAsync(email);

            return getUser.Success
                ? VerifyUserPassword(getUser.Result, passwordAttempt)
                : getUser;
        }

        public async Task<ActionResult> VerifyUserPasswordByUsernameAsync(string username, string passwordAttempt)
        {
            var getUser = await GetUserByUsernameAsync(username);

            return getUser.Success
                ? VerifyUserPassword(getUser.Result, passwordAttempt)
                : getUser;
        }

        public async Task<ActionResult<TUser>> UpdateUserAsync(TUser user)
        {
            var update = await _userRepository.UpdateAsync(user);

            return update;
        }

        public async Task<ActionResult> ChangeUserEmailAsync(string id, ChangeEmailDto dto)
        {
            var getUser = await GetUserByIdAsync(id);

            if (!getUser.Success)
                return getUser;

            var emailAlreadyExists =
                await _userRepository.ExistsAsync(u =>
                    string.Equals(u.Email, dto.NewEmail, StringComparison.CurrentCultureIgnoreCase));

            if (emailAlreadyExists)
                return ActionResult.ApplicationFailureResult(UserManagerErrors.EmailTaken);

            getUser.Result.Email = dto.NewEmail;

            return await UpdateUserAsync(getUser.Result);
        }

        public async Task<ActionResult> ChangeUserUsernameAsync(string id, ChangeUsernameDto dto)
        {
            var getUser = await GetUserByIdAsync(id);

            if (!getUser.Success)
                return getUser;

            var usernameAlreadyExists =
                await _userRepository.ExistsAsync(u =>
                    string.Equals(u.Username, dto.NewUsername, StringComparison.CurrentCultureIgnoreCase));

            if (usernameAlreadyExists)
                return ActionResult.ApplicationFailureResult(UserManagerErrors.UsernameTaken);

            getUser.Result.Username = dto.NewUsername;

            return await UpdateUserAsync(getUser.Result);
        }

        public async Task<ActionResult> ChangeUserPasswordAsync(ChangePasswordDto dto)
        {
            var getUser = await GetUserByIdAsync(dto.UserId);

            if (!getUser.Success)
                return getUser;

            var verifyPassword = VerifyUserPassword(getUser.Result, dto.CurrentPassword);

            if (!verifyPassword.Success)
                return verifyPassword;

            getUser.Result.Password = _passwordHasher.HashPassword(dto.NewPassword);

            return await UpdateUserAsync(getUser.Result);
        }

        public async Task<ActionResult<string>> GeneratePasswordResetTokenAsync(string id)
        {
            var getUser = await GetUserByIdAsync(id);

            //TODO: Investigate this
            if (!getUser.Success)
                return getUser as ActionResult<string>;

            var resetToken = _resetTokenProvider.GenerateToken();

            var resetTokenHash = _resetTokenProvider.GenerateHash(resetToken);

            getUser.Result.PasswordResetToken = resetTokenHash;

            var updateUser = await UpdateUserAsync(getUser.Result);

            if (!updateUser.Success)
                return updateUser as ActionResult<string>;

            return ActionResult<string>.SuccessResult(resetToken);
        }

        public async Task<ActionResult> AttemptPasswordResetAsync(AttemptPasswordResetDto attemptPasswordResetDto)
        {
            var getUser = await GetUserByIdAsync(attemptPasswordResetDto.UserId);

            if (!getUser.Success)
                return getUser;

            var verifyPasswordResetToken = _resetTokenProvider.VerifyHash(
                getUser.Result.PasswordResetToken,
                attemptPasswordResetDto.PasswordResetToken);

            if (!verifyPasswordResetToken)
                return ActionResult.ApplicationFailureResult(UserManagerErrors.PasswordResetTokenInvalid);

            var newPasswordHash = _passwordHasher.HashPassword(attemptPasswordResetDto.NewPassword);

            getUser.Result.Password = newPasswordHash;

            var updateUser = await UpdateUserAsync(getUser.Result);

            return updateUser;
        }

        public async Task<ActionResult> DeactivateUserAsync(string id)
        {
            var getUser = await GetUserByIdAsync(id);

            if (!getUser.Success)
                return getUser;

            getUser.Result.IsActive = false;

            var updateUser = await UpdateUserAsync(getUser.Result);

            return updateUser;
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

        public static ApplicationError PasswordResetTokenInvalid => new ApplicationError
        {
            Code = "PasswordResetTokenInvalid",
            Message = "The password reset token provided is not a valid password reset token"
        };
    }
}