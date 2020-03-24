using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            //Обявяваме списък за View - PageVM
            List<PageVM> pageList;

            //Инициализираме списък (DB)

            using (Db db = new Db())
            {
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            //Връщаме списък с View

            return View(pageList);
        }

        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }
        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //Проверка на моделите за валидност

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {


                //Обявяваме променлива за кратко описание Slug
                string slug;

                //Инициализираме класа PageDTO

                PagesDTO dto = new PagesDTO();

                //Присвояваме заглавие на модела
                dto.Title = model.Title.ToUpper();

                //Проверка има ли кратко описание и ако не го просвояваме

                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ","-").ToLower();
                }

                //Убеждаваме се ,че заглавието и краткото описание са уникални

                if (db.Pages.Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("","That title already exist.");
                    return View(model);
                }
                else if (db.Pages.Any(x => x.Slug == model.Slug))
                {
                    ModelState.AddModelError("", "That slug already exist.");
                    return View(model);
                }

                //Присвояваме останалите значения на модделите

                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                //Съхранчваме модел в базата данни

                db.Pages.Add(dto);
                db.SaveChanges();
            }

            //Съобщение чрез TempData

            TempData["SM"] = "You have added new page!";

            //Преадресиране на потребителя към метод Idex

            return RedirectToAction("Index");
        }

        // GET: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            //Обявяваме модел PageVM
            PageVM model;

            using (Db db = new Db())
            {
                //Получаваме данни за страницата

                PagesDTO dto = db.Pages.Find(id);

                //Проверяваме дали е достъпна въобще тази страница

                if (dto == null)
                {
                    return Content("The Page does not exist.");
                }

                //Инициализираме модела за данните

                model = new PageVM(dto);
            }
            
            //Връщаме View в модела
            return View(model);
        }

        // POST: Admin/Pages/EditPage
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            //Проверяваме модела за валидност
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {
                //получаваме ID  на стъраницата 
                int id = model.Id;

                //Обявяваме променлива за нашето кратко заглавие , т.е това е локална променлива
                string slug = "home";

                //Получаваме страницата по ID
                PagesDTO dto = db.Pages.Find(id);

                //Присвоявам названието от получените модели в DTO
                dto.Title = model.Title;

                //Проверяваме нашият Slug дали го има и го присвояваме ако е необходимо
                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ","-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ","-").ToLower();
                    }
                }

                //Проверяваме Slug и Title  за уникалност
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("","The title already exist.");
                    return View(model);
                }
                else if (db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "The slug already exist.");
                    return View(model);
                }

                //Записваме останалите значения в клас DTO

                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                //Записваме в базата

                db.SaveChanges();
            }

            //Отмятаме съобщение чрез TempData

            TempData["SM"] = "You have edited the page.";

            //Преадресация на потребител

            return RedirectToAction("EditPage");
        }


        // GET: Admin/Pages/PageDetails/id
        public ActionResult PageDetails(int id)
        {
            //Обявяваме модела PageVM
            PageVM model;

            using (Db db = new Db())
            {
                //Получаваме страницата
                PagesDTO dto = db.Pages.Find(id);

                //Убеждаваме се ,че страницата е достъпна
                if (dto == null)
                {
                    return Content("The page does not exist.");
                }

                //Присвояваме на нашият модел всички  полета от  базата
                model = new PageVM(dto);

            }

            //Връщаме модела във  View - то
            return View(model);
        }

    }
}