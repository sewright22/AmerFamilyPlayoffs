using PlayoffPool.MVC.Controllers;
using PlayoffPool.MVC.Extensions;

namespace PlayoffPool.MVC
{
	public static class Constants
	{
		public static class Controllers
		{
			public static string ACCOUNT = nameof(AccountController).GetControllerNameForUri();
			public static string HOME = nameof(HomeController).GetControllerNameForUri();
		}

		public static class Actions
		{
			public static string LOGIN = nameof(AccountController.Login);
			public static string LOGOUT = nameof(HomeController.LogOut);
			public static string REGISTER = nameof(AccountController.Register);
		}

		public static class Roles
		{
			public static string Admin = "Admin";
			public static string Player = "Player";
    }
	}
}
