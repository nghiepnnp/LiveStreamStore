using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models.Cart;
using LiveStreamStore.Lib.Services.Geos;
using LiveStreamStore.Lib.Services.Livestreams;
using LiveStreamStore.Lib.Services.WorkContext;
using LiveStreamStore.Web.Frontend;

namespace LiveStream.Web.Frontend.Controllers
{
    public class BaseController : Controller
    {
        protected static readonly log4net.ILog _Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected IWorkContext _WorkContext = EngineContext.Resolve<IWorkContext>();
        protected IAddressService _AddressService = EngineContext.Resolve<IAddressService>();
        protected ILivestreamService _LivestreamService = EngineContext.Resolve<ILivestreamService>();
        protected ILocationAddressService _LocationAddressService = EngineContext.Resolve<ILocationAddressService>();

        public Customer _Customer { get => _WorkContext.CurrentCustomer; set => _WorkContext.CurrentCustomer = value; }
        public Customer _CustomerCookie { get => _WorkContext.CustomerCookie; set => _WorkContext.CustomerCookie = value; }

        public void SetCustomerCookie()
        {
            _WorkContext.SetCustomerCookie();
        }

        //Shopping Cart
        public void SetCookieCart(int Id, int Quantity, int Limited, int LiveStreamId, string StoreCode)
        {
            _WorkContext.SetCookieCart(Id, Quantity, Limited, LiveStreamId, StoreCode);
        }

        public List<ResultCart> GetCookieCartByStoreCode()
        {
            string[] Path = (HttpContext.Request.Path).ToString().Split('/');
            var listCart = _WorkContext.GetCookieCart();
            List<ResultCart> resultCarts = new List<ResultCart>();
            if (listCart != null)
            {
                foreach (var item in listCart)
                {
                    var livestream = _LivestreamService.GetLiveStreamById(item.LiveStreamId);
                    if (item.StoreCode == Path[1])
                    {
                        if (livestream.OpenPreSale == true)
                        {
                            resultCarts.Add(item);
                        }
                        else
                        {
                            DeleteCart(item.Id);
                        }
                    }
                }
            }
            return resultCarts;
        }

        public void DeleteCart(int Id)
        {
            _WorkContext.DeleteCart(Id);
        }

        public void RemoveCookie(string Name)
        {
            _WorkContext.RemoveCookie(Name);
        }

        //Ham dung chung
        public Address GetAddress(int id)
        {
            return _AddressService.GetAddressByCustomerId(id);
        }

        public List<StateProvince> GetAllStateProvince()
        {
            return _LocationAddressService.GetAllStateProvince().ToList();
        }

        public string _Timezoneoffset
        {
            get
            {
                return _WorkContext.CurrentTimeZoneOffSet;
            }
            set { _WorkContext.CurrentTimeZoneOffSet = value; }
        }

        public string GetCurrentTimeZone()
        {
            try
            {
                return _WorkContext.CurrentTimeZoneOffSet;
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                throw;
            }
        }

        public void SetSessionTimeZone(string timezoneoffset)
        {
            try
            {
                _WorkContext.CurrentTimeZoneOffSet = timezoneoffset;
                _WorkContext.SetCookieTimeZone();
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                throw;
            }
        }

    }
}
