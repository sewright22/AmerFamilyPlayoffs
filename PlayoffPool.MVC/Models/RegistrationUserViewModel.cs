using System.ComponentModel.DataAnnotations;

namespace PlayoffPool.MVC.Models
{
    public class RegistrationUserViewModel
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Please enter your email..")]
        [Display(Name = "Email/Username")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter Password...")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please Enter the Confirm Password...")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
