using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecuritySystemsStore.Data;
using SecuritySystemsStore.Services;
using SecuritySystemsStore.ViewModels.Pages;

namespace SecuritySystemsStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PagesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IPageService pageService;

        public PagesController(ApplicationDbContext db,IPageService pageService)
        {
            this.db = db;
            this.pageService = pageService;
        }

        //GET: Admin/Pages
        public async Task<IActionResult> Index()
        {
            var pageList = await this.pageService.GetAll();

            return this.View(pageList);
        }

        //GET: Admin/Pages/AddPage
        public IActionResult AddPage()
        {
            return this.View();
        }

        //POST: Admin/Pages/AddPage
        [HttpPost]
        public async Task<IActionResult> AddPageAsync(AddPageVM input)
        {
            if (!ModelState.IsValid)
            {
                return this.View(input);
            }

            if (db.Pages.Any(x => x.Title == input.Title))
            {
                ModelState.AddModelError("","That title already exist.");
                return this.View(input);
            }
            else if (db.Pages.Any(x => x.Slug == input.Slug))
            {
                ModelState.AddModelError("", "That slug already exist.");
                return this.View(input);
            }

            await this.pageService.AddPageAsync(input.Title, input.Slug, input.Body, input.HasSidebar);

            TempData["SM"] = "You have added a new page!";

            return this.RedirectToAction("Index");
        }
    }
}