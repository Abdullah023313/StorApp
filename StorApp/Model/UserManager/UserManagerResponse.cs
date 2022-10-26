﻿namespace StorApp.Model.Dtos
{
    public class UserResponse
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public List<string> Errors { get; set; }
        public string Token { get; set; }=string.Empty;
        public string? ExpireDate { get; set; }
    }
}
