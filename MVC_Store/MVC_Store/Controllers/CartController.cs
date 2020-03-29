using MVC_Store.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Store.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            //Обявяваме list  тип CartVM

            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            //Проверяваме дали кошницата е празна

            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Your cart is empty.";

                return View();
            }

            //Пресмятаме сумата и я записваме във ViewBag 

            decimal total = 0m;

            foreach (var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            //Връщаме лист във View
            return View(cart);
        }

        public ActionResult CartPartial()
        {
            //Обявяваме  модел CartVM

            CartVM model = new CartVM();

            //Обявяваме променлива за количество

            int qty = 0;

            //Обявяваме променлива за цена

            decimal price = 0m;

            //Проверяваме сесия на количката

            if (Session["cart"] != null)
            {
                //Получаваме общото количество на продуктите и цената

                var list = (List<CartVM>)Session["cart"];

                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }
            }
            else
            {
                //Или настройваме количество и цената към 0

                model.Quantity = 0;
                model.Price = 0m;
            }

            //Връщаме частично View с модела

            return PartialView("_CartPartial",model);
        }
    }
}