namespace PlayoffPool.MVC.Models.Admin
{
	public class UserModel
	{
		public string? RoleId { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? Email { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
	}
}
