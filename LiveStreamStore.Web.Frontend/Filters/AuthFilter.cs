using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveStreamStore.Lib.Services.Users;
using LiveStreamStore.Lib.Services.WorkContext;

namespace LiveStreamStore.Web.Frontend.Filters
{
    public class AuthFilter : ActionFilterAttribute, IActionFilter
    {
        private IWorkContext _WorkContext = EngineContext.Resolve<IWorkContext>();
        private readonly IHttpContextAccessor _HttpContextAccessor = EngineContext.Resolve<IHttpContextAccessor>();
        private IUserService _UserStoreService = EngineContext.Resolve<IUserService>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var customer = _WorkContext.CurrentCustomer ?? _WorkContext.GetCustomerCookie();
            string[] Path = (context.HttpContext.Request.Path).ToString().Split('/');
            //if (customer != null)
            //{
            //    if (Path[1] != customer.UserStore.StoreCode)
            //    {
            //        context.Result = new RedirectToRouteResult(
            //           new RouteValueDictionary
            //           {
            //                { "controller", "Customer" },
            //                { "action", "Logout" }
            //           });
            //    }
            //}

            var userStore = _UserStoreService.GetUserStoreByStoreCode(Path[1]);
            if (userStore == null)
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Customer" },
                        { "action", "Error404" }
                    });
            }
        }
    }
}
