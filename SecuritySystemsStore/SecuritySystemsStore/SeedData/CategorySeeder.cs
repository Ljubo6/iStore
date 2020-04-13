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
            var category = this.dbContext.Categories.FirstOrDefault(x => x.Name == "Access Control");
            if (category != null)
            {
                return;
            }
            await dbContext.Categories.AddAsync(new Category
            {
                Name = "Access Control",
                Slug = "access-control",
                Sorting = 3
            });
            await this.dbContext.SaveChangesAsync();
        }

        private async Task SeedFireSystemsAsync()
        {
            var category = this.dbContext.Categories.FirstOrDefault(x => x.Name == "Fire Systems");
            if (category != null)
            {
                return;
            }
            await dbContext.Categories.AddAsync(new Category
            {
                Name = "Fire Systems",
                Slug = "fire-systems",
                Sorting = 2
            });
            await this.dbContext.SaveChangesAsync();
        }

        private async Task SeedSecyritySystemsAsync()
        {
            var category = this.dbContext.Categories.FirstOrDefault(x => x.Name == "Alarm Systems");
            if (category != null)
            {
                return;
            }
            await dbContext.Categories.AddAsync(new Category
            {
                Name = "Alarm Systems",
                Slug = "alarm-systems",
                Sorting = 1
            });
            await this.dbContext.SaveChangesAsync();
        }
    }
}
