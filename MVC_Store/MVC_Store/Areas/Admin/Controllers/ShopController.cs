using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop
        public ActionResult Categories()
        {
            //Обявяваме модел от тип List
            List<CategoryVM> categoryVMList;

            
            using (Db db = new Db())
            {
                //Инициализираме модела на данните
                categoryVMList = db.Categories
                    .ToArray()
                    .OrderBy(x => x.Sorting)
                    .Select(x => new CategoryVM(x))
                    .ToList();

                //Връщаме List към View -то
            }
            return View(categoryVMList);
        }

        // POST: Admin/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            //Обявяваме променлиза тип string ID
            string Id;
            using (Db db = new Db())
            {
                //Проверяваме името на категорията за уникалност
                if (db.Categories.Any(x => x.Name == catName))
                {
                    return "titletaken";
                }

                //Инициализираме модела DTO
                CategoryDTO dto = new CategoryDTO();

                //Добавяме данните в модела
                dto.Name = catName;
                dto.Slug = catName.Replace(" ","-").ToLower();
                dto.Sorting = 100;

                //Запис
                db.Categories.Add(dto);
                db.SaveChanges();

                //Получаваме Id за да го върнем 
                Id = dto.Id.ToString();

            }

            //Връщаме Id  във View
            return Id;
        }

        // POST: Admin/Shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (Db db = new Db())
            {
                //Реализираме начален брояч
                int count = 1;
                //Инициализираме модела за данни
                CategoryDTO dto;

                //Правим сортировка за всяка страница
                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;

                    db.SaveChanges();
                    count++;
                }

            }
        }


        // GET: Admin/Shop/DeleteCategory/id
        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                //Почаваме модела на категорията
                CategoryDTO dto = db.Categories.Find(id);

                //Изтриваме категорияте
                db.Categories.Remove(dto);

                //Записваме изменетияте в базата

                db.SaveChanges();
            }

            //Добавяме съобщение за успешно изтриване
            TempData["SM"] = "You have deleted a category";
            //Преадресираме потребителя
            return RedirectToAction("Categories");
        }

        // POST: Admin/Shop/RenameCategory/id
        [HttpPost]
        public string RenameCategory(string newCatName,int id)
        {
            using (Db db = new Db())
            {

                //Проверка името за уникалност
                if (db.Categories.Any(x => x.Name == newCatName))
                {
                    return "titletaken";
                }
                //Получаваме модел DTO 
                CategoryDTO dto = db.Categories.Find(id);
                //Редактираме модела DTO
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ","-").ToLower();
                //Запис на промените
                db.SaveChanges();

            }

            //Връщаме дума(не е резултат)

            return "ok";
        }
    }
}