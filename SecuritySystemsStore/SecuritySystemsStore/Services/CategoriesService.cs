﻿using Microsoft.EntityFrameworkCore;
using SecuritySystemsStore.Data;
using SecuritySystemsStore.Models;
using SecuritySystemsStore.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecuritySystemsStore.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly ApplicationDbContext db;

        public CategoriesService(ApplicationDbContext db)
        {
            this.db = db;
        }
        public async Task<IEnumerable<CategoryVM>> GetAllCategories()
        {
            var categoryList = db.Categories.OrderBy(x => x.Sorting).Select(x => new CategoryVM
            {
                Id = x.Id,
                Name = x.Name,
                Slug = x.Slug,
                Sorting = x.Sorting,
            }).ToListAsync();

            return await categoryList;
        }

        public string GetId(string catName)
        {
            if (db.Categories.Any(x => x.Name == catName))
            {
                return "titletaken";
            }
            var lastCategory = (db.Categories.Max(x => x.Sorting));

            var category = new Category
            {
                Name = catName,
                Slug = catName.Replace(" ", "-").ToLower(),
                Sorting = lastCategory + 1,
            };

            this.db.AddAsync(category);
            this.db.SaveChanges();

            return category.Id.ToString();           
         }

        public void ReorderCategories(int[] id)
        {
            int count = 1;

            foreach (var categoryId in id)
            {
                var category = this.db.Categories.Find(categoryId);

                category.Sorting = count;

                this.db.SaveChanges();

                count++;
            }
        }
    }
}