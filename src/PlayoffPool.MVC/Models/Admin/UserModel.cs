using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PlayoffPool.MVC.Models
{
	public class UserModel
	{
		[Required]
		public string? Id { get; set; }
		public string? RoleId { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? Email { get; set; }

		[Display(Name = "Reset Password")]
		public bool ShouldResetPassword { get; set; }
        public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>();
	}
}
