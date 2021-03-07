using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveStreamStore.Lib.Models.Enums;
using LiveStreamStore.Lib.Services.WorkContext;

namespace LiveStreamStore.Web.Filters
{
    public class AuthFilter : ActionFilterAttribute, IActionFilter
    {
        private IWorkContext _WorkContext = EngineContext.Resolve<IWorkContext>();
        private readonly IHttpContextAccessor _HttpContextAccessor = EngineContext.Resolve<IHttpContextAccessor>();
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = _WorkContext.CurrentUser ?? _WorkContext.GetUserCookie();

            if (user == null)
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "UserStore" },
                        { "action", "Login" }
                    });
            }                   
        }
    }
}
