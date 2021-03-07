using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models;
using LiveStreamStore.Lib.Services.Caching;
using LiveStreamStore.Lib.Services.Geos;
using LiveStreamStore.Lib.Utilities;

namespace LiveStreamStore.Lib.Services.Customers
{
    public class CustomerServices : ICustomerService
    {
        private ICacheService _CacheService;
        private IAddressService _AddressService;
        public CustomerServices(ICacheService cacheService, IAddressService addressService)
        {
            _CacheService = cacheService;
            _AddressService = addressService;
        }
        public List<Customer> GetListCustomerHasInfoByListFacebookId(List<string> listFacebookId)
        {
            try
            {
                var result = new List<Customer>();
                result = GetAllCustomer().Where(x => listFacebookId.Contains(x.FaceBookId)).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Comment> GetCustomerForComment(List<Comment> comments)
        {
            try
            {
                var listFacebookId = comments.Select(x => x.FaceBookId).Distinct().ToList();
                var listCustomer = GetAllCustomer().Where(x => listFacebookId.Contains(x.FaceBookId)).ToList();
                foreach (var item in comments)
                {
                    for (int i = 0; i < listCustomer.Count; i++)
                    {
                        if (item.FaceBookId == listCustomer[i].FaceBookId)
                        {
                            item.HasAddress = listCustomer[i].Address.Any();
                            item.Phone = item.Phone ?? listCustomer[i].Phone;
                            item.TotalCancellations = listCustomer[i].TotalCancellations ?? 0;
                            item.TotalPurchased = listCustomer[i].TotalPurchased ?? 0;
                        }
                    }
                }
                return comments;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Customer> GetAllCustomer()
        {
            var result = new List<Customer>();

            return _CacheService.Get(CacheKeyName.AllCustomer, () =>
            {
                using (var context = new LiveStreamStoreContext())
                {
                    result = context.Customer.Include(y => y.Address).ToList();
                }
                return result;
            });
        }
        public void ClearCustomerCacheKey()
        {
            _CacheService.RemoveByPrefix(CacheKeyName.CustomerPrefix);
        }
        public ErrorObject CreateCustomer(Customer customer)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var cus = context.Customer.FirstOrDefault(x => x.FaceBookId == customer.FaceBookId || x.Phone == customer.Phone);
                    if (cus != null) // khach hang cu mua livestream
                    {
                        if (cus.Phone != customer.Phone)
                        {
                            cus.Phone = customer.Phone;
                            if (context.SaveChanges() > 0)
                            {
                                List<Customer> customers = _CacheService.Get<List<Customer>>(CacheKeyName.AllCustomer);
                                if (customers != null)
                                {
                                    customers.FirstOrDefault(x => x.Id == cus.Id).Phone = cus.Phone;
                                    _CacheService.Remove(CacheKeyName.AllCustomer);
                                    _CacheService.Set(CacheKeyName.AllCustomer, customers);
                                }
                                error.SetData(customer);
                            };

                        }
                        error.SetData(cus);
                    }
                    else
                    { // khach hang moi
                        context.Customer.Add(customer);
                        if (context.SaveChanges() > 0)
                        {
                            List<Customer> customers = _CacheService.Get<List<Customer>>(CacheKeyName.AllCustomer);
                            if (customers != null)
                            {
                                customers.Add(customer);
                                _CacheService.Remove(CacheKeyName.AllCustomer);
                                _CacheService.Set(CacheKeyName.AllCustomer, customers);
                            }
                            error.SetData(customer);
                        }
                    }
                }
                return error;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //Dang nhap va dang ky customer
        public ErrorObject Login(string Username, string Password, string StoreCode)
        {
            var error = Error.Success();
            try
            {
                using var db = new LiveStreamStoreContext();
                var customer = db.Customer.Where(x => (x.Phone == Username) && x.Password.Equals(Password) && x.User.Store.Code == StoreCode).Include(x => x.User).FirstOrDefault();
                if (customer == null)
                {
                    return Error.CUSTOMER_INVALID;
                }
                customer.User.Customer = null;
                return error.SetData(customer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ErrorObject LoginWithPhone(string Phone)
        {
            var error = Error.Success();
            try
            {
                using var db = new LiveStreamStoreContext();
                var customer = db.Customer.Where(x => (x.Phone == Phone)).FirstOrDefault();
                if (customer == null)
                {
                    return Error.CUSTOMER_INVALID;
                }
                return error.SetData(customer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ErrorObject CreateOrUpdateCustomer(Customer customer)
        {
            var error = Error.Success();
            try
            {
                using (var db = new LiveStreamStoreContext())
                {
                    var cus = db.Customer.Where(x => (x.Phone == customer.Phone)).Include(x => x.Address).FirstOrDefault();
                    if (cus == null) // create
                    {
                        db.Add(customer);
                        return db.SaveChanges() > 0 ? error : error.Failed("Failed");
                    }
                    else
                    { // update      
                        if (cus.Email != customer.Email || cus.Fullname != customer.Fullname) // check profile thay đổi
                        {
                            var err = UpdateProfile(cus.Id, customer.Email, customer.Fullname);
                            if (err.Code == Error.FAILED.Code)
                            {
                                return err.SetMessage("Update profile failed");
                            }
                        }
                        var addOld = cus.Address?.LastOrDefault();
                        var addNew = customer.Address?.LastOrDefault();

                        if (addOld.Address1 != addNew.Address1
                            || addOld.DistrictId != addNew.DistrictId
                            || addOld.WardId != addNew.WardId
                            || addOld.StateProvinceId != addNew.StateProvinceId) // check địa chỉ thay đổi
                        {
                            addOld.Address1 = addNew.Address1;
                            addOld.DistrictId = addNew.DistrictId;
                            addOld.WardId = addNew.WardId;
                            addOld.StateProvinceId = addNew.StateProvinceId;
                            error = _AddressService.UpdateAddress(addOld);
                        }
                        return error;
                        //cus.Address.
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ErrorObject Insert(Customer customer, string storeCode)
        {
            var error = Error.Success();

            using (var context = new LiveStreamStoreContext())
            {
                customer.Password = customer.Password.EncryptMd5();
                customer.CreatedDateUtc = DateTime.Now;

                var userStore = context.User.FirstOrDefault(x => x.Store.Code == storeCode);
                customer.UserId = userStore.Id;

                if (context.Customer.Any(x => x.Phone == customer.Phone && x.UserId == customer.UserId))
                {
                    return Error.PHONE_EXISTED;
                }
                context.Customer.Add(customer);
                return context.SaveChanges() > 0 ? error : error.Failed("Create customer failed");
            }
        }

        public Customer GetCustomerById(int id)
        {
            using (var context = new LiveStreamStoreContext())
            {
                var a = context.Customer.Where(x => x.Id == id).Include(x => x.Avatar).Include(x => x.Address).FirstOrDefault();
                return a;
            }
        }

        //Profile
        public ErrorObject ChangePassword(int id, string oldpassword, string newpassword)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var cus = context.Customer.FirstOrDefault(x => x.Id == id);
                    if (oldpassword != cus.Password)
                    {
                        return Error.PASSWORD_INCORRECT;
                    }
                    cus.ModifiedDateUtc = DateTime.UtcNow;
                    cus.Password = newpassword;
                    context.Customer.Update(cus);
                    return context.SaveChanges() > 0 ? error : error.Failed("ChangePassword Customer failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public ErrorObject UpdateProfile(int id, string email, string fullname)
        {
            var error = Error.Success();
            using (var context = new LiveStreamStoreContext())
            {
                var cus = context.Customer.FirstOrDefault(x => x.Id == id);
                cus.Fullname = fullname;
                cus.Email = email;
                cus.ModifiedDateUtc = DateTime.Now;
                context.Customer.Update(cus);
                return context.SaveChanges() > 0 ? error : error.Failed("Update Customer failed");
            }
        }

        public ErrorObject UpdateCustomerAvatar(int id, IFormFile formFile, Data.DBContext.Models.File file, string domainName, string WebRootPath)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var cus = context.Customer.FirstOrDefault(x => x.Id == id);
                    var phone = cus.Phone;
                    file = context.File.FirstOrDefault(x => x.Id == cus.AvatarId);
                    if (formFile != null)
                    {
                        if (file == null)
                        {
                            file = new Data.DBContext.Models.File();
                        }
                        var OldFilePath = file.FileName;
                        error = UploadCustomerFile(formFile, file, domainName, WebRootPath, OldFilePath, phone);
                        if (error.Code != Error.SUCCESS.Code)
                        {
                            return error;
                        }
                        if (cus.AvatarId == null)
                        {
                            cus.AvatarId = file.Id;
                        }
                    }
                    context.Customer.Update(cus);
                    return context.SaveChanges() > 0 ? error : error.Failed("Failed");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ErrorObject UploadCustomerFile(IFormFile formFile, Data.DBContext.Models.File file, string domainName, string WebRootPath, string OldFilePath, string phone)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    string extension = System.IO.Path.GetExtension(formFile.FileName);
                    string filename = System.IO.Path.GetFileNameWithoutExtension(formFile.FileName);
                    if (extension != ".jpeg" && extension != ".png" && extension != ".jpg")
                    {
                        return Error.INCORRECT_IMAGE_FORMAT;
                    }
                    if (formFile.Length > 2097152) // Limit 2Mb
                    {
                        return Error.IMAGE_SIZE_LARGE;
                    }
                    file.FileName = filename = filename + $@"_{phone}_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + extension;

                    string fileDirectory = WebRootPath + $@"\images\Customer";

                    if (!System.IO.Directory.Exists(fileDirectory))
                    {
                        System.IO.Directory.CreateDirectory(fileDirectory);
                    }

                    string saveFilePath = fileDirectory + $@"\{filename}";

                    using (var stream = new FileStream(saveFilePath, FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                    }
                    file.FileUrl = "/" + "images" + "/" + "Customer/" + filename;
                    file.FileExtension = extension;
                    file.FilePath = saveFilePath;
                    file.FileSize = Convert.ToInt32(formFile.Length);
                    if (file.Id == 0)
                    {
                        file.CreatedDateUtc = DateTime.UtcNow;
                        context.File.Add(file);
                    }
                    else
                    {
                        context.File.Update(file);
                    }
                    if (OldFilePath != null)
                    {
                        System.IO.File.Delete(WebRootPath + $@"\images\Customer" + $@"\{OldFilePath}");
                    }
                    return context.SaveChanges() > 0 ? error : error.Failed("Upload File Failed");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
