using System.ComponentModel.DataAnnotations;

namespace PlayoffPool.MVC.Models
{
	public class LoginViewModel
	{
		[Required] public string Email { get; set; }
		[Required] public string Password { get; set; }
	}
}
