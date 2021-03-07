using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models;

namespace LiveStreamStore.Lib.Services.Users
{
    public interface IUserService
    {
        ErrorObject LoginWithFacebook(LiveStreamStore.Lib.Data.DBContext.Models.User user);
        ErrorObject UpdateProfile(int id, string fullname, string email, string phone);
        LiveStreamStore.Lib.Data.DBContext.Models.User GetUserStoreById(int id);
        ErrorObject UpdateUserAvatar(int id, IFormFile formFile, File file, string domainName, string WebRootPath, string foldelSave);
        Data.DBContext.Models.User GetUserStoreByStoreCode(string storeCode);
        string GetFacebookTokenByLiveStreamId(int livestreamId);
    }
}
