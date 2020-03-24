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
                categoryVMList = db.Caregories
                    .ToArray()
                    .OrderBy(x => x.Sorting)
                    .Select(x => new CategoryVM(x))
                    .ToList();

                //Връщаме List към View -то
            }
            return View(categoryVMList);
        }
    }
}