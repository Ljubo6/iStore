using MVC_Store.Models.Data;
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

                model.Quantity = qty;
                model.Price = price;
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

        public ActionResult AddToCartPartial(int id)
        {
            //Обявяваме лист ,параметризиран от тим CartVM

            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            //Обявяваме модел CartVM

            CartVM model = new CartVM();

           

            using (Db db = new Db())
            {
                //Получаваме продукта

                ProductDTO product = db.Products.Find(id);

                //Проверяваме дали продукта се намира в кошницата

                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

                //Ако не,то добавяме този продукт

                if (productInCart == null)
                {
                    cart.Add(new CartVM()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });

                }
                //Ако съществува просто трябва да се добави единица продукт 
                else
                {
                    productInCart.Quantity++;
                }
            }

            //получаваме общото количество,цена и добавяме данните в модела

            int qty = 0;
            decimal price = 0m;

            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;
            //Съхраняваме състоянието на кошницата в сесията

            Session["cart"] = cart;

            //Връщаме частично  View
            
            return PartialView("_AddToCartPartial",model);
        }


        //GET: /cart/IncrementProduct
        public JsonResult IncrementProduct(int productId)
        {
            //Обявяваме лист cart

            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {

                //Получаваме модел CartVM от листа

                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                //Добавяме количество с 1

                model.Quantity++;

                //Съхраняваме необходимите данни

                var result = new { qty = model.Quantity, price = model.Price };

                //Връщаме JSON отговор с данните

                return Json(result,JsonRequestBehavior.AllowGet);

            }
        }

        //GET: /cart/DecrementProduct

        public ActionResult DecrementProduct(int productId)
        {
            //Обявяваме лист cart

            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {

                //Получаваме модел CartVM от листа

                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                //Намаляме количество с 1

                if (model.Quantity > 1)
                {
                    model.Quantity--;
                }
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }

                //Съхраняваме необходимите данни

                var result = new { qty = model.Quantity, price = model.Price };

                //Връщаме JSON отговор с данните

                return Json(result, JsonRequestBehavior.AllowGet);

            }
        }

        public void RemoveProduct(int productId)
        {
            //Обявяваме лист cart

            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                //Получаваме модел CartVM от листа

                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                cart.Remove(model);
            }
        }
    }
}