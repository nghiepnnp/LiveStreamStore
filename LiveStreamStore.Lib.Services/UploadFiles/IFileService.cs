using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models;

namespace LiveStreamStore.Lib.Services.UploadFiles
{
    public interface IFileService
    {
        ErrorObject UploadFile(IFormFile formFile, File file, string domainName, string WebRootPath, string foldelSave);
    }
}
