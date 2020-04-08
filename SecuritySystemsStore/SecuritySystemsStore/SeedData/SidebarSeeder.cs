using SecuritySystemsStore.Data;
using SecuritySystemsStore.Models;
using SecuritySystemsStore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecuritySystemsStore.SeedData
{
    public class SidebarSeeder
    {
        private readonly ApplicationDbContext dbContext;

        public SidebarSeeder(IServiceProvider serviceProvider, ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task SeedDataAsync()
        {
            await SeedSidebarAsync();
        }
        private async Task SeedSidebarAsync()
        {
            var sidebar = await this.dbContext.Sidbar.FindAsync(1);
            if (sidebar != null)
            {
                return;
            }
            await dbContext.Sidbar.AddAsync(new Sidebar
            {
                Body = "Body",
            });
            await this.dbContext.SaveChangesAsync();
        }
    }
}
