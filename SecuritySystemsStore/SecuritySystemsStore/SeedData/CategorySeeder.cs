using SecuritySystemsStore.Data;
using SecuritySystemsStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecuritySystemsStore.SeedData
{
    public class CategorySeeder
    {
        private readonly ApplicationDbContext dbContext;

        public CategorySeeder(IServiceProvider serviceProvider, ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task SeedDataAsync()
        {
            await SeedSecyritySystemsAsync();
            await SeedFireSystemsAsync();
            await SeedAccessControlSystemsAsync();
        }

        private async Task SeedAccessControlSystemsAsync()
        {
            var category = this.dbContext.Categories.FirstOrDefault(x => x.Name == "AccessControlSystems");
            if (category != null)
            {
                return;
            }
            await dbContext.Categories.AddAsync(new Category
            {
                Name = "AccessControlSystems",
                Slug = "centaur",
                Sorting = 3
            });
            await this.dbContext.SaveChangesAsync();
        }

        private async Task SeedFireSystemsAsync()
        {
            var category = this.dbContext.Categories.FirstOrDefault(x => x.Name == "FireSystems");
            if (category != null)
            {
                return;
            }
            await dbContext.Categories.AddAsync(new Category
            {
                Name = "FireSystems",
                Slug = "kentec",
                Sorting = 2
            });
            await this.dbContext.SaveChangesAsync();
        }

        private async Task SeedSecyritySystemsAsync()
        {
            var category = this.dbContext.Categories.FirstOrDefault(x => x.Name == "SecuritySystems");
            if (category != null)
            {
                return;
            }
            await dbContext.Categories.AddAsync(new Category
            {
                Name = "SecuritySystems",
                Slug = "paradox",
                Sorting = 1
            });
            await this.dbContext.SaveChangesAsync();
        }
    }
}
