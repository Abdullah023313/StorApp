using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace StorApp.Model.UserManager
{
    public class UserDto
    {
        public IdentityUser User { get; set; }
        public List<Claim> Claims { get; set; }


    }
}
