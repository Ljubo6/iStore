using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.ModelBinding;
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

        // GET: Admin/Shop/AddProduct
        [HttpGet]
        public ActionResult AddProduct()
        {
            //Обявяваме модела на данни
            ProductVM model = new ProductVM();
            //Добавяме списък на категориите от базата в модела
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(),"id","Name");
            }
            //Връщаме модела във View -то

            return View(model);

        }
        // POST: Admin/Shop/AddProduct
        [HttpPost]
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            //Проверяваме модела за валидност
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }
            }

            //Проверяваме името на  модела за уникалност
            using (Db db = new Db())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "This product name is taken");
                    return View(model);
                }
            }

            //Обявяваме променлива ProductID
            int id;

            //Инициализираме и записваме модела на основате на ProductDTO
            using (Db db = new Db())
            {
                ProductDTO product = new ProductDTO();

                product.Name = model.Name;
                product.Slug = model.Name.Replace(" ", "-").ToLower();
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);

                product.CategoryName = catDTO.Name;

                db.Products.Add(product);
                db.SaveChanges();

                id = product.Id;
            }

            //Добавяме съобщение TempDate
            TempData["SM"] = "You have added a product";

            #region Upload Image

            //Създаваме необходимите линкове на директории
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            //Проверяваме наличието на директории(ако няма създаваме)
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

            //Проверяваме, дали е бил файла пуснат
            if (file != null && file.ContentLength > 0)
            {
                //Взимаме разширението на файла
                string ext = file.ContentType.ToLower();
                //Проверяваме резширението на файла
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "The image was not uploaded - wrong image extension");
                        return View(model);
                    }
                }


                //Обявяваме променлива с името на изображението
                string imageName = file.FileName;

                //Записваме името на изображението в модела DTO
                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;
                    db.SaveChanges();
                }

                //Назначаваме пътя към оригиналното и  умаланото изображение
                var path = string.Format($"{pathString2}\\{imageName}");
                var path2 = string.Format($"{pathString3}\\{imageName}");

                //Записваме оригиналното изображение

                file.SaveAs(path);

                //Създаваме и съхрабяваме умалено копие
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200).Crop(1, 1);
                img.Save(path2);

            }
            #endregion

            //Преадресираме потребител

            return RedirectToAction("AddProduct");
        }

    }
}