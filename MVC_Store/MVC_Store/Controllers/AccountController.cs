using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MVC_Store.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        // GET: account/create-account
        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        // POST: account/create-account
        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserVM model)
        {
            //Проверяваме входящият модел за валидност

            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }

            //проверяваме съответствието на паролата

            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("","Password do not match!");
                return View("CreateAccount",model);
            }

            using (Db db = new Db())
            {
                //Проверяваме username за уникалност

                if (db.Users.Any(x => x.Username.Equals(model.Username)))
                {
                    ModelState.AddModelError("",$"Username {model.Username} is taken.");
                    model.Username = "";
                    return View("CreateAccount",model);
                }

                //Създаваме екземпляр на класа UserTDO

                UserDTO userDTO = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAdress = model.EmailAdress,
                    Username = model.Username,
                    Password = model.Password
                };

                //Добавяме данни в модел 

                db.Users.Add(userDTO);

                //Съхраняваме данните

                db.SaveChanges();

                //Добавяме роля на потребител

                int id = userDTO.Id;

                UserRoleDTO userRoleDTO = new UserRoleDTO()
                {
                    UserId = id,
                    RoleId = 2
                };

                db.UserRoles.Add(userRoleDTO);
                db.SaveChanges();
            }

            //Записваме съобщение  в TempData

            TempData["SM"] = "You are now registered and can login.";

            //Преадресираме потребителя

            return RedirectToAction("Login");
        }

        // GET: Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            //Проверка за ауторизация на потребителя

            string username = User.Identity.Name;

            //Ако е ауторозован проверяваме го на валидност

            if (!string.IsNullOrEmpty(username))
            {
                return RedirectToAction("user-profile");
            }

            //Връщаме View
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            //Проверяваме модела на валидност

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Проверяваме потребителя на валидност

            bool isValid = false;

            using (Db db = new Db())
            {
                if (db.Users.Any(x => x.Username.Equals(model.Username) && x.Password.Equals(model.Password)))
                {
                    isValid = true;

                    if (!isValid)
                    {
                        ModelState.AddModelError("","Invalid username or password.");
                        return View(model);
                    }
                    else
                    {
                        FormsAuthentication.SetAuthCookie(model.Username,model.RememberMe);
                        return Redirect(FormsAuthentication.GetRedirectUrl(model.Username,model.RememberMe));
                    }
                }
            }

            return View();
        }

        //GET: /account/logout

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult UserNavPartial()
        {
            //Получаваме името на потребителя

            string userName = User.Identity.Name;

            //Обявяваме модела

            UserNavPartialVM model;

            using (Db db = new Db())
            {

                //Получаваме потребителя

                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == userName);

                //Запълваме модела с данни от контекста (DTO)

                model = new UserNavPartialVM()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };
            }

            //Връщаме частично View с модела

            return PartialView(model);
        }

        //GET: /account/user-profile
        [HttpGet]
        [ActionName("user-profile")]
        public ActionResult UserProfile()
        {
            //Получаваме името на потребителя

            string userName = User.Identity.Name;

            //Обявяваме модела

            UserProfileVM model;

            using (Db db = new Db())
            {

                //Получаваме потребителя

                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == userName);

                //Инициализираме нашият модел на данни

                model = new UserProfileVM(dto);

            }

            //Връщаме модела в View
            return View("UserProfile",model);
        }

        //POST: /account/user-profile
        [HttpPost]
        [ActionName("user-profile")]

        public ActionResult UserProfile(UserProfileVM model)
        {
            bool userNameIsChanged = false;

            //Проверяваме модела за валидност

            if (!ModelState.IsValid)
            {
                return View("UserProfile",model);
            }

            //Проверяваме паролата (ако потребителя го променя)

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("","Passwords do not match.");
                    return View("UserProfile",model);
                }
            }

            using (Db db = new Db())
            {
                //Получаваме името на потребителя

                string userName = User.Identity.Name;

                //Проверяваме , дали се сменило името на потребителя

                if (userName != model.Username)
                {
                    userName = model.Username;
                    userNameIsChanged = true;
                }

                //Проверяваме името за уникалност

                if (db.Users.Where(x =>x.Id != model.Id).Any(x => x.Username == userName))
                {
                    ModelState.AddModelError("",$"Username {model.Username} already exist.");
                    model.Username = "";
                    return View("UserProfile",model);
                }

                //Променяме модела на контекста на данните

                UserDTO dto = db.Users.Find(model.Id);

                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.EmailAdress = model.EmailAdress;
                dto.Username = model.Username;

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    dto.Password = model.Password;
                }

                //Съхраняваме промените

                db.SaveChanges();

            }

            //Пускаме съобщение от TempData

            TempData["SM"] = "You have edited your profile!";

            if (!userNameIsChanged)
            {
                //Връщаме View с модела

                return View("UserProfile", model);
            }
            else
            {
                return RedirectToAction("Logout");
            }

           
        }
    }
}