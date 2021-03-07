using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models;

namespace LiveStreamStore.Lib.Services.Customers
{
    public interface ICustomerService
    {
        List<Customer> GetListCustomerHasInfoByListFacebookId(List<string> listFacebookId);
        ErrorObject CreateCustomer(Customer customer);
        List<Comment> GetCustomerForComment(List<Comment> comments);

        //Dang nhap va dang ky customer
        ErrorObject Login(string Username, string Password, string StoreCode);
        ErrorObject LoginWithPhone(string Phone);
        ErrorObject Insert(Customer customer, string storeCode);
        Customer GetCustomerById(int id);
        ErrorObject CreateOrUpdateCustomer(Customer customer);

        //Profile
        ErrorObject ChangePassword(int id, string oldpassword, string newpassword);
        ErrorObject UpdateProfile(int id, string email, string fullname);
        ErrorObject UpdateCustomerAvatar(int id, IFormFile formFile, Data.DBContext.Models.File file, string domainName, string WebRootPath);

    }
}
