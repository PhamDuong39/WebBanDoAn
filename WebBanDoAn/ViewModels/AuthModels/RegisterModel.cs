﻿using System.ComponentModel.DataAnnotations;

namespace WebBanDoAn.ViewModels.AuthModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
        
        public string? Fullname { get; set; }
        public string? Address { get; set; }
    }
}
