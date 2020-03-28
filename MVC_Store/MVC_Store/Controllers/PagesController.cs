using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Controllers
{
    public class PagesController : Controller
    {
        //(Урок 17)
        // GET: Index/page{}
        public ActionResult Index(string page = "")
        {
            //Създаваме умалено име(Slug)
            if (page == "")
            {
                page = "home";
            }
            //Обявяваме модел и клас DTO
            PageVM model;
            PagesDTO dto;

            //Проверяваме дали е достъпна текущата страница
            using (Db db = new Db())
            {
                if (!db.Pages.Any(x => x.Slug.Equals(page)))
                {
                    return RedirectToAction("Index",new { page = ""});
                }
            }
            //Получаваме DTO на странивцата
            using (Db db = new Db())
            {
                dto = db.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }
            //Установяваме заглавието на страницата (Title)

            ViewBag.PageTitle = dto.Title;

            //Проверяваме страничният панел

            if (dto.HasSidebar == true)
            {
                ViewBag.Sidebar = "Yes";
            }
            else
            {
                ViewBag.Sidebar = "No";
            }
            //Запълваме модела с данни

            model = new PageVM(dto);

            //Връщаме View с модела

            return View(model); 
        }

        public ActionResult PagesMenuPartial()
        {
            //Инициализираме List PageVM
            List<PageVM> pageVMList;
            //Получаваме всички страници освен HOME
            using (Db db = new Db())
            {
                pageVMList = db.Pages
                    .ToArray()
                    .OrderBy(x => x.Sorting)
                    .Where(x => x.Slug != "home")
                    .Select(x => new PageVM(x))
                    .ToList();
            }
            //Връщаме частично View с лист от данни
            return PartialView("_PagesMenuPartial", pageVMList);
        }
    }
}