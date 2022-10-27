namespace StorApp.Model.UserManager
{
    public class UserDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }      
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public bool IsEmailConfirmed { get; set; }


    }
}
