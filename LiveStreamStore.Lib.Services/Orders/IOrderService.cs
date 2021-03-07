using System;
using System.Collections.Generic;
using System.Text;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models;
using LiveStreamStore.Lib.Models.Enums;
using LiveStreamStore.Lib.Models.SearchFilter;


namespace LiveStreamStore.Lib.Services.Orders
{
    public interface IOrderService
    {
        ErrorObject CreateOrderTemp(OrderTemp orderTemp);
        ErrorObject CreateOrderDetail(OrderDetail orderDetail);
        List<ResultOrderFilter> GetListOrderByIdLiveStream(SearchOrderFilter searchOrderFilter);
        Order GetOrderById(int id);
        ErrorObject UpdateStatusOrder(int id, EStatusOrder eStatusOrder);
        ErrorObject UpdateListStatusOrder(List<int> ListId, EStatusOrder eStatusOrder);
        List<ResultOrderTempFilter> GetListOrderTempByFilter(SearchOrderTempFilter searchOrderTempFilter);
        List<OrderTemp> GetListOrderTempToExportExcel(SearchOrderTempFilter search);
        ErrorObject UpdateStatusConfirmedListOrdertemp(List<OrderTemp> orderTemps);
        int CountTotalOrderPreSaleByIdLiveStream(int IdLiveStream);
        int CountTotalOrderSaleLiveByIdLiveStream(int IdLiveStream);
        int CountTotalProductPreSaleByIdLiveStream(int IdLiveStream);
        int CountTotalProductSaleLiveByIdLiveStream(int IdLiveStream);
        int CountTotalRowOrderByIdLiveStream(int IdLiveStream, int AddressId);
        int CountCustomerPreSaleByStatus(int IdLiveStream, int status);
        ErrorObject CreateOrder(int idLiveStream);
        ErrorObject AddQuantityToProduct(int? ProductId, int? Quantity);
        List<OrderTemp> GetOrderTempByCustomer(int IdCustomer, int pageNumber);
        ErrorObject DeleteOrderTempById(int OrderTempId);
        ErrorObject RevertOrderTempById(int OrderTempId);
    }
}
