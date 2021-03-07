using System;
using System.Collections.Generic;
using System.Text;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models;

namespace LiveStreamStore.Lib.Services.Cart
{
    public interface ICartService
    {
        ErrorObject CreateCart(ShoppingCart shoppingCart);
        ErrorObject Remove(int Id);
        ErrorObject Add(int cusId, int LiveStreamId, int proId, int Quantity);
        List<ShoppingCart> GetListProductByStoreCode(int id, string storeCode);
        ErrorObject SetQuantityProduct(int CusId, int LiveStream, int ProId, int Quantity);
        ErrorObject DeleteProduct(int customerId, int productId);
        ErrorObject AddQuantityToCart(int cusId, int? ProductId, int? Quantity);
        List<ShoppingCart> GetAllListProductByStoreCode(int id, string storeCode);
        ErrorObject CheckCartOrder(int CustomerId, int ProductId, int Quantity);
    }
}