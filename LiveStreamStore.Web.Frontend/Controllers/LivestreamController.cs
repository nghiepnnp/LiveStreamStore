using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LiveStream.Web.Frontend.Controllers;
using LiveStreamStore.Lib.Models;
using LiveStreamStore.Lib.Models.SearchFilter;
using LiveStreamStore.Lib.Services.Cart;
using LiveStreamStore.Lib.Services.Livestreams;
using LiveStreamStore.Lib.Services.Products;
using LiveStreamStore.Lib.Services.Users;
using LiveStreamStore.Web.Frontend.Filters;

namespace LiveStreamStore.Web.Frontend.Controllers
{
    public class LivestreamController : BaseController
    {
        private readonly ILivestreamService _LiveStreamService;
        private readonly IProductService _ProductService;
        private readonly IUserService _UserStoreService;
        private readonly ICartService _CartService;

        public LivestreamController(ILivestreamService liveStreamService, IProductService productService, IUserService userStoreService, ICartService cartService)
        {
            _LiveStreamService = liveStreamService;
            _ProductService = productService;
            _UserStoreService = userStoreService;
            _CartService = cartService;
        }

        [AuthFilter]
        public IActionResult Index(string StoreCode)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                var check = _UserStoreService.GetUserStoreByStoreCode(StoreCode);
                if (check != null)
                {
                    ViewBag.StoreName = check.Store.Name;
                    return View();
                }
                return RedirectToAction("Error404", "Customer");
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
        }

        [HttpPost]
        public PartialViewResult GetListByStreaming(string timezoneoffset)
        {
            try
            {
                if (_Timezoneoffset == null)
                {
                    SetSessionTimeZone(timezoneoffset);
                }
                ViewBag.CurrentTimeZoneOffSet = _Timezoneoffset;
                string[] Path = (HttpContext.Request.Path).ToString().Split('/');
                var livestreams = _LiveStreamService.GetLiveStreamByStreaming(Path[1]);
                return PartialView("_PartialListLiveStream", livestreams);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return PartialView("_PartialError", ex);
            }
        }

        [HttpPost]
        public PartialViewResult GetProductBeforeLiveStream(int idLivestream, int pageNumber)
        {
            try
            {
                ViewBag.Customer = _Customer;
                if (_Customer != null)
                {
                    string[] Path = (HttpContext.Request.Path).ToString().Split('/');
                    ViewBag.ListCart = _CartService.GetAllListProductByStoreCode(_Customer.Id, Path[1]);
                }
                var products = _ProductService.GetListProductByPage(idLivestream, pageNumber);
                return PartialView("_PartialProductBeforeLiveStream", products);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return PartialView("_PartialError", ex);
            }
        }
        
        [AuthFilter]
        public IActionResult Products(int id)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                return View(_LiveStreamService.GetLiveStreamById(id));
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
        }
    }
}
