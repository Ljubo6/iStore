using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting.Internal;
using SecuritySystemsStore.Data;
using SecuritySystemsStore.Models;
using SecuritySystemsStore.Services;
using SecuritySystemsStore.ViewModels.Shop;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using X.PagedList;

namespace SecuritySystemsStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly ICategoriesService categoriesService;
        private readonly IWebHostEnvironment hostEnvironment;

        public ShopController(ApplicationDbContext db, ICategoriesService categoriesService, IWebHostEnvironment hostEnvironment)
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

                product.ImageName = imageName;

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

        // GET: Admin/Shop/Products
        public async Task<IActionResult> Products(int? page, int? catId)
        {
            var listOfProducts = await this.categoriesService.GetListOfProducts(catId).ToListAsync();

            var listOfProductVM = this.categoriesService.GetListOfProductsViews(listOfProducts);

            var pageNumber = page ?? 1;

            ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

            ViewBag.SelectedCat = catId.ToString();

            var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, 3);

            ViewBag.onePageOfProducts = onePageOfProducts;

            return View(listOfProductVM);
        }

        // GET: Admin/Shop/EditProduct/id
        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await this.db.Products.FindAsync(id);
            if (product == null)
            {
                return Content("That product does not exist.");
            }

            var viewModel = this.categoriesService.GetAllProducts<ProductVM>(id, product);

            return View(viewModel);
        }

        // GET: Admin/Shop/EditProduct/id
        //[HttpPost]
        public async Task<IActionResult> EditProduct(ProductVM model, IFormFile file, Product product)
        {
            int id = model.Id;

            model = this.categoriesService.FillModel<ProductVM>(id, model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (this.db.Products.Where(x => x.Id != id).Any(x => x.Name == model.Name))
            {
                ModelState.AddModelError("", "That product name is taken!");
                return View(model);
            }

            await this.categoriesService.UploadProduct(model, product);

            TempData["SM"] = "You have edited the product!";

            #region Image Upload

            if (file != null && file.Length > 0)
            {
                string imageName = file.FileName;
                string[] pathArr = imageName.Split('\\');
                string[] extension = pathArr.Last().Split(".");

                imageName = pathArr.Last();

                //string ext = extension.Last().ToLower();
                string ext = file.ContentType.ToLower();

                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    var viewModel = this.categoriesService.CheckCategoriesList<ProductVM>(model);
                    ModelState.AddModelError("", "The image was not uploaded - wrong image extension");
                    return this.View(viewModel);
                }

                var originalDirectory = hostEnvironment.WebRootPath + "\\Images\\Uploads\\";

                var pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
                var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");

                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);

                foreach (var file2 in di1.GetFiles())
                {
                    file2.Delete();
                }

                foreach (var file3 in di2.GetFiles())
                {
                    file3.Delete();
                }

                product = await this.db.Products.FindAsync(id);

                product.ImageName = imageName;


                var path = string.Format($"{pathString1}\\{imageName}");
                var path2 = string.Format($"{pathString2}\\{imageName}");

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

            return RedirectToAction("EditProduct");
        }

        // POST: Admin/Shop/DeleteProduct/id
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await this.categoriesService.Delete(id);

            var originalDirectory = hostEnvironment.WebRootPath + "\\Images\\Uploads\\";

            var pathString = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());

            if (Directory.Exists(pathString))
            {
                Directory.Delete(pathString, true);
            }                

            return RedirectToAction("Products");

        }

        // POST: Admin/Shop/SaveGalleryImages/id
        [HttpPost]
        public void SaveGalleryImages(int id)
        {
            foreach (var filename in Request.Form.Files)
            {
                var fileName = filename.Name;
                IFormFile file = filename;

                if (file != null && file.Length > 0)
                {
                    var originalDirectory = hostEnvironment.WebRootPath + "\\Images\\Uploads\\";

                    string pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
                    string pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

                    var path = string.Format($"{pathString1}\\{file.FileName}");
                    var path2 = string.Format($"{pathString2}\\{file.FileName}");

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyToAsync(stream);
                    }

                    this.db.SaveChangesAsync();

                    using var image = Image.Load(file.OpenReadStream());
                    image.Mutate(x => x.Resize(200, 200));
                    image.Save(path2);
                }

            }
        }


        // POST: Admin/Shop/DeleteImage/id/imageName
        public void DeleteImage(int id, string imageName)
        {
            string fullPath1 = (hostEnvironment.WebRootPath + "/Images/Uploads/Products/" + id.ToString() + "/Gallery/" + imageName);
            string fullPath2 = (hostEnvironment.WebRootPath  + "/Images/Uploads/Products/" + id.ToString() + "/Gallery/Thumbs/" + imageName);

            if (System.IO.File.Exists(fullPath1))
            {
                System.IO.File.Delete(fullPath1);
            }
                
            if (System.IO.File.Exists(fullPath2))
            {
                System.IO.File.Delete(fullPath2);
            }
                
        }
    }
}