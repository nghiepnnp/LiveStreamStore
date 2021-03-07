using System;
using System.Collections.Generic;
using System.Text;
using LiveStreamStore.Lib.Models;
using LiveStreamStore.Lib.Models.Api;

namespace LiveStreamStore.Lib.Services.ApiDashBoard
{
    public interface IApiDashBoard
    {
        ErrorObject GetAccessToken(string url);
        ErrorObject RefreshToken(string url);
        ErrorObject CallApi(string domain, string path, string jsonparamater);
        ErrorObject ApiGetCustomerInfo(string Phone, int StoreId);
        ErrorObject ApiCreateCustomer(CustomerInfoLiveStream customerInfoLiveStream);
        ErrorObject ApiUpdateRecipient(RecipientsInfo recipientsInfo);
        ErrorObject ApiCreateListOrder(List<LiveStreamOrderItem> liveStreamOrderItems);
        ErrorObject ApiCreateOrder(LiveStreamOrderItem liveStreamOrderItem);
    }
}
