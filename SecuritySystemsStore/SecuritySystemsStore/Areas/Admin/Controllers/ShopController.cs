using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecuritySystemsStore.Data;
using SecuritySystemsStore.Services;

namespace SecuritySystemsStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly ICategoriesService categoriesService;

        public ShopController(ApplicationDbContext db, ICategoriesService categoriesService)
        {
            this.db = db;

            this.categoriesService = categoriesService;
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

        // POST: Admin/Shop/RenameCategory/id
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            return this.categoriesService.RenameCategories(newCatName,id);
        }
    }
}