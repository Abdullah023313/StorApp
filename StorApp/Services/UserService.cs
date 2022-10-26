using AspNetIdentityDemo.Api.Models;
using AspNetIdentityDemo.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using StorApp.Model.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace StorApp.Services
{
    public class UserService : IUserService
    {

        private readonly UserManager<IdentityUser> _userManger;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;
        private readonly settings settings;

        public UserService(UserManager<IdentityUser> userManager, IConfiguration configuration, IMailService mailService , settings settings)
        {
            _userManger = userManager;
            _configuration = configuration;
            _mailService = mailService;
            this.settings = settings;
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


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret));

            var token = new JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audience,
                claims: claims,
                expires:settings.expires ,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return token;

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
            //ToDo
            { //var confirmEmailToken = await _userManger.GenerateEmailConfirmationTokenAsync(identityUser);

                //var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                //var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                //string url = $"{_configuration["AppUrl"]}/api/auth/confirmemail?userid={identityUser.Id}&token={validEmailToken}";

                //await _mailService.SendEmailAsync(identityUser.Email, "Confirm your email", $"<h1>Welcome to Auth Demo</h1>" +
                //    $"<p>Please confirm your email by <a href='{url}'>Clicking here</a></p>");
            }

            var token = await CreateJwtToken(identityUser);
            return new UserResponse
            {
                Message = "User created successfully!",
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpireDate = token.ValidTo.ToLongDateString(),
                IsSuccess = true,
            };
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
                Message = "User created successfully!",
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpireDate = token.ValidTo,
                IsSuccess = true,
            };
        }



        public Task<UserResponse> ConfirmEmailAsync(string userId, string token)
        {
            throw new NotImplementedException();
        }

        public Task<UserResponse> ForgetPasswordAsync(string email)
        {
            throw new NotImplementedException();
        }


        public Task<UserResponse> ResetPasswordAsync(ResetPassword model)
        {
            throw new NotImplementedException();
        }


    }
}