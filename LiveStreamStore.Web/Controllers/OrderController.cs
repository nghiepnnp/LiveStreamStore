using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LiveStream.Web.Controllers;
using LiveStreamStore.Lib.Models;
using LiveStreamStore.Lib.Models.Api;
using LiveStreamStore.Lib.Models.Enums;
using LiveStreamStore.Lib.Models.SearchFilter;
using LiveStreamStore.Lib.Services.ApiDashBoard;
using LiveStreamStore.Lib.Services.Logs;
using LiveStreamStore.Lib.Services.Orders;

namespace LiveStreamStore.Web.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IApiDashBoard _IApiDashBoard;
        private readonly IOrderService _OrderService;
        private readonly ILogService _LogService;

        public OrderController(IApiDashBoard apiDashBoard, IOrderService orderService, ILogService logService)
        {
            _IApiDashBoard = apiDashBoard;
            _OrderService = orderService;
            _LogService = logService;
        }

        [HttpPost]
        public IActionResult CallApiCreateOrder(int Id)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                var order = _OrderService.GetOrderById(Id);
                List<LiveStreamOrderItem> liveStreamOrderItems = new List<LiveStreamOrderItem>();
                LiveStreamOrderItem liveStreamOrderItem = new LiveStreamOrderItem();
                
                if (order.Status != (int?)EStatusOrder.New)
                {
                    foreach (var item in order.OrderDetail)
                    {
                        liveStreamOrderItem.StoreId = (int)order.StoreId;
                        liveStreamOrderItem.PackageCode = order.Code;
                        liveStreamOrderItem.Phone = order.Customer.Phone;
                        liveStreamOrderItem.Source = _User.Email;
                        liveStreamOrderItem.Description = "test";
                        liveStreamOrderItem.CreatedDate = DateTime.UtcNow;
                        liveStreamOrderItem.LiveStreamDate = (DateTime)order.LiveStream.LivestreamStartTime;
                        liveStreamOrderItem.ModifiedDate = DateTime.UtcNow;
                        liveStreamOrderItem.Status = (int)EStatusOrder.New;

                        liveStreamOrderItem.CategoryName = item.Product.ProductInfo.Category.Name;
                        liveStreamOrderItem.Weight = (decimal)item.Product.Weight;
                        liveStreamOrderItem.Quantity = (int)item.Quantity;
                        liveStreamOrderItem.Price = (decimal)item.Price;
                        liveStreamOrderItem.ProductCode = item.Product.Code;
                        liveStreamOrderItems.Add(liveStreamOrderItem);

                        liveStreamOrderItem = new LiveStreamOrderItem();
                    }

                    error = _IApiDashBoard.ApiCreateListOrder(liveStreamOrderItems);
                    if (error.Code == Error.SUCCESS.Code)
                    {
                        error = _OrderService.UpdateStatusOrder(Id, EStatusOrder.New);
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
                _Log.Info(_LogService.WriteLogFrontend(method, Id));
            }
        }

        [HttpPost]
        public IActionResult CallApiCreateListOrder(int IdLiveStream, int AddressId)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                SearchOrderFilter searchOrderFilter = new SearchOrderFilter();
                searchOrderFilter.IdLiveStream = IdLiveStream;
                searchOrderFilter.AddressId = AddressId;
                var orders = _OrderService.GetListOrderByIdLiveStream(searchOrderFilter);
                List<LiveStreamOrderItem> liveStreamOrderItems = new List<LiveStreamOrderItem>();
                LiveStreamOrderItem liveStreamOrderItem = new LiveStreamOrderItem();
                List<int> ListId = new List<int>();
                foreach (var item in orders)
                {
                    if (item.Status != (int?)EStatusOrder.New)
                    {
                        ListId.Add((int)item.Id);
                        foreach (var orderdetail in item.OrderDetail)
                        {
                            liveStreamOrderItem.StoreId = (int)item.StoreId;
                            liveStreamOrderItem.PackageCode = item.Code;
                            liveStreamOrderItem.Phone = item.CustomerPhone;
                            liveStreamOrderItem.Source = _User.Email;
                            liveStreamOrderItem.Description = "test";
                            liveStreamOrderItem.CreatedDate = DateTime.UtcNow;
                            liveStreamOrderItem.LiveStreamDate = (DateTime)item.LivestreamStartTime;
                            liveStreamOrderItem.ModifiedDate = DateTime.UtcNow;
                            liveStreamOrderItem.Status = (int)EStatusOrder.New;

                            liveStreamOrderItem.CategoryName = orderdetail.Product.ProductInfo.Category.Name;
                            liveStreamOrderItem.Weight = (decimal)orderdetail.Product.Weight;
                            liveStreamOrderItem.Quantity = (int)orderdetail.Quantity;
                            liveStreamOrderItem.Price = (decimal)orderdetail.Price;
                            liveStreamOrderItem.ProductCode = orderdetail.Product.Code;
                            liveStreamOrderItems.Add(liveStreamOrderItem);

                            liveStreamOrderItem = new LiveStreamOrderItem();
                        }
                    }
                }
                
                error = _IApiDashBoard.ApiCreateListOrder(liveStreamOrderItems);
                if (error.Code == Error.SUCCESS.Code)
                {
                    error = _OrderService.UpdateListStatusOrder(ListId, EStatusOrder.New);
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
                _Log.Info(_LogService.WriteLogFrontend(method));
            }
        }
    }
}
