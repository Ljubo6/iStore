using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecuritySystemsStore.Data;
using SecuritySystemsStore.Models;
using SecuritySystemsStore.Services;
using SecuritySystemsStore.ViewModels.Pages;

namespace SecuritySystemsStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PagesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IPagesService pagesService;

        public PagesController(ApplicationDbContext db,IPagesService pagesService)
        {
            this.db = db;
            this.pagesService = pagesService;
        }

        //GET: Admin/Pages
        public async Task<IActionResult> Index()
        {
            var pageList = await this.pagesService.GetAll();

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

            bool isTitleAddUnique = this.pagesService.IsTitleAddUnique(input);
            bool isSlugAddUnique = this.pagesService.IsSlugAddUnique(input);

            if (isTitleAddUnique)
            {
                ModelState.AddModelError("","That title already exist.");
                return this.View(input);
            }
            else if (isSlugAddUnique)
            {
                ModelState.AddModelError("", "That slug already exist.");
                return this.View(input);
            }

            await this.pagesService.AddPageAsync(input.Title, input.Slug, input.Body, input.HasSidebar);

            TempData["SM"] = "You have added a new page!";

            return this.RedirectToAction("Index");
        }

        // GET: Admin/Pages/EditPage/id
        public async Task<IActionResult> EditPage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //var page = await this.pagesService.GetById(id);
            var page = await this.db.Pages.FindAsync(id);

            if (page == null)
            {
                return Content("The page does not exist.");
            }

            var pageViewModel = new EditPageInputVM
            {
                Title = page.Title,
                Slug = page.Slug,
                Body = page.Body,
                HasSidebar = page.HasSidebar,
            };

            return View(pageViewModel);
        }

        // POST: Admin/Pages/EditPage
        [HttpPost]
        public async Task<IActionResult> EditPage(EditPageInputVM input)
        {
            if (!ModelState.IsValid)
            {
                return this.View(input);
            }
            string slug = this.pagesService.ReturnSlug(input);

            bool isTitleEditUnique = this.pagesService.IsTitleEditUnique(input);
            bool isSlugEditUnique = this.pagesService.IsSlugEditUnique(input,slug);

            if (isTitleEditUnique)
            {
                ModelState.AddModelError("", "That title already exist.");
                return this.View(input);
            }
            else if (isSlugEditUnique)
            {
                ModelState.AddModelError("", "That slug already exist.");
                return this.View(input);
            }

            var newInput = new EditPageVM
            {
                Id = input.Id,
                Title = input.Title,
                Slug = slug,
                Body = input.Body,
                HasSidebar = input.HasSidebar,
                
            };

           
            await this.pagesService.EditPageAsync(newInput);


            TempData["SM"] = "You have edited the page.";

            return RedirectToAction("EditPage");
        }

        // GET: Admin/Pages/PageDetails/id
        public async Task<IActionResult> PageDetails(int id)
        {
            var page = await this.db.Pages.FirstOrDefaultAsync(x => x.Id == id);
            if (page == null)
            {
                return this.Content("The page does not exist.");
            }
            var pageViewModel = new DetailsPageVM
            {
                Id = page.Id,
                Title = page.Title,
                Slug = page.Slug,
                Body = page.Body,
            };

            return View(pageViewModel);
        }
    }
}