using System;
using System.Collections.Generic;
using System.Text;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models.Cart;

namespace LiveStreamStore.Lib.Services.WorkContext
{
    public interface IWorkContext
    {
        LiveStreamStore.Lib.Data.DBContext.Models.User CurrentUser { get; set; }
        LiveStreamStore.Lib.Data.DBContext.Models.User GetUserCookie();
        void SetUserCookie();
        LiveStreamStore.Lib.Data.DBContext.Models.User UserCookie { get; set; }

        //Cookie Cart
        void SetCookieCart(int Id, int Quantity, int Limited, int LiveStreamId, string StoreCode);
        List<ResultCart> GetCookieCart();
        void DeleteCart(int id);
        void SetCookieCartForDelete(List<ResultCart> resultCarts);
        bool checkCookieCart(int ProductId, int Quantity);
        void RemoveCookie(string Name);

        //Set va get customer cookie
        Customer CurrentCustomer { get; set; }
        Customer GetCustomerCookie();
        void SetCustomerCookie();
        Customer CustomerCookie { get; set; }

        // TimeZone
        string CurrentTimeZoneOffSet { get; set; }
        string GetTimeZoneOffSetCookie();
        void SetCookieTimeZone();
    }
}
