using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using SecuritySystemsStore.Data;
using SecuritySystemsStore.Services;
using SecuritySystemsStore.ViewModels.Shop;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace SecuritySystemsStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly ICategoriesService categoriesService;
        private readonly IWebHostEnvironment hostEnvironment;

        public ShopController(ApplicationDbContext db, ICategoriesService categoriesService,IWebHostEnvironment hostEnvironment)
        {
            this.db = db;
            this.categoriesService = categoriesService;
            this.hostEnvironment = hostEnvironment;
        }

        // GET: Admin/Shop
        public async Task<IActionResult> Categories()
        {
            var categoryList = await this.categoriesService.GetAllCategories();

            return this.View(categoryList);
        }

        // POST: Admin/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            var id = this.categoriesService.GetId(catName);

            return id;
        }

        // POST: Admin/Shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            this.categoriesService.ReorderCategories(id);
        }

        // GET: Admin/Shop/DeleteCategory/id
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await this.db.Categories.FindAsync(id);

            this.db.Categories.Remove(category);

            await this.db.SaveChangesAsync();

            TempData["SM"] = "You have deleted a category";

            return this.RedirectToAction("Categories");
        }

        // POST: Admin/Shop/RenameCategory/newCatName,id
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            return this.categoriesService.RenameCategories(newCatName, id);
        }

        // GET: Admin/Shop/AddProduct
        [HttpGet]
        public IActionResult AddProduct()
        {
            var viewModel = this.categoriesService.GetCategoriesList<ProductVM>();

            return this.View(viewModel);
        }

        // POST: Admin/Shop/AddProduct/model,file
        [HttpPost]
        public async Task<IActionResult> AddProductAsync(ProductVM input, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = this.categoriesService.CheckCategoriesList<ProductVM>(input);
                return this.View(viewModel);
            }

            if (db.Products.Any(x => x.Name == input.Name))
            {
                var viewModel = this.categoriesService.CheckCategoriesList<ProductVM>(input);
                ModelState.AddModelError("", "That product name is taken!");
                return this.View(viewModel);
            }

            var id = await this.categoriesService.AddProductAsync(input);

            TempData["SM"] = "You have added a product!";

            #region Upload Image
            var originalDirectory = hostEnvironment.WebRootPath + "\\Images\\Uploads\\";
            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            if (!Directory.Exists(pathString1))
            {
                Directory.CreateDirectory(pathString1);
            }

            if (!Directory.Exists(pathString2))
            {
                Directory.CreateDirectory(pathString2);
            }

            if (!Directory.Exists(pathString3))
            {
                Directory.CreateDirectory(pathString3);
            }

            if (!Directory.Exists(pathString4))
            {
                Directory.CreateDirectory(pathString4);
            }

            if (!Directory.Exists(pathString5))
            {
                Directory.CreateDirectory(pathString5);
            }

            if (file != null && file.Length > 0)
            {
                string ext = file.ContentType.ToLower();

                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    var viewModel = this.categoriesService.CheckCategoriesList<ProductVM>(input);
                    ModelState.AddModelError("", "The image was not uploaded - wrong image extension");
                    return this.View(viewModel);
                }

                string imageName = file.FileName;
                string[] pathArr = imageName.Split('\\');
                imageName = pathArr.Last();

                var product = await this.db.Products.FindAsync(id);

                var path = string.Format($"{pathString2}\\{imageName}");
                var path2 = string.Format($"{pathString3}\\{imageName}");

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                await this.db.SaveChangesAsync();

                using var image = Image.Load(file.OpenReadStream());
                image.Mutate(x => x.Resize(200, 200));
                image.Save(path2);
            }

            #endregion

            return RedirectToAction("AddProduct");
        }
    }
}