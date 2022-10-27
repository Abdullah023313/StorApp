using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StorApp.Model.UserManager
{
    public class ResetPassword
    {

        public string Token { get; set; }


        [EmailAddress]
        public string Email { get; set; }


        [StringLength(50, MinimumLength = 5)]
        public string NewPassword { get; set; }


        [StringLength(50, MinimumLength = 5)]
        public string ConfirmPassword { get; set; }
    }
}
