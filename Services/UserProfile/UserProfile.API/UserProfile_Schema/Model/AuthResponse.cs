﻿namespace UserProfile.API.UserProfile_Schema.Model
{
    public class AuthResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
}