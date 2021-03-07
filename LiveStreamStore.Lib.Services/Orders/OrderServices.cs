using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models;
using LiveStreamStore.Lib.Models.Enums;
using LiveStreamStore.Lib.Models.SearchFilter;
using LiveStreamStore.Lib.Services.Caching;

namespace LiveStreamStore.Lib.Services.Orders
{
    public class OrderServices : IOrderService
    {
        public List<ResultOrderFilter> GetListOrderByIdLiveStream(SearchOrderFilter searchOrderFilter)
        {
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var orders = context.ResultOrderFilter.FromSqlRaw("EXECUTE dbo.SP_GetOrderByFilter @IdLiveStream = {0}", searchOrderFilter.IdLiveStream).ToList();

                    // Sắp xếp order có địa chỉ lên trước
                    var orderHasAddress = orders.Where(x => x.AddressId != null).ToList();
                    var orderNotAddress = orders.Where(x => x.AddressId == null).ToList();
                    orderHasAddress.AddRange(orderNotAddress);
                    
                    var orderIds = orders.Select(x => x.Id).ToList();
                    var orderDetails = context.OrderDetail.Where(y => orderIds.Contains(y.OrderId.Value)).Include(z => z.Product.ProductInfo.Category).Include(x => x.Order).Include(x => x.OrderTemp).ToList();

                    // lấy comment
                    orderDetails.ForEach(x => {
                        foreach (var item in x.OrderTemp)
                        {
                            item.Comment = context.Comment.FirstOrDefault(y => y.CommentFaceBookId == item.CommentFaceBookId); 
                        }
                    });

                    // Sắp xếp orderDetail có địa chỉ lên trước
                    var orderDetailHasAddress = orderDetails.Where(x => x.Order.AddressId != null).ToList();
                    var orderDetailNotAddress = orderDetails.Where(x => x.Order.AddressId == null).ToList();
                    orderDetailHasAddress.AddRange(orderDetailNotAddress);

                    orderHasAddress.ForEach(x =>
                    {
                        x.OrderDetail = orderDetailHasAddress.Where(y => y.OrderId == x.Id).ToList();
                    });

                    List<OrderDetail> listOrderDetail = new List<OrderDetail>();
                    listOrderDetail.AddRange(orderDetailHasAddress);

                    var stt = 1;
                    int? soluongtam = 0;
                    for (int i = 0; i < listOrderDetail.Count; i++)
                    {
                        for (int j = 0; j < orderDetailHasAddress.Count; j++)
                        {
                            if (listOrderDetail[i].ProductId == orderDetailHasAddress[j].ProductId)
                            {
                                if (stt == 1)
                                {
                                    listOrderDetail[i].SoLuongProductConLai = listOrderDetail[i].Product.Quantity - listOrderDetail[i].Quantity;
                                    soluongtam = listOrderDetail[i].SoLuongProductConLai;
                                }
                                else
                                {
                                    if (soluongtam > 0)
                                    {
                                        orderDetailHasAddress[j].SoLuongProductConLai = soluongtam - orderDetailHasAddress[j].Quantity;
                                    }
                                    else
                                    {
                                        orderDetailHasAddress[j].SoLuongProductConLai = 0 - orderDetailHasAddress[j].Quantity;
                                    }
                                    soluongtam = orderDetailHasAddress[j].SoLuongProductConLai;
                                }
                                stt++;
                            }
                        }
                        stt = 1;
                        int? productId = listOrderDetail[i].ProductId;
                        for (int z = 0; z < listOrderDetail.Count; z++)
                        {
                            if (productId == listOrderDetail[z].ProductId)
                            {
                                listOrderDetail.Remove(listOrderDetail[z]);
                                z--;
                            }
                        }
                        i--;
                    }
                    var result = orders.Where(x => x.AddressId == null && searchOrderFilter.AddressId == 0 || x.AddressId != null && searchOrderFilter.AddressId == 1 || searchOrderFilter.AddressId == 999).Skip((searchOrderFilter.Page - 1) * searchOrderFilter.Top).Take(searchOrderFilter.Top).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Order GetOrderById(int id)
        {
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var order = context.Order.Where(x => x.Id == id).Include(x => x.OrderDetail).Include(x => x.Customer).Include(x => x.LiveStream).FirstOrDefault();
                    
                    foreach (var item in order.OrderDetail)
                    {
                        var product = context.Product.Where(x => x.Id == item.ProductId).Include(x => x.ProductInfo.Category).FirstOrDefault();
                        item.Product = product;
                    }
                    return order;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ErrorObject UpdateStatusOrder(int id, EStatusOrder eStatusOrder)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var order = context.Order.Where(x => x.Id == id).FirstOrDefault();
                    order.Status = (short)eStatusOrder;
                    return context.SaveChanges() > 0 ? error : error.Failed("Failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ErrorObject UpdateListStatusOrder(List<int> ListId, EStatusOrder eStatusOrder)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var order = context.Order.Where(x => ListId.Contains((int)x.Id)).ToList();
                    order.ForEach(x =>
                    {
                        x.Status = (short)eStatusOrder;
                    });
                    return context.SaveChanges() > 0 ? error : error.Failed("Failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ResultOrderTempFilter> GetListOrderTempByFilter(SearchOrderTempFilter searchOrderTempFilter)
        {
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var orderTemps = context.ResultOrderTempFilter
                        .FromSqlRaw(
                        "EXECUTE dbo.SP_GetOrderTempByFilter @IdLiveStream ={0},@Status= {1},@Top= {2},@Page = {3}, @StartDate={4}, @EndDate={5}", searchOrderTempFilter.IdLiveStream, searchOrderTempFilter.Status,
                        searchOrderTempFilter.Top, searchOrderTempFilter.Page,searchOrderTempFilter.StartDate, searchOrderTempFilter.EndDate).ToList();
                    return orderTemps;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<OrderTemp> GetListOrderTempToExportExcel(SearchOrderTempFilter search)
        {
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var orderTemps = context.OrderTemp.Where(x => x.LiveStreamId == search.IdLiveStream && x.Status == search.Status && x.CreatedDateUtc >= search.StartDate && x.CreatedDateUtc <= search.EndDate).Include(x => x.Customer).Include(x => x.OrderDetail.Product.ProductInfo.Category).Include(x => x.LiveStream).ToList();
                    List<OrderTemp> temps = new List<OrderTemp>();
                    temps.AddRange(orderTemps);
                    for (int i = 0; i < temps.Count; i++)
                    {
                        var listOrderTempSame = orderTemps.Where(x => x.Customer.Phone == temps[i].Customer.Phone).ToList();
                        foreach (var item1 in listOrderTempSame)
                        {
                            item1.Count = listOrderTempSame.Count();
                            temps.Remove(item1);
                        }
                        i--;
                    }
                    return orderTemps.OrderBy(x => x.Count).ThenBy(x => x.OrderDetail.Product.Code).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ErrorObject UpdateStatusConfirmedListOrdertemp(List<OrderTemp> orderTemps)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    foreach (var item in orderTemps)
                    {
                        var ordertemp= context.OrderTemp.FirstOrDefault(x => x.Id == item.Id);
                        ordertemp.Status = (int)EStatusOrderTemp.Confirmed;
                    }
                    return context.SaveChanges() > 0 ? error : error.Failed("Failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int CountTotalOrderPreSaleByIdLiveStream(int IdLiveStream)
        {
            using (var context = new LiveStreamStoreContext())
            {
                return context.OrderTemp.Where(x => x.LiveStreamId == IdLiveStream && x.IsPreSale == true && x.Status != -1).Select(x => x.CustomerId).Distinct().ToList().Count();
            }
        }
        
        public int CountTotalOrderSaleLiveByIdLiveStream(int IdLiveStream)
        {
            using (var context = new LiveStreamStoreContext())
            {
                return context.OrderTemp.Where(x => x.LiveStreamId == IdLiveStream && x.IsPreSale == false && x.Status != -1).Select(x => x.CustomerId).Distinct().ToList().Count();
            }
        }

        public int CountTotalProductPreSaleByIdLiveStream(int IdLiveStream)
        {
            using (var context = new LiveStreamStoreContext())
            {
                return (int)context.OrderTemp.Where(x => x.LiveStreamId == IdLiveStream && x.IsPreSale == true && x.Status != -1).Include(x => x.OrderDetail).Select(x => x.OrderDetail.Quantity).Sum(x => x);
            }
        }

        public int CountTotalProductSaleLiveByIdLiveStream(int IdLiveStream)
        {
            using (var context = new LiveStreamStoreContext())
            {
                return (int)context.OrderTemp.Where(x => x.LiveStreamId == IdLiveStream && x.IsPreSale == false && x.Status != -1).Include(x => x.OrderDetail).Select(x => x.OrderDetail.Quantity).Sum(x => x);
            }
        }

        public int CountTotalRowOrderByIdLiveStream(int IdLiveStream, int AddressId)
        {
            using (var context = new LiveStreamStoreContext())
            {
                return context.Order.Where(x => x.LiveStreamId == IdLiveStream && (x.AddressId == null && AddressId == 0 || x.AddressId != null && AddressId == 1)).ToList().Count();
            }
        }
        public int CountCustomerPreSaleByStatus(int IdLiveStream, int status)
        {
            using (var context = new LiveStreamStoreContext())
            {
                return context.OrderTemp
                    .Where(x => x.LiveStreamId == IdLiveStream && x.IsPreSale == true && x.Status == status)
                    .Select(x => x.CustomerId).Distinct().Count();
            }
        }
        public ErrorObject DeleteOrderTempById(int OrderTempId)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var orderTemp = context.OrderTemp.FirstOrDefault(x => x.Id == OrderTempId);
                    orderTemp.Status = (int)EStatusOrderTemp.Deleted;
                    return context.SaveChanges() > 0 ? error : error.Failed("Failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ErrorObject RevertOrderTempById(int OrderTempId)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var orderTemp = context.OrderTemp.FirstOrDefault(x => x.Id == OrderTempId);
                    orderTemp.Status = (int)EStatusOrderTemp.Pending;
                    return context.SaveChanges() > 0 ? error : error.Failed("Failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ErrorObject CreateOrderTemp(OrderTemp orderTemp)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    orderTemp.Status = (int)EStatusOrderTemp.Pending;
                    context.OrderTemp.Add(orderTemp);
                    return context.SaveChanges() > 0 ? error : error.Failed("Failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ErrorObject CreateOrderDetail(OrderDetail orderDetail)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    orderDetail.TotalPrice = orderDetail.Price * orderDetail.Quantity;
                    context.OrderDetail.Add(orderDetail);
                    return context.SaveChanges() > 0 ? error.SetData(orderDetail) : error.Failed("Failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ErrorObject CreateOrder(int idLiveStream)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var orderTemps = context.OrderTemp.Where(x => x.LiveStreamId == idLiveStream).Include(x => x.OrderDetail.Product).Include(x => x.Customer.Address).Include(x => x.Customer.User).ToList();
                    var listOrderTemps = new List<OrderTemp>();
                    for (int i = 0; i < orderTemps.Count; i++)
                    {
                        for (int j = 0; j < orderTemps.Count; j++)
                        {
                            if (orderTemps[j].CustomerId == orderTemps[i].CustomerId)
                            {
                                listOrderTemps.Add(orderTemps[j]);
                            }
                        }
                        var orderTemp = listOrderTemps.FirstOrDefault();
                        var address = orderTemp.Customer.Address.FirstOrDefault();
                        double? totalPrice = listOrderTemps.Sum(x => x.OrderDetail.TotalPrice);
                        int? totalItem = listOrderTemps.Sum(x => x.OrderDetail.Quantity);

                        var code = Code();
                        Order order = new Order();
                        var lastCode = context.Order.ToList();
                        if (lastCode.Count == 0)
                        {
                            order.Code = "VUE" + code + "00" + 1;
                        }
                        else
                        {
                            order.Code = "VUE" + code + "00" + (Int32.Parse((lastCode[lastCode.Count - 1].Code).Substring(9)) + 1).ToString();
                        }
                        order.TotalItem = totalItem;
                        order.TotalPrice = totalPrice;
                        order.CreatedDateUtc = DateTime.UtcNow;
                        order.LiveStreamId = idLiveStream;
                        order.CustomerId = orderTemp.CustomerId;
                        order.UserId = orderTemp.Customer.UserId;
                        order.StoreId = orderTemp.Customer.User.StoreId;
                        order.AddressId = address?.Id;
                        context.Order.Add(order);
                        context.SaveChanges();
                        foreach (var item in listOrderTemps)
                        {
                            context.OrderDetail.Include(y => y.OrderTemp).FirstOrDefault(x => x.Id == item.OrderDetailId).OrderId = order.Id;
                            orderTemps.Remove(item);
                        }
                        listOrderTemps.Clear();
                        i = 0;
                    }
                    return context.SaveChanges() > 0 ? error : error.Failed("Failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string Code()
        {
            var splitDate = (DateTime.Now.ToString("yyyy-MM-dd")).Split('-');
            var date = splitDate[2];
            var month = splitDate[1];
            var year = splitDate[0].Substring(splitDate[0].Length - 1);
            switch (Int32.Parse(month))
            {
                case 1:
                    month = "A";
                    break;
                case 2:
                    month = "B";
                    break;
                case 3:
                    month = "C";
                    break;
                case 4:
                    month = "D";
                    break;
                case 5:
                    month = "E";
                    break;
                case 6:
                    month = "F";
                    break;
                case 7:
                    month = "G";
                    break;
                case 8:
                    month = "H";
                    break;
                case 9:
                    month = "I";
                    break;
                case 10:
                    month = "J";
                    break;
                case 11:
                    month = "K";
                    break;
                case 12:
                    month = "L";
                    break;
            }

            var code = (year + month + date);
            return code;
        }

        public List<OrderTemp> GetOrderTempByCustomer(int IdCustomer, int pageNumber)
        {
            var error = Error.Success();
            try
            {
                int pageSize = 15;
                using (var context = new LiveStreamStoreContext())
                {
                    var result = context.OrderTemp.Where(x => x.CustomerId == IdCustomer).Include(x => x.OrderDetail.Product.ProductInfo.Image).OrderByDescending(x => x.CreatedDateUtc).ToList().Skip(pageNumber * pageSize).Take(pageSize).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ErrorObject AddQuantityToProduct(int? ProductId, int? Quantity)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var product = context.Product.Where(x => x.Id == ProductId).FirstOrDefault();
                    if (product.NumberProductSold == null)
                    {
                        product.NumberProductSold = Quantity;
                    }
                    else
                    {
                        product.NumberProductSold += Quantity;
                    }
                    context.Product.Update(product);
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
