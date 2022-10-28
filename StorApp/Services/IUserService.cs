using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;
using StorApp.Model.Dtos;
using StorApp.Model.UserManager;
using StorApp.Services.StorApi.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace StorApp.Services
{
    public interface IUserService
    {
        Task<JwtSecurityToken> CreateJwtToken(IdentityUser user);

        Task<UserResponse> RegisterUserAsync(Register model);

        Task<UserResponse> LoginUserAsync(Login model);


        Task<UserResponse> ConfirmEmailAsync(string userId, string token);

        Task<UserResponse> ResetPasswordAsync(ResetPassword model);

        Task<UserResponse> ForgetPasswordAsync(string email);
        Task<UserResponse> addRole(string rolename);
        Task<UserResponse> updateRole(string oldRolename, string newRolename);

        Task<UserResponse> addRoleToUser(string email, string rolename);

    }
}