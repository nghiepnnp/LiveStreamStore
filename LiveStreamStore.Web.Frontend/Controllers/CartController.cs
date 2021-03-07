using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using LiveStream.Web.Frontend.Controllers;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models;
using LiveStreamStore.Lib.Services.Cart;
using LiveStreamStore.Lib.Services.Customers;
using LiveStreamStore.Lib.Services.Products;
using LiveStreamStore.Lib.Services.WorkContext;
using LiveStreamStore.Web.Frontend.Filters;

namespace LiveStreamStore.Web.Frontend.Controllers
{
    public class CartController : BaseController
    {
        private readonly IProductService _ProductService;
        private readonly IWorkContext _WorkContext;
        private readonly ICustomerService _CustomerService;
        private readonly ICartService _CartService;

        public CartController(IProductService productService, IWorkContext workContext, ICustomerService customerService, ICartService cartService)
        {
            _ProductService = productService;
            _WorkContext = workContext;
            _CustomerService = customerService;
            _CartService = cartService;
        }

        [AuthFilter]
        public IActionResult Index()
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                ViewBag.StateProvince = GetAllStateProvince();
                string[] Path = (HttpContext.Request.Path).ToString().Split('/');
                if (_Customer != null)
                {
                    ViewBag.CookieCart = _CartService.GetListProductByStoreCode(_Customer.Id, Path[1]);
                    var customer = _CustomerService.GetCustomerById(_Customer.Id);
                    ViewBag.Address = GetAddress(_Customer.Id);
                    return View(customer);
                }
                ViewBag.CookieCart = GetCookieCartByStoreCode();
                return View();
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
        }

        [HttpPost]
        public PartialViewResult GetListCart()
        {
            try
            {
                if (_Customer == null)
                {
                    ViewBag.CookieCart = GetCookieCartByStoreCode();
                    List<Product> products = new List<Product>();
                    if (ViewBag.CookieCart != null)
                    {
                        foreach (var item in ViewBag.CookieCart)
                        {
                            if (item.Id != 0)
                            {
                                products.Add(_ProductService.GetProductById(item.Id));
                            }
                        }
                    }
                    return PartialView("_PartialListCart", products);
                }
                else
                {
                    string[] Path = (HttpContext.Request.Path).ToString().Split('/');
                    ViewBag.ListCart = _CartService.GetListProductByStoreCode(_Customer.Id, Path[1]);
                    return PartialView("_PartialListCart");
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return PartialView("_PartialError", ex);
            }
        }

        [HttpPost]
        public IActionResult GetProductCartById(int IdProduct)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                var product = _ProductService.GetProductById(IdProduct);
                product.ProductInfo.Image = null;
                product.ProductInfo.Product = null;
                return Json(product);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
        }

        //Lưu sản phẩm khi click vào thêm sản phẩm
        [HttpPost]
        public IActionResult SetCart(int LivestreamId, int ProductId, int Quantity, int Limited)
        {
            var error = new ErrorObject(Error.SUCCESS);
            var check = new ErrorObject(Error.SUCCESS);
            try
            {
                if (_Customer == null)
                {
                    var list = GetCookieCartByStoreCode();
                    if (list.Count != 0)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (list[i].Id == ProductId)
                            {
                                check = _ProductService.CheckProduct(ProductId, (list[i].Quantity + Quantity));
                                break;
                            }
                        }
                    }
                    else
                    {
                        check = _ProductService.CheckProduct(ProductId, Quantity);
                    }
                    if (check.Code != error.Code)
                    {
                        return Json(check);
                    }

                    var value = _WorkContext.checkCookieCart(ProductId, Quantity);
                    if (value == true)
                    {
                        string[] Path = (HttpContext.Request.Path).ToString().Split('/');
                        SetCookieCart(ProductId, Quantity, Limited, LivestreamId, Path[1]);
                    }
                    else
                    {
                        return Json(Error.LIMITED);
                    }
                }
                else
                {
                    ShoppingCart shoppingCart = new ShoppingCart();
                    shoppingCart.CustomerId = _Customer.Id;
                    shoppingCart.LivestreamId = LivestreamId;
                    shoppingCart.ProductId = ProductId;
                    shoppingCart.Quantity = Quantity;
                    error = _CartService.CreateCart(shoppingCart);
                }
                return Json(error);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
        }

        //Lấy để set số sản phảm của giỏ hàng
        [HttpPost]
        public int GetCart()
        {
            try
            {
                var count = 0;
                if (_Customer == null)
                {
                    var list = GetCookieCartByStoreCode();
                    if (list == null)
                    {
                        return count;
                    }
                    for (int i = 0; i < list.Count; i++)
                    {
                        count += list[i].Quantity;
                    }
                }
                else
                {
                    string[] Path = (HttpContext.Request.Path).ToString().Split('/');
                    var list = _CartService.GetListProductByStoreCode(_Customer.Id, Path[1]);
                    for (int i = 0; i < list.Count; i++)
                    {
                        count += (int)list[i].Quantity;
                    }
                    RemoveCookie("Cookie.Cart");
                }
                return count;
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                throw ex;
            }
        }

        [HttpPost]
        public IActionResult Delete(int Id)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                if (_Customer == null)
                {
                    DeleteCart(Id);
                }
                else
                {
                    error = _CartService.DeleteProduct(_Customer.Id, Id);
                }
                return Json(error);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
        }

        [HttpPost]
        public IActionResult SetQuantityCart(int ProductId, int Quantity, int Limited, int LivestreamId)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                if (_Customer == null)
                {
                    string[] Path = (HttpContext.Request.Path).ToString().Split('/');
                    SetCookieCart(ProductId, Quantity, Limited, LivestreamId , Path[1]);
                }
                else
                {
                    error = _CartService.SetQuantityProduct(_Customer.Id, LivestreamId, ProductId, Quantity);
                }
                return Json(error);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
        }
    }
}
