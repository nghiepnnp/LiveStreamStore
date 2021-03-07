using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models;

namespace LiveStreamStore.Lib.Services.UploadFiles
{
    public class FileServices : IFileService
    {
        public ErrorObject UploadFile(IFormFile formFile, File file, string domainName, string WebRootPath, string foldelSave)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    string ext = System.IO.Path.GetExtension(formFile.FileName); // get đuôi file
                    if (ext != ".jpeg" && ext != ".png" && ext != ".jpg")
                    {
                        return Error.INCORRECT_IMAGE_FORMAT;
                    }

                    //if (formFile.Length > 2097152)
                    //{
                    //    return Error.IMAGE_SIZE_LARGE;
                    //}

                    if (file.Id != 0)
                    {
                        if (file.FilePath != null)
                        {
                            string _imageToBeDeleted = WebRootPath + $@"\{file.FileUrl}";
                            if (System.IO.File.Exists(_imageToBeDeleted))
                            {
                                System.IO.File.Delete(_imageToBeDeleted);
                            }
                        }
                    }

                    // Set file Name
                    var fileName = Guid.NewGuid().ToString().Replace("-", "") + System.IO.Path.GetExtension(formFile.FileName);
                    // Set file Directory
                    string fileDirectory = WebRootPath + $@"\images" + $@"\{foldelSave}";

                    if (!System.IO.Directory.Exists(fileDirectory))
                    {
                        System.IO.Directory.CreateDirectory(fileDirectory);
                    }
                    // Get url To Save
                    string saveFilePath = fileDirectory + $@"\{fileName}";

                    using (var stream = new System.IO.FileStream(saveFilePath, System.IO.FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                    }

                    // File url
                    string saveFileUrl = "/" + "images" + "/" + foldelSave + "/" + fileName;

                    file.FileExtension = ext;
                    file.FileName = fileName;
                    file.FilePath = saveFilePath;
                    file.FileUrl = saveFileUrl;
                    file.FileSize = Convert.ToInt32(formFile.Length);
                    file.Domain = domainName;
                    if (file.Id == 0)
                    {
                        file.CreatedDateUtc = DateTime.UtcNow;
                        context.File.Add(file);
                    }
                    else
                    {
                        context.File.Update(file);
                    }
                    return context.SaveChanges() > 0 ? error : error.Failed("Upload File Failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
