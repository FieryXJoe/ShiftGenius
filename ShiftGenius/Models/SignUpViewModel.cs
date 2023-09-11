using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ShiftGenius.Models
{
    // SignUpViewModel.cs
    public class SignUpViewModel
    {
        public string Token { get; set; }

        [Required(ErrorMessage = "Please enter your name.")]
        [RegularExpression(@"^([a-zA-Z]+)\s([a-zA-Z]+)$", ErrorMessage = "Name must include both a first name and a last name, separated by a space. Special characters and numbers are not allowed.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}