using System;
using System.Threading.Tasks;
using SharedKernel.Common;
using SharedKernel.Identity.Dtos;

namespace SharedKernel.Identity
{
    public interface IUserManager<TUser>
        where TUser : User
    {
        /// <summary>
        /// Add a new user to the database
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<ActionResult<TUser>> AddUserAsync(TUser user);

        /// <summary>
        /// Get a user's record by their user ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ActionResult<TUser>> GetUserByIdAsync(string id);

        /// <summary>
        /// Get a user's record by their email address
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<ActionResult<TUser>> GetUserByEmailAsync(string email);

        /// <summary>
        /// Get a user's record by their username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<ActionResult<TUser>> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Verify a user's password, obtaining their record by their user ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="passwordAttempt"></param>
        /// <returns></returns>
        Task<ActionResult> VerifyUserPasswordByIdAsync(string id, string passwordAttempt);

        /// <summary>
        /// Verify a user's password, obtaining their record by their email address
        /// </summary>
        /// <param name="email"></param>
        /// <param name="passwordAttempt"></param>
        /// <returns></returns>
        Task<ActionResult> VerifyUserPasswordByEmailAsync(string email, string passwordAttempt);

        /// <summary>
        /// Verify a user's password, obtaining their record by their username
        /// </summary>
        /// <param name="username"></param>
        /// <param name="passwordAttempt"></param>
        /// <returns></returns>
        Task<ActionResult> VerifyUserPasswordByUsernameAsync(string username, string passwordAttempt);

        /// <summary>
        /// Update a user's record
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<ActionResult<TUser>> UpdateUserAsync(TUser user);

        /// <summary>
        /// Attempt to change the user's password
        /// </summary>
        /// <param name="changePasswordDto"></param>
        /// <returns></returns>
        Task<ActionResult> ChangeUserPasswordAsync(ChangePasswordDto changePasswordDto);

        /// <summary>
        /// Generate a password reset token and save it to the user's record
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ActionResult<string>> GeneratePasswordResetTokenAsync(string id);

        /// <summary>
        /// Attempt a password reset using a password reset token
        /// </summary>
        /// <param name="attemptPasswordResetDto"></param>
        /// <returns></returns>
        Task<ActionResult> AttemptPasswordResetAsync(AttemptPasswordResetDto attemptPasswordResetDto);

        /// <summary>
        /// Delete a user from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ActionResult> DeleteUserAsync(string id);
    }
}