﻿namespace likhitan.Models.ClientDto
{
    public class LogoutDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int UserId { get; set; }
    }
}
