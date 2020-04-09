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
    }
}