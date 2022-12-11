using AmerFamilyPlayoffs.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PlayoffPool.MVC.Helpers
{
    public class ApplicationDataManager : IDataManager
    {
        public ApplicationDataManager(
            AmerFamilyPlayoffContext dataContext, 
            UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager, 
            SignInManager<User> signInManager, 
            IConfiguration configuration)
        {
            this.DataContext = dataContext;
            this.UserManager = userManager;
            this.RoleManager = roleManager;
            this.SignInManager = signInManager;
            this.Configuration = configuration;
        }

        public UserManager<User> UserManager { get; }

        public SignInManager<User> SignInManager { get; }
        public IConfiguration Configuration { get; }
        public AmerFamilyPlayoffContext DataContext { get; }

        public RoleManager<IdentityRole> RoleManager { get; }

        public async virtual Task Seed()
        {
#if DEBUG
            await this.DataContext.Database.EnsureDeletedAsync().ConfigureAwait(false);
#endif
            await this.DataContext.Database.MigrateAsync().ConfigureAwait(false);
            await this.SeedRole(Constants.Roles.Admin).ConfigureAwait(false);
            await this.SeedRole(Constants.Roles.Player).ConfigureAwait(false);
            await this.SeedAdminUser().ConfigureAwait(false);
#if DEBUG
            await this.SeedPlayerUser().ConfigureAwait(false);
#endif
        }

        private async Task SeedAdminUser()
        {
            var seedDataSection = this.Configuration.GetSection("SeedData");
            var adminUser = seedDataSection.GetSection("AdminUser");

            var userToAdd = new User
            {
                UserName = adminUser["Email"],
                Email = adminUser["Email"],
                FirstName = adminUser["FirstName"],
                LastName = adminUser["LastName"],
            };

            var result = await this.UserManager.CreateAsync(userToAdd, adminUser["Password"]).ConfigureAwait(false);

            if (result.Succeeded)
            {
                await this.UserManager.AddToRoleAsync(userToAdd, Constants.Roles.Admin).ConfigureAwait(false);
            }
        }

        private async Task SeedPlayerUser()
        {
            var seedDataSection = this.Configuration.GetSection("SeedData");

            var userToAdd = new User
            {
                UserName = "player@email.com",
                Email = "player@email.com",
                FirstName = "Player",
                LastName = "User",
            };

            var result = await this.UserManager.CreateAsync(userToAdd, "P@ssword!23").ConfigureAwait(false);

            if (result.Succeeded)
            {
                await this.UserManager.AddToRoleAsync(userToAdd, Constants.Roles.Player).ConfigureAwait(false);
            }
        }

        private async Task SeedRole(string role)
        {
            if (await this.RoleManager.RoleExistsAsync(role).ConfigureAwait(false) == false)
            {
                await this.RoleManager.CreateAsync(new IdentityRole(role)).ConfigureAwait(false);
            }
        }
    }
}
