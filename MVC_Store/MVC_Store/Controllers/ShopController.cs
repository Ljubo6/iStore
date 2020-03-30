using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index","Pages");
        }

        public ActionResult CategoryMenuPartial()
        {
            //Обявяваме модел тип List<> CategoryVM
            List<CategoryVM> categoryVMList;
            //Инициализираме модела на данните
            using (Db db = new Db())
            {
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }
            //Връщаме частичното View

            return PartialView("_CategoryMenuPartial", categoryVMList);
        }

        // GET: Shop/Category/name
        public ActionResult Category(string name)
        {
            //Обявяваме списък тип List
            List<ProductVM> productVMList;
            using (Db db = new Db())
            {
                //Получаваме ID категории

                CategoryDTO categoryDTO = db.Categories.Where(x => x.Slug == name).FirstOrDefault();

                int catId = categoryDTO.Id;

                //Инициализираме списък с данните

                productVMList = db.Products.ToArray().Where(x => x.CategoryId == catId).Select(x => new ProductVM(x)).ToList();

                //Получаваме име на категориите

                var productCat = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();

                //Правим проверка за null

                if (productCat == null)
                {
                    var catName = db.Categories.Where(x => x.Slug == name).Select(x => x.Name).FirstOrDefault();

                    ViewBag.CategoryName = catName;
                }
                else
                {
                    ViewBag.CategoryName = productCat.CategoryName;
                }

            }

            //Връщаме View с модела

            return View(productVMList);
        }

        // GET: Shop/product-details/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            //Обявяваме модел DTO  и VM

            ProductDTO dto;
            ProductVM model;

            //Инициализираме Id на продукта

            int id = 0;

            using (Db db = new Db())
            {

                //Проверяваме продукта на достъпност

                if (!db.Products.Any(x => x.Slug.Equals(name)))
                {
                    return RedirectToAction("Index","Shop");
                }

                //Инициализираме модела DTO даннии

                dto = db.Products.Where(x => x.Slug == name).FirstOrDefault();

                //Получаваме Id

                id = dto.Id;

                //Инициализираме модела VM данни

                model = new ProductVM(dto);

            }
            //Получаваме изобравение от галерията

            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));

            //Връщаме модела във View

            return View("ProductDetails",model);
        }
    }
}