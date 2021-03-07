using System;
using System.Collections.Generic;
using System.Text;
using LiveStreamStore.Lib.Data.DBContext.Models;
using Microsoft.AspNetCore.Http;
using LiveStreamStore.Lib.Utilities;
using System.Collections;
using Microsoft.EntityFrameworkCore.Internal;
using LiveStreamStore.Lib.Models.Cart;
using System.Linq;
using LiveStreamStore.Lib.Models;
using LiveStreamStore.Lib.Services.Users;
using LiveStreamStore.Lib.Services.Customers;

namespace LiveStreamStore.Lib.Services.WorkContext
{
    public class WebWorkContext: IWorkContext
    {
        private const string UserSessionName = "Session.User";
        private const string UserCookieName = "Cookie.User";
        private readonly IUserService _UserStoreService;
        private const string CartCookieName = "Cookie.Cart";
        private const string CustomerSessionName = "Session.Customer";
        private const string CustomerCookieName = "Cookie.Customer";
        private const string TimeZoneSessionName = "Session.TimeZone";
        private const string TimeZoneCookieName = "Cookie.TimeZone";

        private readonly IHttpContextAccessor _HttpContextAccessor;
        private ICustomerService _CustomerService;
        public WebWorkContext(IHttpContextAccessor httpContextAccessor,IUserService userStoreService, ICustomerService customerService)
        {
            _HttpContextAccessor = httpContextAccessor;
            _UserStoreService = userStoreService;
            _CustomerService = customerService;
        }
        public LiveStreamStore.Lib.Data.DBContext.Models.User CurrentUser
        {
            get => _HttpContextAccessor.HttpContext.Session.GetObject<LiveStreamStore.Lib.Data.DBContext.Models.User>(UserSessionName);
            set
            {
                if (value != null)
                {
                    _HttpContextAccessor.HttpContext.Session.SetObject(UserSessionName, value);
                    //SetUserCookie();  
                }
                else
                {
                    _HttpContextAccessor.HttpContext.Session.Remove(UserSessionName);
                    RemoveCookie(UserCookieName);
                    RemoveCookie(TimeZoneCookieName);
                }
            }
        }

        public LiveStreamStore.Lib.Data.DBContext.Models.User UserCookie
        {
            get => GetUserCookie();
            set
            {
                SetUserCookie();
            }
        }

        public void SetUserCookie()
        {
            SetCookie(UserCookieName, CurrentUser.FaceBookId + "|" + CurrentUser.FaceBookToken);
            SetCookie(TimeZoneCookieName, CurrentTimeZoneOffSet);
        }

        public LiveStreamStore.Lib.Data.DBContext.Models.User GetUserCookie()
        {
            var cookieValue = GetCookie(UserCookieName);
            if (string.IsNullOrEmpty(cookieValue))
            {
                return null;
            }
            /*if (cookie.Expires < DateTime.UtcNow) //khóa lại vì browser ko gửi expire lên server
            {
                RemoveCookie(UserCookieName);
                return null;
            }*/
            var tmp = cookieValue.Split('|');
            var result = _UserStoreService.LoginWithFacebook(
                new LiveStreamStore.Lib.Data.DBContext.Models.User
                { FaceBookId = tmp.First(), FaceBookToken = tmp.Last() });
            if (result.Code == Error.SUCCESS.Code)
            {
                CurrentUser = result.GetData<LiveStreamStore.Lib.Data.DBContext.Models.User>();
            }
            return CurrentUser;
        }

        //Set và get cookie
        public void SetCookie(string key, string value)
        {
            var option = new CookieOptions
            {
                Expires = string.IsNullOrEmpty(value) ? DateTime.Now.AddMonths(-1) : DateTime.Now.AddHours(24 * 365)
            };
            _HttpContextAccessor.HttpContext.Response.Cookies.Delete(key);
            _HttpContextAccessor.HttpContext.Response.Cookies.Append(key, value, option);
        }

        public string GetCookie(string key)
        {
            return _HttpContextAccessor.HttpContext.Request.Cookies[key];
        }

        public void RemoveCookie(string Name)
        {
            if (_HttpContextAccessor.HttpContext == null) return;
            SetCookie(Name, string.Empty);
        }


        //Set và get cookie list cart
        public void SetCookieCart(int Id, int Quantity, int Limited, int LiveStreamId, string StoreCode)
        {
            var cookieValue = GetCookie(CartCookieName);
            if (string.IsNullOrEmpty(cookieValue))
            {
                SetCookie(CartCookieName, Id + "." + Quantity + "." + Limited + "." + LiveStreamId + "." + StoreCode);
            }
            else
            {
                SetCookie(CartCookieName, cookieValue + "|" + Id + "." + Quantity + "." + Limited + "." + LiveStreamId + "." + StoreCode);
            }
        }

        public void SetCookieCartForDelete(List<ResultCart> resultCarts)
        {
            var value = "";
            for (int i = 0; i < resultCarts.Count; i++)
            {
                value = (value == "") ? resultCarts[i].Id + "." + resultCarts[i].Quantity + "." + resultCarts[i].Limited + "." + resultCarts[i].LiveStreamId + "." + resultCarts[i].StoreCode : value + "|" + resultCarts[i].Id + "." + resultCarts[i].Quantity + "." + resultCarts[i].Limited + "." + resultCarts[i].LiveStreamId + "." + resultCarts[i].StoreCode;
            }
            SetCookie(CartCookieName, value);
        }

