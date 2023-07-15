using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PlayoffPool.MVC.Extensions
{
	public static class StringExtensions
	{
		public static string GetControllerNameForUri(this string controllerName)
		{
			return controllerName.Replace("Controller", string.Empty);
		}
	}
}
