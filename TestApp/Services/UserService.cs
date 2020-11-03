using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TestApp.Enums;
using TestApp.Identity;
using TestApp.Models;
using TestApp.Tools;

namespace TestApp.Services
{
    public interface IUserService
    {
        Task<UserAuthData> Authenticate(AuthenticateRequest model);
        Task<UserAuthData> RefreshToken(string token);
        Task<UserAuthData> Register(Register model);
        Task<Account> Get(HttpContext context);
    }

    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private IOptions<AppSettingsModel> _settings;

        public UserService(UserManager<ApplicationUser> userManager, IOptions<AppSettingsModel> settings)
        {
            _userManager = userManager;
            _settings = settings;
        }

        public async Task<UserAuthData> Authenticate(AuthenticateRequest model)
        {
            var user = await _userManager.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.UserName == model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return await CreateTokens(user, UserType.User, true);
            }
            return null;
        }

        public async Task<UserAuthData> RefreshToken(string token)
        {
            var user = await _userManager.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == token));
            if (user == null) return null;

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (refreshToken.RefreshTokenExpiration < DateTime.UtcNow || !refreshToken.IsActive)
                return null;

            await DeleteOldRefreshToken(user, token);

            return await CreateTokens(user, UserType.User, true);
        }

        public async Task<UserAuthData> Register(Register model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return null;

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return null;

            return await CreateTokens(user, UserType.User, true);
        }

        public async Task<Account> Get(HttpContext context)
        {
            var identity = context.User.Identity as ClaimsIdentity;
            if (identity == null) return null;

            IEnumerable<Claim> claims = identity.Claims;
            string userName = TokensHelper.GetClaimFromJwt(context, ClaimTypes.Name);

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return null;

            return new Account
            {
                Username = user.UserName,
                Email = user.Email
            };
        }

        private async Task<UserAuthData> CreateTokens(ApplicationUser user, UserType userType, bool rememberMe)
        {
            var authClaims = new List<Claim>
            {
                new Claim(CustomClaims.UserId.ToString(),user.Id),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(CustomClaims.UserType.ToString(),userType.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Value.JwtSecret));

            DateTime expires = DateTime.UtcNow.AddMinutes(_settings.Value.AccessTokenDurationMinutes);

            var accessToken = new JwtSecurityToken(
                issuer: _settings.Value.JwtValidIssuer,
                audience: _settings.Value.JwtValidAudience,
                expires: expires,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var refreshToken = GenerateRefreshToken();
            await AddNewAndDeleteOldRefreshTokens(user, refreshToken);

            return new UserAuthData
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                Expires = expires.ToString("O"),
                ExpiresIn = (expires - DateTime.UtcNow).TotalSeconds.ToString(),
                Issued = DateTime.UtcNow.ToString("O"),
                UserEmail = user.Email,
                UserType = userType.ToString(),
                RefreshToken = rememberMe ? refreshToken : ""
            };
        }

        private async Task AddNewAndDeleteOldRefreshTokens(ApplicationUser user, string refreshToken)
        {
            var numberOfRefTokensPerUser = _settings.Value.NumberOfRefreshTokensPerUser;
            user.RefreshTokens = user.RefreshTokens.OrderByDescending(d => d.RefreshTokenExpiration).ToList()
                .GetRange(0, (user.RefreshTokens.Count < numberOfRefTokensPerUser - 1) ? user.RefreshTokens.Count : numberOfRefTokensPerUser - 1);

            user.RefreshTokens.Add(new RefreshToken
            {
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(_settings.Value.RefreshTokenDurationDays),
                Token = refreshToken,
                User = user,
                IsActive = true
            });

            await _userManager.UpdateAsync(user);
        }

        private async Task DeleteOldRefreshToken(ApplicationUser user, string refreshTokenToDelete)
        {
            var rtToDel = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshTokenToDelete);
            if (rtToDel != null)
                user.RefreshTokens.Remove(rtToDel);

            await _userManager.UpdateAsync(user);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