        public List<ResultCart> GetCookieCart()
        {
            var cookieValue = GetCookie(CartCookieName);
            if (string.IsNullOrEmpty(cookieValue))
            {
                return null;
            }
            var tmp = cookieValue.Split('|');
            List<int> IdList = new List<int>();
            List<int> QtyList = new List<int>();
            List<int> LimList = new List<int>();
            List<int> LiveList = new List<int>();
            List<string> StoreCodeList = new List<string>();
            List<ResultCart> Result = new List<ResultCart>();
            foreach (var item in tmp)
            {
                var split = item.Split(".");
                var Id = Int32.Parse(split[0]);
                var Quantity = Int32.Parse(split[1]);
                var Limited = Int32.Parse(split[2]);
                var LiveStreamId = Int32.Parse(split[3]);
                var StoreCode = split[4];
                //IdList.IndexOf(Id) tra ve vi tri cua id trong list IdList
                if (IdList.IndexOf(Id) != -1) //ton tai
                {
                    QtyList[IdList.IndexOf(Id)] += Quantity;
                    if (QtyList[IdList.IndexOf(Id)] > Limited)
                    {
                        QtyList[IdList.IndexOf(Id)] = Limited;
                    }
                }
                else
                {
                    IdList.Add(Id);
                    QtyList.Add(Quantity);
                    LimList.Add(Limited);
                    LiveList.Add(LiveStreamId);
                    StoreCodeList.Add(StoreCode);
                }
            }
            for (int i = 0; i < IdList.Count; i++)
            {
                Result.Add(new ResultCart { Id = IdList[i], Quantity = QtyList[i], Limited = LimList[i], LiveStreamId = LiveList[i], StoreCode = StoreCodeList[i] });
            }
            return Result;
        }

        public void DeleteCart(int id)
        {
            var cookieValue = GetCookieCart();
            RemoveCookie(CartCookieName);
            cookieValue.Remove(cookieValue.Where(x => x.Id == id).FirstOrDefault());
            SetCookieCartForDelete(cookieValue);
        }

        public bool checkCookieCart(int ProductId, int Quantity)
        {
            var cookieValue = GetCookieCart();
            if (cookieValue != null)
            {
                for (int i = 0; i < cookieValue.Count; i++)
                {
                    if (cookieValue[i].Id == ProductId && (cookieValue[i].Quantity + Quantity) > cookieValue[i].Limited)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        //Set cookie customer login
        public Customer CurrentCustomer
        {
            get => _HttpContextAccessor.HttpContext.Session.GetObject<Customer>(CustomerSessionName);
            set
            {
                if (value != null)
                {
                    _HttpContextAccessor.HttpContext.Session.SetObject(CustomerSessionName, value);
                }
                else
                {
                    _HttpContextAccessor.HttpContext.Session.Remove(CustomerSessionName);
                    RemoveCookie(CustomerCookieName);
                }
            }
        }

        public Customer CustomerCookie
        {
            get => GetCustomerCookie();
            set
            {
                SetCustomerCookie();
            }
        }

        public virtual string CurrentTimeZoneOffSet
        {
            get
            {
                var timezone = _HttpContextAccessor.HttpContext.Session.GetString(TimeZoneSessionName) ?? GetTimeZoneOffSetCookie();
                return timezone;
            }
            set
            {
                if (value != null)
                {
                    _HttpContextAccessor.HttpContext.Session.SetString(TimeZoneSessionName, value);
                }
                else
                {
                    _HttpContextAccessor.HttpContext.Session.Remove(TimeZoneSessionName);
                }
            }
        }

        public Customer GetCustomerCookie()
        {
            var cookieValue = GetCookie(CustomerCookieName);
            if (string.IsNullOrEmpty(cookieValue))
            {
                return null;
            }
            var tmp = cookieValue.Split('|');
            string[] Path = (_HttpContextAccessor.HttpContext.Request.Path).ToString().Split('/');
            //var result = _CustomerService.Login(tmp.First(), tmp.Last(), Path[1]);
            var result = _CustomerService.LoginWithPhone(tmp.First());
            if (result.Code == Error.SUCCESS.Code)
            {
                CurrentCustomer = result.GetData<Customer>();
            }
            return CurrentCustomer;
        }

        public void SetCustomerCookie()
        {
            //SetCookie(CustomerCookieName, CurrentCustomer.Phone + "|" + CurrentCustomer.Password + "|" + CurrentCustomer.UserStore.StoreCode);
            SetCookie(CustomerCookieName, CurrentCustomer.Phone);
        }

        public string GetTimeZoneOffSetCookie()
        {
            var cookie = GetCookie(TimeZoneCookieName);
            var value = cookie;
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            var timezone = value;
            return timezone;
        }

        public void SetCookieTimeZone()
        {
            SetCookie(TimeZoneCookieName, CurrentTimeZoneOffSet);
        }

    }
}
