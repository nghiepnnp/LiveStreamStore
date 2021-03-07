using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models;
using LiveStreamStore.Lib.Services.Logs;
using LiveStreamStore.Lib.Services.Users;
using LiveStreamStore.Web;
using LiveStreamStore.Web.Filters;

namespace LiveStream.Web.Controllers
{
    public class UserStoreController : BaseController
    {
        private readonly IUserService _UserStoreService;
        private readonly IHostingEnvironment _HostingEnvironment;
        private readonly ILogService _LogService;

        public UserStoreController(IUserService userStoreService, IHostingEnvironment hostingEnvironment, ILogService logService)
        {
            _UserStoreService = userStoreService;
            _HostingEnvironment = hostingEnvironment;
            _LogService = logService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [Route("signinfacebook")]
        public IActionResult SigninFaceBook(string timezoneoffset)
        {
            //RemoveCookieAddress();
            var authProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("CallbackSigninFaceBook", "UserStore", new { timezoneoffset = timezoneoffset })
            };
            return Challenge(authProperties, "Facebook");
        }

        public async Task<ActionResult> CallbackSigninFaceBook(string timezoneoffset)
        {
            try
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                if (token == null)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    var UserClaims = HttpContext.User.Claims;
                    var idFaceBook = UserClaims
                        .FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
                    var email = string.Empty;
                    var claimEmail = UserClaims
                        .FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
                    if (claimEmail != null)
                    {
                        email = UserClaims
                       .FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
                    }
                    var name = UserClaims
                        .FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").Value;
                    var result = _UserStoreService.LoginWithFacebook(
                            new User
                            {
                                FaceBookId = idFaceBook,
                                Email = email,
                                FaceBookToken = token,
                                Fullname = name
                            });
                    if (result.Code == Error.SUCCESS.Code)
                    {
                        SetSessionTimeZone(timezoneoffset);
                        _User = result.GetData<User>();
                        SetUserCookie();
                    }
                    return RedirectToAction("Index", "Livestream");
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                throw ex;
            }

        }

        [AuthFilter]
        public IActionResult Profile()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult PartialProfile()
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                var user = _UserStoreService.GetUserStoreById(_User.Id);
                return PartialView("_PartialProfile", user);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return PartialView("_PartialError", ex);
            }
        }

        [HttpPost]
        public IActionResult UpdateProfile(int id, string fullname, string email, string phone)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                error = _UserStoreService.UpdateProfile(id, fullname, email, phone);
                return Json(error);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
            finally
            {
                var method = MethodBase.GetCurrentMethod();
                _Log.Info(_LogService.WriteLog(method, id, fullname, email, phone));
            }
        }

        [HttpPost]
        public IActionResult ChangeProfileAvatar(int id, IFormFile formFile)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                File file = new File();
                IHttpContextAccessor _HttpContextAccessor = EngineContext.Resolve<IHttpContextAccessor>();
                string domainName = "https://" + _HttpContextAccessor.HttpContext.Request.Host.Value;
                string WebRootPath = _HostingEnvironment.WebRootPath;
                string foldelSave = "UserStores";
                error = _UserStoreService.UpdateUserAvatar(id, formFile, file, domainName, WebRootPath, foldelSave);
                return Json(error);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
            finally
            {
                var method = MethodBase.GetCurrentMethod();
                _Log.Info(_LogService.WriteLog(method, id, formFile));
            }
        }

        public IActionResult Logout()
        {
            _User = null;
            return RedirectToAction("Login", "UserStore");
        }

        [HttpPost]
        public PartialViewResult PartialLogout()
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                User user = null;
                if (_User != null)
                {
                    user = _UserStoreService.GetUserStoreById(_User.Id);
                }
                return PartialView("_PartialLogout", user);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return PartialView("_PartialError", ex);
            }
        }

    }
}
