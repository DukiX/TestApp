using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TestApp.DB;
using TestApp.Enums;
using TestApp.ExceptionHandling;
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
        Task<Account> Update(HttpContext context, Update model);
        Task<bool> Delete(HttpContext context);
        Task<bool> SaveImage(HttpContext context);
        Task<byte[]> GetImage(HttpContext context);
        bool DeleteImage(HttpContext context);
        Task<UserAuthData> ChangePassword(HttpContext context, ChangePassword change);
    }

    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOptions<AppSettingsModel> _settings;

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
                var role = await _userManager.GetRolesAsync(user);
                return await CreateTokens(user, role.FirstOrDefault(), true);
            }
            throw new ErrorException(ErrorCode.AuthorizationFailed, "Uneli ste pogrešno korisničko ime ili lozinku. Pokušajte ponovo.");
        }

        public async Task<UserAuthData> RefreshToken(string token)
        {
            var user = await _userManager.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == token));
            if (user == null) throw new ErrorException(ErrorCode.RefreshTokenInvalid, "Kredencijali istekli. Ulogujte se ponovo.");

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (refreshToken.RefreshTokenExpiration < DateTime.UtcNow || !refreshToken.IsActive)
                throw new ErrorException(ErrorCode.RefreshTokenInvalid, "Kredencijali istekli. Ulogujte se ponovo.");

            await DeleteOldRefreshToken(user, token);

            var role = await _userManager.GetRolesAsync(user);
            return await CreateTokens(user, role.FirstOrDefault(), true);
        }

        public async Task<UserAuthData> Register(Register model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                throw new ErrorException(ErrorCode.UsernameAlreadyExists, "Greška pri kreiranju korisnika. Korisničko ime već postoji u sistemu.");

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                throw new ErrorException(ErrorCode.UserRegistrationError, "Greška pri kreiranju korisnika. Email adresa već postoji u sistemu.");

            await _userManager.AddToRoleAsync(user, model.Role);

            return await CreateTokens(user, model.Role, true);
        }

        public async Task<Account> Get(HttpContext context)
        {
            string userName = TokensHelper.GetClaimFromJwt(context, ClaimTypes.Name);

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new ErrorException(ErrorCode.UserNotFound, "Korisnik ne postoji u sistemu.");

            var role = TokensHelper.GetClaimFromJwt(context, ClaimTypes.Role);

            return new Account
            {
                Username = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Role = role
            };
        }

        public async Task<Account> Update(HttpContext context, Update model)
        {
            string userName = TokensHelper.GetClaimFromJwt(context, ClaimTypes.Name);

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new ErrorException(ErrorCode.UserNotFound, "Korisnik ne postoji u sistemu.");

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;

            var res = await _userManager.UpdateAsync(user);

            if (!res.Succeeded)
                throw new ErrorException(ErrorCode.UserUpdateError, "Greška pri čuvanju profila.");

            return new Account
            {
                Username = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber
            };
        }

        public async Task<bool> Delete(HttpContext context)
        {
            string userName = TokensHelper.GetClaimFromJwt(context, ClaimTypes.Name);

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return true;

            var res = await _userManager.DeleteAsync(user);

            if (!res.Succeeded)
                return false;

            return true;
        }

        public async Task<bool> SaveImage(HttpContext context)
        {
            IFormFile file;
            try
            {
                file = context.Request.Form.Files.FirstOrDefault(f => f.Name == "file");
                if (file == null)
                    throw new ErrorException(ErrorCode.ImageNotFound, "Slika nije pronađena.");
            }
            catch (Exception)
            {
                throw new ErrorException(ErrorCode.ImageNotFound, "Slika nije pronađena.");
            }
            if (file.Length > 10000000)
                throw new ErrorException(ErrorCode.ImageTooLarge, "Slika zauzima previše prostora.");

            string ext = Path.GetExtension(file.FileName);
            var path1 = Path.Combine("Resources", "Images");
            var path = Path.Combine(path1, "Avatars");

            string userName = TokensHelper.GetClaimFromJwt(context, ClaimTypes.Name);
            string imageName = userName;

            var fullPath = Path.Combine(path, imageName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return true;
        }

        public async Task<byte[]> GetImage(HttpContext context)
        {
            try
            {
                var path1 = Path.Combine("Resources", "Images");
                var path = Path.Combine(path1, "Avatars");

                string userName = TokensHelper.GetClaimFromJwt(context, ClaimTypes.Name);
                string imageName = userName;

                //var fileName = Directory.EnumerateFiles(@path, imageName).FirstOrDefault();

                var fileName = Path.Combine(path, imageName);

                using var memory = new MemoryStream();
                using (var stream = new FileStream(fileName, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                return memory.GetBuffer();
            }
            catch (Exception)
            {
                throw new ErrorException(ErrorCode.ImageNotFound, "Slika nije pronađena.");
            }
        }

        public bool DeleteImage(HttpContext context)
        {
            try
            {
                var path1 = Path.Combine("Resources", "Images");
                var path = Path.Combine(path1, "Avatars");

                string userName = TokensHelper.GetClaimFromJwt(context, ClaimTypes.Name);
                string imageName = userName;

                var fullPath = Path.Combine(path, imageName);

                File.Delete(fullPath);

                return true;
            }
            catch (Exception)
            {
                throw new ErrorException(ErrorCode.ImageNotFound, "Greška pri brisanju slike.");
            }
        }

        public async Task<UserAuthData> ChangePassword(HttpContext context, ChangePassword change)
        {
            string userName = TokensHelper.GetClaimFromJwt(context, ClaimTypes.Name);

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new ErrorException(ErrorCode.UserNotFound, "Korisnik ne postoji u sistemu.");

            var res = await _userManager.ChangePasswordAsync(user, change.OldPassword, change.NewPassword);

            if (!res.Succeeded)
                throw new ErrorException(ErrorCode.PasswordChangeFailed, "Greška pri menjanju lozinke.");

            var role = TokensHelper.GetClaimFromJwt(context, ClaimTypes.Role);
            return await CreateTokens(user, role, true);
        }


        private async Task<UserAuthData> CreateTokens(ApplicationUser user, string userRole, bool rememberMe)
        {
            var authClaims = new List<Claim>
            {
                new Claim(CustomClaims.UserId.ToString(),user.Id),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, userRole),
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
                UserRole = userRole,
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
