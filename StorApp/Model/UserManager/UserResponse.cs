namespace StorApp.Model.UserManager
{
    public class UserResponse
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}

