using System;
using System.Collections.Generic;
using System.Text;
using LiveStreamStore.Lib.Models;
using LiveStreamStore.Lib.Data.DBContext.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using LiveStreamStore.Lib.Services.UploadFiles;
using Microsoft.EntityFrameworkCore;
using LiveStreamStore.Lib.Services.Livestreams;

namespace LiveStreamStore.Lib.Services.Users
{
    public class UserServices : IUserService
    {
        private readonly IFileService _fileService;
        private readonly ILivestreamService _LivestreamService;

        public UserServices(IFileService fileService, ILivestreamService livestreamService)
        {
            _fileService = fileService;
            _LivestreamService= livestreamService;
        }
        public ErrorObject LoginWithFacebook(User user)
        {
            var error = Error.Success();          
            try
            {
                var UserStore = new User();
                using (var context = new LiveStreamStoreContext())
                {
                    UserStore = context.User.FirstOrDefault(x => x.FaceBookId == user.FaceBookId);
                    if (UserStore == null) // user mới
                    {
                        user.StoreId = 2; 
                        context.User.Add(user);
                        UserStore = user;
                        if (context.SaveChanges() < 1) {
                            return Error.DATABASE;
                        };                        
                    }
                    else
                    {
                        UserStore.FaceBookToken = user.FaceBookToken;
                        context.SaveChanges();
                    }
                    return error.SetData(UserStore);
                }              
            }
            catch (Exception ex)
            {
                throw ex;
            }

          
        }
        public ErrorObject CreateUserStore(User user)
        {
            try
            {
                var error = Error.Success();
                //var UserStore = new LiveStreamStore.Lib.Data.DBContext.Models.UserStore();
                using (var context = new LiveStreamStoreContext())
                {
                    context.User.Add(user);
                    context.SaveChanges();
                }
                return error;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public User GetUserStoreById(int id)
        {
            using (var context = new LiveStreamStoreContext())
            {
                return context.User.Where(x => x.Id == id).Include(x => x.Avatar).Include(x => x.Store).FirstOrDefault();
            }
        }
        public ErrorObject UpdateProfile(int id, string fullname, string email, string phone)
        {
            var error = Error.Success();
            using (var context = new LiveStreamStoreContext())
            {
                var currentuser = context.User.FirstOrDefault(x => x.Id == id);
                currentuser.Fullname = fullname;
                currentuser.Email = email;
                currentuser.Phone = phone;
                currentuser.ModifiedDateUtc = DateTime.UtcNow;
                context.User.Update(currentuser);
                return context.SaveChanges() > 0 ? error : error.Failed("Update User failed");
            }
        }

        public ErrorObject UpdateUserAvatar(int id, IFormFile formFile, File file, string domainName, string WebRootPath, string foldelSave)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var currentUser = context.User.FirstOrDefault(x => x.Id == id);
                    var username = currentUser.Username;
                    file = context.File.FirstOrDefault(x => x.Id == currentUser.AvatarId);

                    if (formFile != null)
                    {
                        if (file == null)
                        {
                            file = new File();
                        }
                        error = _fileService.UploadFile(formFile, file, domainName, WebRootPath, foldelSave);
                        if (error.Code != Error.SUCCESS.Code)
                        {
                            return error;
                        }
                        if (currentUser.AvatarId == null)
                        {
                            currentUser.AvatarId = file.Id;
                        }
                    }

                    context.User.Update(currentUser);
                    return context.SaveChanges() > 0 ? error : error.Failed("Failed");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public User GetUserStoreByStoreCode(string storeCode)
        {
            using (var context = new LiveStreamStoreContext())
            {
                return context.User.Where(x => x.Store.Code == storeCode).Include(x => x.LiveStream).Include(x => x.Store).FirstOrDefault();
            }
        }
        public string GetFacebookTokenByLiveStreamId(int livestreamId)
        {
            try
            {
                var userId = _LivestreamService.GetUserIdLiveStreamByLiveStreamId(livestreamId);
                using (var context = new LiveStreamStoreContext())
                {
                    return context.User.FirstOrDefault(x => x.Id == userId).FaceBookToken;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
     
        }
    }
}
