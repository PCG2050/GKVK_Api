﻿namespace GKVK_Api.Models.DTO
{
    public class RegisterUserDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordPlainText { get; set; }
        public string Role { get; set; }
    }
}
