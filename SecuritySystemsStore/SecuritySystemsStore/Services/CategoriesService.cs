using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SecuritySystemsStore.Data;
using SecuritySystemsStore.Models;
using SecuritySystemsStore.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SecuritySystemsStore.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment environment;
        private readonly IMapper mapper;

        public CategoriesService(ApplicationDbContext db,IWebHostEnvironment environment,IMapper mapper)
        {
            this.db = db;
            this.environment = environment;
            this.mapper = mapper;
        }

        public async Task<int> AddProductAsync(ProductVM input)
        {
            
            var category = db.Categories.FirstOrDefault(x => x.Id == input.CategoryId);
            Product product = new Product();

            product.Name = input.Name;
            product.Slug = input.Name.Replace(" ", "-").ToLower();
            product.Description = input.Description;
            product.Price = input.Price;
            product.CategoryId = input.CategoryId;
            product.CategoryName = category.Name;

            await this.db.Products.AddAsync(product);
            await this.db.SaveChangesAsync();

            return product.Id;
        }

        public ProductVM CheckCategoriesList<T>(ProductVM model)
        {
            model.Categories = db.Categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            return model;
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

        public ProductVM GetCategoriesList<T>()
        {
            var model = new ProductVM();

            model.Categories = db.Categories.Select(c => new SelectListItem 
            { 
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            return model;
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

        public IEnumerable<ProductVM> GetListOfProductsViews(int? catId)
        {
            var listOfProductsVM = new List<ProductVM>();
            var listOfProducts = db.Products.ToArray()
            .Where(x => catId == null || catId == 0 || x.CategoryId == catId).ToList();

            foreach (var product in listOfProducts)
            {
                var listView =  mapper.Map<ProductVM>(product);

                listOfProductsVM.Add(listView);
            }

            //var listOfProductVM = db.Products.ToArray()
            //    .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
            //    .Select(x => new ProductVM 
            //    { 
            //        Name = x.Name,
            //        Slug = x.Slug,
            //        Description = x.Description,
            //        Price = x.Price,
            //        CategoryName = x.CategoryName,
            //        CategoryId = x.CategoryId,
            //        ImageName = x.ImageName
            //    })
            //    .ToList();

            return listOfProductsVM;
        }

        public string RenameCategories(string newCatName, int id)
        {
            if (db.Categories.Any(x => x.Name == newCatName))
            { 
                return "titletaken";
            }

            var category = this.db.Categories.Find(id);

            category.Name = newCatName;

            category.Slug = newCatName.Replace(" ", "-").ToLower();

            db.SaveChanges();

            return "OK";
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
