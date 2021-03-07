using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LiveStream.Web.Frontend.Controllers;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models;
using LiveStreamStore.Lib.Services.Customers;
using LiveStreamStore.Lib.Services.Logs;
using LiveStreamStore.Lib.Utilities;
using LiveStreamStore.Web.Frontend.Filters;

namespace LiveStreamStore.Web.Frontend.Controllers
{
    public class ProfileController : BaseController
    {
        private readonly ICustomerService _CustomerService;
        private readonly IHostingEnvironment _HostingEnvironment;
        private readonly ILogService _LogService;

        public ProfileController(ICustomerService customerService, IHostingEnvironment hostingEnvironment, ILogService logService)
        {
            _CustomerService = customerService;
            _HostingEnvironment = hostingEnvironment;
            _LogService = logService;
        }

        [AuthFilter]
        public IActionResult Index()
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                if (_Customer == null)
                {
                    return RedirectToAction("Error404", "Customer");
                }
                ViewBag.StateProvince = GetAllStateProvince();
                ViewBag.Address = GetAddress(_Customer.Id);
                var tempuser = _CustomerService.GetCustomerById(_Customer.Id);
                return View(tempuser);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
        }

        //[HttpPost]
        //public IActionResult ResetPassword(string oldpassword, string newpassword)
        //{
        //    var error = new ErrorObject(Error.SUCCESS);
        //    try
        //    {
        //        error = _CustomerService.ChangePassword(_Customer.Id, oldpassword.EncryptMd5(), newpassword.EncryptMd5());
        //        return Json(error);
        //    }
        //    catch (Exception ex)
        //    {
        //        _Log.Error(ex);
        //        return Json(error.System(ex));
        //    }
        //}

        [HttpPost]
        public IActionResult UpdateProfile(int id, string fullname, string email)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                error = _CustomerService.UpdateProfile(id, email, fullname);
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
                _Log.Info(_LogService.WriteLogFrontend(method, id, fullname, email));
            }
        }

        public IActionResult ChangeProfileAvatar(int id, IFormFile formFile)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                File file = new File();
                IHttpContextAccessor _HttpContextAccessor = EngineContext.Resolve<IHttpContextAccessor>();
                string domainName = "https://" + _HttpContextAccessor.HttpContext.Request.Host.Value;
                string WebRootPath = _HostingEnvironment.WebRootPath;
                error = _CustomerService.UpdateCustomerAvatar(id, formFile, file, domainName, WebRootPath);
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
                _Log.Info(_LogService.WriteLogFrontend(method, id, formFile));
            }
        }
    }
}
