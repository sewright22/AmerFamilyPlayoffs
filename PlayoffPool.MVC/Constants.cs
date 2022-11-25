using PlayoffPool.MVC.Controllers;
using PlayoffPool.MVC.Extensions;

namespace PlayoffPool.MVC
{
	public static class Constants
	{
		public static class Controllers
		{
			public static string ACCOUNT = nameof(AccountController).GetControllerNameForUri();
		}

		public static class Actions
		{
			public static string LOGIN = nameof(AccountController.Login);
			public static string REGISTER = nameof(AccountController.Register);
		}
	}
}
