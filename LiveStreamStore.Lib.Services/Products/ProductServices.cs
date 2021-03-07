using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models;
using LiveStreamStore.Lib.Models.Enums;
using LiveStreamStore.Lib.Services.UploadFiles;

namespace LiveStreamStore.Lib.Services.Products
{
    public class ProductServices : IProductService
    {
        private readonly IFileService _FileService;

        public ProductServices(IFileService fileService)
        {
            _FileService = fileService;
        }

        public List<Product> GetListProductByIdLiveStream(int idLivestream, string keyword)
        {
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var products = context.Product.Where(x => x.LiveStreamId == idLivestream && x.Status != (short)EProductStatus.Deleted).Include(x => x.ProductInfo.Category).Include(x => x.ProductInfo.Image).OrderByDescending(x => x.Id).ToList();
                    if (!String.IsNullOrEmpty(keyword)) // kiểm tra chuỗi tìm kiếm có rỗng/null hay không 
                    {
                        products = products.Where(x => x.ProductInfo.Name.ToLower().Contains(keyword.ToLower()) || x.Code.ToLower().Contains(keyword.ToLower())).ToList(); //lọc theo chuỗi tìm kiếm
                    }
                    return products;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ErrorObject InsertProductInfo(ProductInfo productInfo, IFormFile formFile, File file, string domainName, string WebRootPath, string foldelSave)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    if (formFile != null)
                    {
                        error = _FileService.UploadFile(formFile, file, domainName, WebRootPath, foldelSave);
                        if (error.Code != Error.SUCCESS.Code)
                        {
                            return error;
                        }
                        productInfo.ImageId = file.Id;
                    }
                    context.ProductInfo.Add(productInfo);
                    return context.SaveChanges() > 0 ? error : error.Failed("Failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ErrorObject InsertProduct(Product product, ProductInfo productInfo, int idLivestream, IFormFile formFile, File file, string domainName, string WebRootPath, string foldelSave)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var products = context.Product.Where(x => x.LiveStreamId == idLivestream && x.Status != (short)EProductStatus.Deleted).ToList();

                    //check code already exists
                    if (products.Any(x => x.Code.ToLower().Equals(product.Code.ToLower())))
                    {
                        return Error.PRODUCT_CODE_EXISTED;
                    }

                    var storeId = context.LiveStream.Where(x => x.Id == idLivestream).Select(x => x.User.StoreId).FirstOrDefault();

                    if (product.ProductInfoId == null)
                    {
                        productInfo.StoreId = storeId;
                        error = InsertProductInfo(productInfo, formFile, file, domainName, WebRootPath, foldelSave);
                        if (error.Code != Error.SUCCESS.Code)
                        {
                            return error;
                        }
                        product.ProductInfoId = productInfo.Id;
                    }

                    product.StoreId = storeId;
                    product.LiveStreamId = idLivestream;
                    product.Status = (short)EProductStatus.Active;
                    product.CreatedDateUtc = DateTime.UtcNow;
                    context.Product.Add(product);
                    return context.SaveChanges() > 0 ? error.SetData(product) : error.Failed("Failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ErrorObject UpdateProduct(Product product, ProductInfo productInfo, int idLivestream, IFormFile formFile, File file, string domainName, string WebRootPath, string foldelSave)
        {
            var error = Error.Success();
            using (var context = new LiveStreamStoreContext())
            {
                var products = context.Product.Where(x => x.LiveStreamId == idLivestream && x.Status != (short)EProductStatus.Deleted).Include(x => x.ProductInfo).ToList();

                var productOld = context.Product.FirstOrDefault(x => x.Id == product.Id);
                file = context.File.Where(x => x.Id == productOld.ProductInfo.ImageId).FirstOrDefault();

                //check code already exists
                if (products.Any(x => x.Code.ToLower().Equals(product.Code.ToLower()) && productOld.Code.ToLower() != product.Code.ToLower()))
                {
                    return Error.PRODUCT_CODE_EXISTED;
                }

                if (formFile != null)
                {
                    if (file == null)
                    {
                        file = new File();
                    }
                    error = _FileService.UploadFile(formFile, file, domainName, WebRootPath, foldelSave);
                    if (error.Code != Error.SUCCESS.Code)
                    {
                        return error;
                    }
                    if (productOld.ProductInfo.ImageId == null)
                    {
                        productOld.ProductInfo.ImageId = file.Id;
                    }
                }

                productOld.ProductInfo.Description = productInfo.Description;
                productOld.Code = product.Code;
                productOld.Weight = product.Weight;
                productOld.Price = product.Price;
                productOld.Quantity = product.Quantity;
                productOld.Limited = product.Limited;

                context.Product.Update(productOld);
                return context.SaveChanges() > 0 ? error : error.Failed("Failed");
            }
        }

        public ErrorObject UpdateOneFieldProduct(Product product, ProductInfo productInfo, int idLivestream, IFormFile formFile, File file, string domainName, string WebRootPath, string foldelSave)
        {
            var error = Error.Success();
            using (var context = new LiveStreamStoreContext())
            {
                var productOld = context.Product.Where(m => m.Id == product.Id).Include(m => m.ProductInfo).FirstOrDefault();
                var products = context.Product.Where(x => x.LiveStreamId == idLivestream && x.Status != (short)EProductStatus.Deleted).Include(x => x.ProductInfo).ToList();
                if (product.Code != null)
                {
                    if (products.Any(x => x.Code.ToLower().Equals(product.Code.ToLower()) && productOld.Code.ToLower() != product.Code.ToLower()))
                    {
                        return Error.PRODUCT_CODE_EXISTED;
                    }
                }

                file = context.File.Where(x => x.Id == productOld.ProductInfo.ImageId).FirstOrDefault();
                if (formFile != null)
                {
                    if (file == null)
                    {
                        file = new File();
                    }
                    error = _FileService.UploadFile(formFile, file, domainName, WebRootPath, foldelSave);
                    if (error.Code != Error.SUCCESS.Code)
                    {
                        return error;
                    }
                    if (productOld.ProductInfo.ImageId == null)
                    {
                        productOld.ProductInfo.ImageId = file.Id;
                    }
                }
                productOld.Code = product.Code ?? productOld.Code;
                productOld.Weight = product.Weight ?? productOld.Weight;
                productOld.Price = product.Price ?? productOld.Price;
                productOld.Quantity = product.Quantity ?? productOld.Quantity;
                productOld.Limited = product.Limited ?? productOld.Limited;
                productOld.ProductInfo.Description = productInfo.Description ?? productOld.ProductInfo.Description;

                context.Product.Update(productOld);
                return context.SaveChanges() > 0 ? error : error.Failed("Failed");
            }
        }

        public ErrorObject InsertListProductCopyLiveStream(List<Product> products, int liveStreamId)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var storeId = context.LiveStream.Where(x => x.Id == liveStreamId).Select(x => x.User.StoreId).FirstOrDefault();
                    foreach (var copy in products)
                    {
                        ProductInfo prodInfo = new ProductInfo();
                        prodInfo.StoreId = storeId;
                        prodInfo.CategoryId = copy.ProductInfo.CategoryId;
                        prodInfo.ImageId = copy.ProductInfo.ImageId;
                        prodInfo.Name = copy.ProductInfo.Name;
                        prodInfo.Barcode = copy.ProductInfo.Barcode;
                        prodInfo.Description = copy.ProductInfo.Description;
                        prodInfo.CreatedDateUtc = DateTime.UtcNow;
                        context.ProductInfo.Add(prodInfo);
                        context.SaveChanges();

                        Product prod = new Product();
                        prod.LiveStreamId = liveStreamId;
                        prod.StoreId = storeId;
                        prod.ProductInfoId = prodInfo.Id;
                        prod.Code = copy.Code;
                        prod.Price = copy.Price;
                        prod.Weight = copy.Weight;
                        prod.Quantity = copy.Quantity;
                        prod.NumberProductSold = 0;
                        prod.Limited = copy.Limited;
                        prodInfo.CreatedDateUtc = DateTime.UtcNow;
                        prod.Status = (short)EProductStatus.Active;
                        context.Product.Add(prod);
                    }
                    return context.SaveChanges() > 0 ? error : error.Failed("Thất bại");
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public ErrorObject UpdateStatusProductInfo(int id, EProductStatus eProductStatus)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var productInfo = context.ProductInfo.Where(x => x.Id == id).FirstOrDefault();
                    productInfo.IsDeleted = (short)eProductStatus;
                    return context.SaveChanges() > 0 ? error : error.Failed("Failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ErrorObject UpdateStatusProduct(int id, EProductStatus eProductStatus)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var product = context.Product.Where(x => x.Id == id).FirstOrDefault();
                    product.Status = (short)eProductStatus;
                    return context.SaveChanges() > 0 ? error : error.Failed("Failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Product GetProductById(int Id)
        {
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    return context.Product.Where(x => x.Id == Id).Include(x => x.ProductInfo.Image).SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ErrorObject CheckProduct(int ProductId, int Quantity)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var product = context.Product.FirstOrDefault(x => x.Id == ProductId);
                    if (product.NumberProductSold != null)
                    {
                        if ((product.NumberProductSold + Quantity) > product.Quantity)
                        {
                            return Error.CHECK_PRODUCT;
                        }
                    }
                    return error;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Product> GetListProductByPage(int idLivestream, int pageNumber)
        {
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var products = context.Product.Where(x => x.LiveStreamId == idLivestream && x.Status != (short)EProductStatus.Deleted && x.Quantity != 0).Include(x => x.ProductInfo.Category).Include(x => x.ProductInfo.Image).ToList();
                    int pageSize = 9;
                    return products.OrderBy(x => x.Id).Skip(pageNumber * pageSize).Take(pageSize).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ProductInfo> GetListProductInfoByIdStore(int idStore)
        {
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var productInfos = context.ProductInfo.Where(x => x.StoreId == idStore && x.IsDeleted != (short)EProductStatus.Deleted)
                        .Include(x => x.Category)
                        .Include(x => x.Image)
                        .Include(x => x.Product)
                        .ToList();
                    return productInfos;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ErrorObject InsertListProductInfo(List<ProductInfo> productInfo, List<IFormFile> formFile, File file, string domainName, string WebRootPath, string foldelSave, List<bool> existsImg)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    int i = 0;
                    foreach (var proInfo in productInfo)
                    {
                        if (existsImg[i])
                        {
                            int indexImage = 0;
                            if (formFile != null)
                            {
                                if (file != null)
                                {
                                    file = new File();
                                }
                                error = _FileService.UploadFile(formFile[indexImage], file, domainName, WebRootPath, foldelSave);
                                if (error.Code != Error.SUCCESS.Code)
                                {
                                    return error;
                                }
                                proInfo.ImageId = file.Id;
                            }
                            indexImage++;
                        }
                        context.ProductInfo.Add(proInfo);
                        i++;
                    }
                    return context.SaveChanges() > 0 ? error.SetData(productInfo.Select(m => m.Id).ToList()) : error.Failed("Failed!");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ErrorObject InsertListProduct(List<Product> product, List<ProductInfo> productInfo, int idLivestream, List<IFormFile> formFile, File file, string domainName, string WebRootPath, string foldelSave, List<bool> existsImg)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var products = context.Product.Where(x => x.LiveStreamId == idLivestream && x.Status != (short)EProductStatus.Deleted).ToList();
                    foreach (var prod in product)
                    {
                        if (products.Any(x => x.Code.ToLower().Equals(prod.Code.ToLower())))
                        {
                            return Error.PRODUCT_CODE_EXISTED;
                        }
                    }
                    error = InsertListProductInfo(productInfo, formFile, file, domainName, WebRootPath, foldelSave, existsImg);
                    if (error.Code != Error.SUCCESS.Code)
                    {
                        return error.Failed("Thất bại.");
                    }
                    int i = 0;
                    var storeId = context.LiveStream.Where(x => x.Id == idLivestream).Select(x => x.User.StoreId).FirstOrDefault();
                    foreach (var prod in (List<int>)error.Data)
                    {
                        if (product[i].ProductInfoId == null)
                        {
                            product[i].ProductInfoId = prod;
                            product[i].StoreId = storeId;
                            product[i].LiveStreamId = idLivestream;
                            product[i].Status = (short)EProductStatus.Active;
                            product[i].CreatedDateUtc = DateTime.UtcNow;
                            context.Product.Add(product[i]);
                        }
                        i++;
                    }
                    return context.SaveChanges() > 0 ? error : error.Failed("Failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
