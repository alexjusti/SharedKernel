using System;
using System.Threading.Tasks;
using SharedKernel.Common;
using SharedKernel.Identity.Dtos;

namespace SharedKernel.Identity
{
    public interface IUserManager<TUser>
        where TUser : User
    {
        Task<ActionResult<TUser>> AddUserAsync(TUser user);

        Task<ActionResult<TUser>> GetUserByIdAsync(string id);

        Task<ActionResult<TUser>> GetUserByEmailAsync(string email);

        Task<ActionResult<TUser>> GetUserByUsernameAsync(string username);

        Task<ActionResult> VerifyUserPasswordByIdAsync(string id, string passwordAttempt);

        Task<ActionResult> VerifyUserPasswordByEmailAsync(string email, string passwordAttempt);

        Task<ActionResult> VerifyUserPasswordByUsernameAsync(string username, string passwordAttempt);

        Task<ActionResult<TUser>> UpdateUserAsync(TUser user);

        Task<ActionResult> ChangeUserPasswordAsync(ChangePasswordDto changePasswordDto);

        Task<ActionResult> DeleteUserAsync(string id);
    }
}