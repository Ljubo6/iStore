using MVC_Store.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MVC_Store
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //Създаваме мотод за обработка на запитване за аутентификация

        protected void Application_AuthenticateRequest()
        {
            //Проверяваме  дали потребителя е ауторизиран

            if (User == null)
            {
                return;
            }

            //Полуяаваме името на потребителя

            string userName = Context.User.Identity.Name;

            //Декларираме масива на ролите

            string[] roles = null;

            using (Db db = new Db())
            {

                //Запълваме ролите
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == userName);

                if (dto == null)
                {
                    return;
                }

                roles = db.UserRoles.Where(x => x.UserId == dto.Id).Select(x => x.Role.Name).ToArray();

            }

            //Създаваме обект интерфейс IPrincipal

            IIdentity userIdentity = new GenericIdentity(userName);
            IPrincipal newUserObj = new GenericPrincipal(userIdentity,roles);

            //Декларираме и инициализираме данните Context.User

            Context.User = newUserObj;

        }
    }
}
