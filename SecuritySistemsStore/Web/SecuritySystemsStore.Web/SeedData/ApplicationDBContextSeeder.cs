namespace SecuritySystemsStore.Web.SeedData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using SecuritySystemsStore.Data;
    using SecuritySystemsStore.Data.Models;

    public class ApplicationDBContextSeeder
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly ApplicationDbContext dbContext;

        public ApplicationDBContextSeeder(IServiceProvider serviceProvider, ApplicationDbContext dbContext)
        {
            this.userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
            this.roleManager = serviceProvider.GetService<RoleManager<ApplicationRole>>();
            this.dbContext = dbContext;
        }

        public async Task SeedDataAsync()
        {
            await this.SeedUsersAsync();
            await this.SeedRoles();
            await this.SeedUserToRoles();
        }

        private async Task SeedUserToRoles()
        {
            var user = await this.userManager.FindByNameAsync("Ljubo");
            var role = await this.roleManager.FindByNameAsync("Admin");

            var exists = this.dbContext.UserRoles.Any(x => x.UserId == user.Id && x.RoleId == role.Id);

            if (exists)
            {
                return;
            }

            this.dbContext.UserRoles.Add(new IdentityUserRole<string>
            {
                RoleId = role.Id,
                UserId = user.Id,
            });

            await this.dbContext.SaveChangesAsync();
        }

        private async Task SeedRoles()
        {
            var role = await this.roleManager.FindByNameAsync("Admin");

            if (role != null)
            {
                return;
            }

            await this.roleManager.CreateAsync(new ApplicationRole
            {
                Name = "Admin",
            });
        }

        private async Task SeedUsersAsync()
        {
            var user = await this.userManager.FindByNameAsync("Ljubo");

            if (user != null)
            {
                return;
            }

            await this.userManager.CreateAsync(
                new ApplicationUser
            {
                UserName = "Ljubo",
                Email = "Ljubo6@abv.bg",
                EmailConfirmed = true,
            },
                "Ljubo3538");
        }
    }
}
