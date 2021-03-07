using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LiveStream.Web.Frontend.Controllers;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models;
using LiveStreamStore.Lib.Services.Cart;
using LiveStreamStore.Lib.Services.Livestreams;
using LiveStreamStore.Lib.Services.Logs;
using LiveStreamStore.Lib.Services.Orders;
using LiveStreamStore.Lib.Services.Products;
using LiveStreamStore.Web.Frontend.Filters;

namespace LiveStreamStore.Web.Frontend.Controllers
{
    public class OrderController : BaseController
    {
        private readonly ILivestreamService _LiveStreamService;
        private readonly IOrderService _OrderService;
        private readonly ICartService _CartService;
        private readonly IProductService _ProductService;
        private readonly ILogService _LogService;

        public OrderController(ILivestreamService liveStreamService, IOrderService orderService, ICartService cartService, IProductService productService, ILogService logService)
        {
            _LiveStreamService = liveStreamService;
            _OrderService = orderService;
            _CartService = cartService;
            _ProductService = productService;
            _LogService = logService;
        }

        [AuthFilter]
        public IActionResult History()
        {
             var error = new ErrorObject(Error.SUCCESS);
            try
            {
                if (_Customer == null)
                {
                    return RedirectToAction("Error404", "Customer");
                }
                return View(_OrderService.GetOrderTempByCustomer(_Customer.Id, 0));
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
        }

        [HttpPost]
        public PartialViewResult PartialListOrderHistory(int pageNumber)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                var orderTemps = _OrderService.GetOrderTempByCustomer(_Customer.Id, pageNumber);
                ViewBag.CurrentTimeZoneOffSet = _Timezoneoffset;
                return PartialView("_PartialListOrderHistory", orderTemps);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return PartialView("_PartialError", ex);
            }
        }

        [HttpPost]
        public PartialViewResult PartialMobileListOrderHistory(int pageNumber)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                var orderTemps = _OrderService.GetOrderTempByCustomer(_Customer.Id, pageNumber);
                return PartialView("_PartialMobileListOrderHistory", orderTemps);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return PartialView("_PartialError", ex);
            }
        }

        [HttpPost]
        public IActionResult CreateOrder(List<OrderDetail> createOrderItem, List<OrderTemp> createOrderTemp)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                if (createOrderItem.Any())
                {
                    for (int i = 0; i < createOrderItem.Count; i++)
                    {
                        var check = _CartService.CheckCartOrder(_Customer.Id, (int)createOrderItem[i].ProductId, (int)createOrderItem[i].Quantity);
                        if (check.Code != error.Code)
                        {
                            var product = _ProductService.GetProductById((int)createOrderItem[i].ProductId);
                            return Json(new { check, product.ProductInfo.Name });
                        }
                    }
                    for (int i = 0; i < createOrderItem.Count; i++)
                    {
                        var product = _ProductService.GetProductById((int)createOrderItem[i].ProductId);
                        createOrderItem[i].Price = product.Price;
                        error = _OrderService.CreateOrderDetail(createOrderItem[i]);
                        _OrderService.AddQuantityToProduct(createOrderItem[i].ProductId, createOrderItem[i].Quantity);
                        _CartService.AddQuantityToCart(_Customer.Id, createOrderItem[i].ProductId, createOrderItem[i].Quantity);
                        if (error.Code == Error.SUCCESS.Code)
                        {
                            createOrderTemp[i].OrderDetailId = error.GetData<OrderDetail>().Id;
                            error = _OrderService.CreateOrderTemp(createOrderTemp[i]);
                        }
                    }
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
                _Log.Info(_LogService.WriteLogFrontend(method, createOrderItem, createOrderTemp));
            }
        }
    }
}
