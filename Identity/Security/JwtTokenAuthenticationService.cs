using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Identity.Entities;
using SharedKernel.Identity.Errors;
using SharedKernel.Identity.Services;
using SharedKernel.Shared;

namespace SharedKernel.Identity.Security
{
    public class JwtTokenAuthenticationService<TUser> : ITokenAuthenticationService<TUser>
        where TUser : User
    {
        private readonly IUserService<TUser> _userService;
        private readonly IPasswordService _passwordService;
        private readonly JwtSettings _settings;

        public JwtTokenAuthenticationService(IUserService<TUser> userService, IPasswordService passwordService,
            JwtSettings settings)
        {
            _userService = userService;
            _passwordService = passwordService;
            _settings = settings;
        }

        public async Task<Result<string>> AuthenticateAsync(string username, string password,
            CancellationToken cancellation = default)
        {
            var getUser = await _userService.GetUserByUsernameAsync(username, cancellation);

            if (!getUser.IsSuccess)
                return Result<string>.InputFailure(IdentityErrors.UserNotFound(username));

            var verifyPassword = _passwordService.VerifyPassword(password, getUser.Response.Password);

            if (!verifyPassword)
                return Result<string>.InputFailure(IdentityErrors.InvalidPassword);

            var claims = new[]
            {
                new Claim("user.id", getUser.Response.ID)
            };

            var key = new SymmetricSecurityKey(_settings.Secret);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                _settings.Issuer,
                _settings.Audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.DurationMinutes),
                signingCredentials: credentials);

            var encodedToken = new JwtSecurityTokenHandler()
                .WriteToken(token);

            return Result<string>.Ok(encodedToken);
        }

        public async Task<Result<string>> RefreshAsync(string oldToken, CancellationToken cancellation = default)
        {
            throw new System.NotImplementedException();
        }
    }
}