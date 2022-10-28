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
using System.Web;

namespace StorApp.Services
{
    public class UserService : IUserService
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMailService _mailService;
        private readonly Settings _settings;
        private readonly StorDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(UserManager<IdentityUser> userManager, IMailService mailService, Settings settings, StorDbContext context, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _mailService = mailService;
            _settings = settings;
            _context = context;
            _logger = logger;
        }

        public async Task<JwtSecurityToken> CreateJwtToken(IdentityUser user)
        {

            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim(ClaimTypes.Role, role));



            var claims = new[]
            {
                new Claim(ClaimTypes.GivenName, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, "SuperAdmin")
            };

            claims.Union(roleClaims);
            claims.Union(userClaims);


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: _settings.expires,
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

            var result = await _userManager.CreateAsync(identityUser, model.Password);
            if (result.Succeeded)
            {
                var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);

                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                string url = $"{_settings.Issuer}/api/Authentication/confirmemail?userid={identityUser.Id}&token={validEmailToken}";
                var body = $"<html><body><p>Please confirm your email by <a href='{url}'>Clicking here</a></p></body></html>";
                await _mailService.SendEmailAsync(identityUser.Email, "Confirm your email", body, "Confirm your email");


                return new UserResponse
                {
                    Message = "User created successfully!",
                    IsSuccess = true,
                };
            }

            return new UserResponse
            {
                Message = "User did not create",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }


        public async Task<UserResponse> LoginUserAsync(Login model)
        {

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || user.EmailConfirmed == false)
            {
                return new UserResponse
                {
                    Message = "There is no user with that Email address",
                    IsSuccess = false,
                };
            }


            var result = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!result)
                return new UserResponse
                {
                    Message = "Invalid password",
                    IsSuccess = false,
                };


            var token = await CreateJwtToken(user);

            return new UserResponse
            {
                Message = $"{new JwtSecurityTokenHandler().WriteToken(token)}",
                IsSuccess = true,
            };
        }


        public async Task<UserResponse> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new UserResponse
                {
                    IsSuccess = false,
                    Message = "User not found"
                };

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ConfirmEmailAsync(user, normalToken);


            if (result.Succeeded)
                return new UserResponse
                {
                    Message = "Email confirmed successfully!",
                    IsSuccess = true,
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
            var user = await _userManager.FindByEmailAsync(model.Email);
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
            var result = await _userManager.ResetPasswordAsync(user, normalToken, model.NewPassword);
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

        public async Task<UserResponse> ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new UserResponse
                {
                    IsSuccess = false,
                    Message = "No user associated with email",
                };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);

            return new UserResponse
            {
                IsSuccess = true,
                Message = validToken
            };
        }

        public async Task<UserResponse> addRole(string rolename)
        {
            bool RoleExists = await _context.Roles.AnyAsync(r => r.NormalizedName == rolename.ToUpper());
            if (!RoleExists)
            {
                _logger.LogInformation($"Adding {rolename} role"); _context.Roles.Add(new IdentityRole()
                {
                    Name = rolename,
                    NormalizedName = rolename.ToUpper()
                });
                _context.SaveChanges();
            }

            return new UserResponse
            {
                IsSuccess = true,
                Message = "Role created successfully!"
            };
        }


        public async Task<UserResponse> updateRole(string oldRolename , string newRolename)
        {
            var Role = await _context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == oldRolename.ToUpper());
            if (Role!=null)
            {
                _logger.LogInformation($"Modify {oldRolename} role");
                Role.Name=newRolename;
                Role.NormalizedName = newRolename.ToUpper();
                _context.Roles.Update(Role);
                _context.SaveChanges();
                return new UserResponse
                {
                    IsSuccess = true,
                    Message = "Role Modify successfully!"
                };
            }
            return new UserResponse
            {
                IsSuccess = false,
                Message = $"Failed Find {oldRolename}!"
            };


        }
        public async Task<UserResponse> addRoleToUser(string email, string rolename)
        {
            bool RoleExists = await _context.Roles.AnyAsync(r => r.NormalizedName == rolename.ToUpper());
            if (!RoleExists)
            {
                _logger.LogInformation($"Adding {rolename} role");
                var result = await addRole(rolename);

                if (!result.IsSuccess)
                {
                    _logger.LogInformation($"Failed to add new Role");
                    return new UserResponse
                    {
                        IsSuccess = false,
                        Message = "Failed to add new Role!"
                    };
                }
            }

            // Select the user
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogInformation($"Failed Find {email}");
                return new UserResponse
                {
                    IsSuccess = false,
                    Message = $"Failed Find {email}!"
                };
            }

            //add the role to the user
            if (!await _userManager.IsInRoleAsync(user, rolename))
            {
                _logger.LogInformation($"Adding {email} to {rolename} role");
                var userResult = await _userManager.AddToRoleAsync(user, rolename);
                if (userResult.Succeeded)
                    return new UserResponse
                    {
                        IsSuccess = true,
                        Message = $"{email} has been added to the role of {rolename}!"
                    };

                _logger.LogInformation($" Failed Add {email} to {rolename} role");
                return new UserResponse
                {
                    IsSuccess = false,
                    Message = $"Failed Add {email} to {rolename} role!"
                };
            }

            return new UserResponse
            {
                IsSuccess = true,
                Message = $"{email} was previously added to the {rolename} role!"
            };
        }
    }
}