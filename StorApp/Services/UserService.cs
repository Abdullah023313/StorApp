using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using StorApp.Model;
using StorApp.Model.Dtos;
using StorApp.Model.UserManager;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace StorApp.Services
{
    public class UserService : IUserService
    {

        private readonly UserManager<IdentityUser> _userManger;
        private readonly IMailService _mailService;
        private readonly Settings _settings;
        private readonly StorDbContext _context;

        public UserService(UserManager<IdentityUser> userManager, IMailService mailService, Settings settings, StorDbContext context)
        {
            _userManger = userManager;
            _mailService = mailService;
            _settings = settings;
            _context = context;
        }

        public async Task<JwtSecurityToken> CreateJwtToken(IdentityUser user)
        {
            var userClaims = await _userManger.GetClaimsAsync(user);
            var roles = await _userManger.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(ClaimTypes.GivenName, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: _settings.expires,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return token;

        }

        public async Task<UserResponse> LoginUserAsync(Login model)
        {

            var user = await _userManger.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new UserResponse
                {
                    Message = "There is no user with that Email address",
                    IsSuccess = false,
                };
            }


            var result = await _userManger.CheckPasswordAsync(user, model.Password);
            if (!result)
                return new UserResponse
                {
                    Message = "Invalid password",
                    IsSuccess = false,
                };

            var token = await CreateJwtToken(user);

            return new UserResponse
            {
                Message = "successfully!",
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpireDate = token.ValidTo,
                IsSuccess = true,
            };
        }
        public async Task<UserResponse> RegisterUserAsync(Register model)
        {
            if (model == null)
                throw new NullReferenceException("Reigster Model is null");

            if (model.Password != model.ConfirmPassword)
                return new UserResponse
                {
                    Message = "Confirm password doesn't match the password",
                    IsSuccess = false,
                };

            var identityUser = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Email,

            };

            var result = await _userManger.CreateAsync(identityUser, model.Password);
            if (!result.Succeeded)
            {
                return new UserResponse
                {
                    Message = "User did not create",
                    IsSuccess = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            var confirmEmailToken = await _userManger.GenerateEmailConfirmationTokenAsync(identityUser);

            var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
            var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

            string url = $"{_settings.Issuer}/api/Authentication/confirmemail?userid={identityUser.Id}&token={validEmailToken}";
            //ToDo
            await _mailService.SendEmailAsync(identityUser.Email, "Confirm your email", $"<a href='{url}'>Clicking here</a>");
            return new UserResponse
            {
                Message = $"{confirmEmailToken}",
                IsSuccess = true,
                Token = $"{url}",
            };
        }


        public async Task<UserResponse> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManger.FindByIdAsync(userId);
            if (user == null)
                return new UserResponse
                {
                    IsSuccess = false,
                    Message = "User not found"

                };

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManger.ConfirmEmailAsync(user, normalToken);
            var JwtToken = await CreateJwtToken(user);
            if (result.Succeeded)
                return new UserResponse
                {
                    Message = "Email confirmed successfully!",
                    IsSuccess = true,
                    Token = token,
                    ExpireDate = JwtToken.ValidTo,
                };

            return new UserResponse
            {
                IsSuccess = false,
                Message = "Email did not confirm",
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }




        public async Task<UserResponse> ResetPasswordAsync(ResetPassword model)
        {
            var user = await _userManger.FindByEmailAsync(model.Email);
            if (user == null)
                return new UserResponse
                {
                    IsSuccess = false,
                    Message = "No user associated with email",
                };

            if (model.NewPassword != model.ConfirmPassword)
                return new UserResponse
                {
                    IsSuccess = false,
                    Message = "Password doesn't match its confirmation",
                };

            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManger.ResetPasswordAsync(user, normalToken, model.NewPassword);

            if (result.Succeeded)
                return new UserResponse
                {
                    Message = "Password has been reset successfully!",
                    IsSuccess = true,
                };

            return new UserResponse
            {
                Message = "Something went wrong",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description).ToList(),
            };
        }

        public async Task<List<UserDto>> GetUsers()
        {
            throw new Exception();
        }

     
    }
}