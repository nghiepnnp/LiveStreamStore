using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models;

namespace LiveStreamStore.Lib.Services.Cart
{
    public class CartService : ICartService
    {
        public ErrorObject CreateCart(ShoppingCart shoppingCart)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var cart = context.ShoppingCart.Where(x => x.CustomerId == shoppingCart.CustomerId && x.LivestreamId == shoppingCart.LivestreamId && x.ProductId == shoppingCart.ProductId)
                        .Include(x => x.Product).FirstOrDefault();
                    var pro = context.Product.FirstOrDefault(x => x.Id == shoppingCart.ProductId);
                    if (shoppingCart.Quantity > pro.Limited)
                    {
                        return Error.LIMITED;
                    }
                    if (cart == null)
                    {
                        if (pro.NumberProductSold == null)
                        {
                            context.ShoppingCart.Add(shoppingCart);
                        }
                        else
                        {
                            if ((shoppingCart.Quantity + pro.NumberProductSold) <= pro.Quantity)
                            {
                                context.ShoppingCart.Add(shoppingCart);
                            }
                            else
                            {
                                return Error.CHECK_PRODUCT;
                            }
                        }
                    }
                    else if (cart.QuantitySold == null)
                    {
                        cart.Quantity = cart.Quantity + shoppingCart.Quantity;
                        if ((cart.Quantity + cart.Product.NumberProductSold) > cart.Product.Quantity)
                        {
                            return Error.LIMITED;
                        }
                        else if (cart.Quantity <= cart.Product.Limited)
                        {
                            context.ShoppingCart.Update(cart);
                        }
                        else
                        {
                            return Error.LIMITED;
                        }
                    }
                    else
                    {
                        if ((cart.Quantity + shoppingCart.Quantity + cart.Product.NumberProductSold) > cart.Product.Quantity)
                        {
                            return Error.CHECK_PRODUCT;
                        }
                        else if (cart.QuantitySold == cart.Product.Limited || (cart.Quantity + cart.QuantitySold + shoppingCart.Quantity) > cart.Product.Limited)
                        {
                            return Error.LIMITED;
                        }
                        else
                        {
                            cart.Quantity = cart.Quantity + shoppingCart.Quantity;
                            context.ShoppingCart.Update(cart);
                        }
                    }
                    return context.SaveChanges() > 0 ? error : error.Failed("Create customer failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ErrorObject Remove(int Id)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var list = context.ShoppingCart.Where(x => x.CustomerId == Id && x.Livestream.OpenPreSale == true).ToList();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].QuantitySold == null)
                        {
                            context.ShoppingCart.Remove(list[i]);
                        }
                        else
                        {
                            list[i].Quantity = 0;
                            context.ShoppingCart.Update(list[i]);
                        }
                        list.Remove(list[i]);
                        i--;
                    }
                    return context.SaveChanges() > 0 ? error : error.Failed("Remove failed!");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ErrorObject Add(int cusId, int LiveStreamId, int proId, int Quantity)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var list = context.ShoppingCart.Where(x => x.CustomerId == cusId && x.LivestreamId == LiveStreamId && x.ProductId == proId).Include(x => x.Product).FirstOrDefault();
                    if (list == null)
                    {
                        ShoppingCart shoppingCarts = new ShoppingCart();
                        shoppingCarts.CustomerId = cusId;
                        shoppingCarts.LivestreamId = LiveStreamId;
                        shoppingCarts.ProductId = proId;
                        shoppingCarts.Quantity = Quantity;
                        context.ShoppingCart.Add(shoppingCarts);
                    }
                    else
                    {
                        if ((list.QuantitySold + Quantity) <= list.Product.Limited)
                        {
                            list.Quantity += Quantity;
                        }
                    }
                    return context.SaveChanges() > 0 ? error : error.Failed("Save failed!");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ShoppingCart> GetListProductByStoreCode(int id, string storeCode)
        {
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var list = context.ShoppingCart.Where(x => x.CustomerId == id && x.Livestream.User.Store.Code == storeCode && x.Quantity != 0)
                        .Include(x => x.Livestream).Include(x => x.Product.ProductInfo.Category).Include(x => x.Product.ProductInfo.Image).ToList();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if(list[i].Livestream.OpenPreSale == false)
                        {
                            if (list[i].QuantitySold == null)
                            {
                                context.ShoppingCart.Remove(list[i]);
                            }
                            else
                            {
                                list[i].Quantity = 0;
                                context.ShoppingCart.Update(list[i]);
                            }
                            context.SaveChanges();

                            list.Remove(list[i]);
                            i--;
                        }
                     }
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ErrorObject SetQuantityProduct(int CusId, int LiveStream, int ProId, int Quantity)
        {
            try
            {
                var error = Error.Success();
                using (var context = new LiveStreamStoreContext())
                {
                    var cart = context.ShoppingCart.Where(x => x.CustomerId == CusId && x.LivestreamId == LiveStream && x.ProductId == ProId).Include(x => x.Product).FirstOrDefault();
                    if (cart.Product.NumberProductSold == null || (cart.Product.Quantity - cart.Product.NumberProductSold) > cart.Product.Limited)
                    {
                        if (cart.QuantitySold == null && Quantity > cart.Product.Limited)
                        {
                            return Error.LIMITED;
                        }
                        else if (cart.QuantitySold != null && Quantity + cart.QuantitySold > cart.Product.Limited)
                        {
                            return Error.LIMITED;
                        }
                    }
                    else
                    {
                        if (cart.QuantitySold == null && Quantity > (cart.Product.Quantity - cart.Product.NumberProductSold))
                        {
                            return Error.CHECK_PRODUCT;
                        }
                        else if (cart.QuantitySold != null && Quantity > (cart.Product.Quantity - cart.Product.NumberProductSold))
                        {
                            return Error.CHECK_PRODUCT;
                        }
                    }
                    cart.Quantity = Quantity;
                    context.ShoppingCart.Update(cart);
                    return context.SaveChanges() > 0 ? error : error.Failed("Set failed!");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ErrorObject DeleteProduct(int customerId, int productId)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var cart = context.ShoppingCart.Where(x => x.CustomerId == customerId && x.ProductId == productId).FirstOrDefault();
                    if (cart.QuantitySold == null)
                    {
                        context.ShoppingCart.Remove(cart);
                    }
                    else
                    {
                        cart.Quantity = 0;
                        context.ShoppingCart.Update(cart);
                    }
                    return context.SaveChanges() > 0 ? error : error.Failed("Remove failed!");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ErrorObject AddQuantityToCart(int cusId, int? ProductId, int? Quantity)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var cart = context.ShoppingCart.Where(x => x.CustomerId == cusId && x.ProductId == ProductId).FirstOrDefault();
                    if (cart.QuantitySold == null)
                    {
                        cart.QuantitySold = Quantity;
                    }
                    else
                    {
                        cart.QuantitySold += Quantity;
                    }
                    cart.Quantity = 0;
                    context.ShoppingCart.Update(cart);
                    return context.SaveChanges() > 0 ? error : error.Failed("Failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ShoppingCart> GetAllListProductByStoreCode(int id, string storeCode)
        {
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var list = context.ShoppingCart.Where(x => x.CustomerId == id && x.Livestream.User.Store.Code == storeCode).Include(x => x.Livestream)
                            .Include(x => x.Product.ProductInfo.Category).Include(x => x.Product.ProductInfo.Image).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ErrorObject CheckCartOrder(int CustomerId, int ProductId, int Quantity)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var cart = context.ShoppingCart.Where(x => x.CustomerId == CustomerId && x.ProductId == ProductId).Include(x => x.Product).FirstOrDefault();
                    if (cart.Product.NumberProductSold == null || (cart.Product.Quantity - cart.Product.NumberProductSold) > cart.Product.Limited)
                    {
                        if (cart.QuantitySold == null && Quantity > cart.Product.Limited)
                        {
                            return Error.LIMITED;
                        }
                        else if (cart.QuantitySold != null && Quantity + cart.QuantitySold > cart.Product.Limited)
                        {
                            return Error.LIMITED;
                        }
                    }
                    else
                    {
                        if (cart.QuantitySold == null && Quantity > (cart.Product.Quantity - cart.Product.NumberProductSold))
                        {
                            return Error.CHECK_PRODUCT;
                        }
                        else if (cart.QuantitySold != null && (Quantity + cart.QuantitySold) > (cart.Product.Quantity - cart.Product.NumberProductSold))
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
    }
}