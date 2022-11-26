using Microsoft.AspNetCore.Mvc.Rendering;

namespace PlayoffPool.MVC.Models
{
	public class UserModel
	{
		public string? RoleId { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? Email { get; set; }
        public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>();
	}
}
