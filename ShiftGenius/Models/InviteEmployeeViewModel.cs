using System.ComponentModel.DataAnnotations;

namespace ShiftGenius.Models
{
    public class InviteEmployeeViewModel
    {
        [Required(ErrorMessage = "Please enter an email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string EmailAddress { get; set; }
    }
}
