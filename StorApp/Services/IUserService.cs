using AspNetIdentityDemo.Api.Models;
using AspNetIdentityDemo.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;
using StorApp.Model.Dtos;
using StorApp.Services.StorApi.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace StorApp.Services
{
    public interface IUserService
    {

        Task<UserResponse> RegisterUserAsync(Register model);

        Task<UserResponse> LoginUserAsync(Login model);


        Task<UserResponse> ConfirmEmailAsync(string userId, string token);

        Task<UserResponse> ForgetPasswordAsync(string email);

        Task<UserResponse> ResetPasswordAsync(ResetPassword model);
    }
}