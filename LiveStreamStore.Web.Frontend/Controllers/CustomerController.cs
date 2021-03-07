using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using LiveStream.Web.Frontend.Controllers;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models;
using LiveStreamStore.Lib.Models.Api;
using LiveStreamStore.Lib.Services.ApiDashBoard;
using LiveStreamStore.Lib.Services.Cart;
using LiveStreamStore.Lib.Services.Customers;
using LiveStreamStore.Lib.Services.Logs;
using LiveStreamStore.Lib.Services.Users;
using LiveStreamStore.Lib.Utilities;

namespace LiveStreamStore.Web.Frontend.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _CustomerService;
        private readonly ICartService _CartService;
        private readonly IApiDashBoard _IApiDashBoard;
        private readonly IUserService _UserStoreService;
        private readonly ILogService _LogService;

        public CustomerController(ICustomerService customerService, ICartService cartService, IApiDashBoard apiDashBoard, IUserService userStoreService, ILogService logService)
        {
            _CustomerService = customerService;
            _CartService = cartService;
            _IApiDashBoard = apiDashBoard;
            _UserStoreService = userStoreService;
            _LogService = logService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult PartialLogout()
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                if (_Customer != null)
                {
                    var customer = _CustomerService.GetCustomerById(_Customer.Id);
                    return PartialView("_PartialLogout", customer);
                }
                return PartialView("_PartialLogout");
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return PartialView("_PartialError", ex);
            }
        }

        [HttpPost]
        public IActionResult Login(string Phone)
        {
            var response = new ErrorObject(Error.SUCCESS);
            try
            {
                string[] Path = (HttpContext.Request.Path).ToString().Split('/');
                var storeId = _UserStoreService.GetUserStoreByStoreCode(Path[1]);
                response = _IApiDashBoard.ApiGetCustomerInfo(Phone, (int)storeId.StoreId);
                if (response.Code == Error.SUCCESS.Code)
                {
                    var data = response.GetData<ResultGetInfoCustomer>();
                    Customer customer = new Customer();
                    customer.Fullname = data.Fullname;
                    customer.Email = data.Email;
                    customer.Phone = data.Phone;
                    // lay dia chi 
                    var recipientInfo = data.RecipientsInfos.LastOrDefault();
                    Address address = new Address();
                    address.DistrictId = recipientInfo.DistrictId;
                    address.WardId = recipientInfo.WardId;
                    address.StateProvinceId = recipientInfo.CityId;
                    address.Address1 = recipientInfo.Add1;
                    customer.Address.Add(address);
                    response = _CustomerService.CreateOrUpdateCustomer(customer);
                    if (response.Code == Error.SUCCESS.Code)
                    {
                        response = _CustomerService.LoginWithPhone(Phone);
                        if(response.Code == Error.SUCCESS.Code)
                        {
                            _Customer = response.GetData<Customer>();
                            SetCustomerCookie();
                            var list = GetCookieCartByStoreCode();
                            if (list.Count != 0)
                            {
                                _CartService.Remove(_Customer.Id);
                                for (int i = 0; i < list.Count; i++)
                                {
                                    _CartService.Add(_Customer.Id, list[i].LiveStreamId, list[i].Id, list[i].Quantity);
                                }
                            }
                        }
                    }
                }               
                return Json(response);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(response.System(ex.Message));
            }
        }

        [HttpPost]
        public IActionResult CreateOrUpdate(Customer customer, string storeCode)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                if (ModelState.IsValid)
                {
                    if (customer.Id == 0)
                    {
                        error = _CustomerService.Insert(customer, storeCode);
                    }
                }
                else
                {
                    return Json(Error.INVALID_DATA);
                }
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
                _Log.Info(_LogService.WriteLogFrontend(method, customer, storeCode));
            }
        }

        public IActionResult Logout()
        {
            _Customer = null;
            return RedirectToAction("Index", "Livestream");
        }

        public IActionResult Error404()
        {
            return PartialView("Error404");
        }
    }
}
