using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models;
using LiveStreamStore.Lib.Models.Enums;

namespace LiveStreamStore.Lib.Services.Products
{
    public interface IProductService
    {
        List<Product> GetListProductByIdLiveStream(int idLivestream, string keyword);
        ErrorObject InsertProductInfo(ProductInfo productInfo, IFormFile formFile, File file, string domainName, string WebRootPath, string foldelSave);
        ErrorObject InsertProduct(Product product, ProductInfo productInfo, int idLivestream, IFormFile formFile, File file, string domainName, string WebRootPath, string foldelSave);
        ErrorObject UpdateProduct(Product product, ProductInfo productInfo, int idLivestream, IFormFile formFile, File file, string domainName, string WebRootPath, string foldelSave);
        ErrorObject UpdateStatusProduct(int id, EProductStatus eProductStatus);
        ErrorObject UpdateStatusProductInfo(int id, EProductStatus eProductStatus);
        Product GetProductById(int Id);
        ErrorObject CheckProduct(int ProductId, int Quantity);
        List<Product> GetListProductByPage(int idLivestream, int pageNumber); 
        ErrorObject InsertListProductCopyLiveStream(List<Product> products, int liveStreamId);
        ErrorObject UpdateOneFieldProduct(Product product, ProductInfo productInfo, int idLivestream, IFormFile formFile, File file, string domainName, string WebRootPath, string foldelSave);
        List<ProductInfo> GetListProductInfoByIdStore(int idStore);

        ErrorObject InsertListProductInfo(List<ProductInfo> productInfo, List<IFormFile> formFile, File file, string domainName, string WebRootPath, string foldelSave, List<bool> existsImg);
        ErrorObject InsertListProduct(List<Product> product, List<ProductInfo> productInfo, int idLivestream, List<IFormFile> formFile, File file, string domainName, string WebRootPath, string foldelSave, List<bool> existsImg);
    }
}
