using AmerFamilyPlayoffs.Data;
using Microsoft.AspNetCore.Identity;

namespace PlayoffPool.MVC.Helpers
{
    public interface IDataManager
    {
        UserManager<User> UserManager { get; }
        SignInManager<User> SignInManager { get; }
        AmerFamilyPlayoffContext DataContext { get; }
        RoleManager<IdentityRole> RoleManager { get; }
        Task Seed();
    }
}
