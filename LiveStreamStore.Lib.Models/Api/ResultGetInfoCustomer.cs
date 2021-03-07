
using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LiveStreamStore.Lib.Models.Api
{
    


    public partial class ResultGetInfoCustomer
    {
        [JsonProperty("Id")]
        public long Id { get; set; }

        [JsonProperty("StoreId")]
        public long StoreId { get; set; }

        [JsonProperty("Fullname")]
        public string Fullname { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("Phone")]
        public string Phone { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }

        [JsonProperty("SenderId")]
        public long SenderId { get; set; }

        [JsonProperty("RewardPoint")]
        public long RewardPoint { get; set; }

        [JsonProperty("RewardAmount")]
        public long RewardAmount { get; set; }

        [JsonProperty("CreatedDate")]
        public DateTimeOffset CreatedDate { get; set; }

        [JsonProperty("ModifiedDate")]
        public DateTimeOffset ModifiedDate { get; set; }

        [JsonProperty("Status")]
        public long Status { get; set; }

        [JsonProperty("Avatar")]
        public object Avatar { get; set; }

        [JsonProperty("TypeId")]
        public object TypeId { get; set; }

        [JsonProperty("CustomerCode")]
        public object CustomerCode { get; set; }

        [JsonProperty("tblStoreAccount")]
        public TblStoreAccount TblStoreAccount { get; set; }

        [JsonProperty("tblStoreCustomerType")]
        public object TblStoreCustomerType { get; set; }

        [JsonProperty("tblTrackingItems")]
        public object[] TblTrackingItems { get; set; }

        [JsonProperty("RecipientsInfos")]
        public RecipientsInfo[] RecipientsInfos { get; set; }
    }

    public partial class TblStoreAccount
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("FullName")]
        public string FullName { get; set; }

        [JsonProperty("StoreName")]
        public string StoreName { get; set; }

        [JsonProperty("Address1")]
        public object Address1 { get; set; }

        [JsonProperty("Address2")]
        public object Address2 { get; set; }

        [JsonProperty("CityId")]
        public object CityId { get; set; }

        [JsonProperty("DistrictId")]
        public object DistrictId { get; set; }

        [JsonProperty("StateId")]
        public object StateId { get; set; }

        [JsonProperty("WarehouseId")]
        public long WarehouseId { get; set; }

        [JsonProperty("TypeId")]
        public object TypeId { get; set; }

        [JsonProperty("Zip")]
        public object Zip { get; set; }

        [JsonProperty("Phone")]
        public string Phone { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("Password")]
        public object Password { get; set; }

        [JsonProperty("Status")]
        public long Status { get; set; }

        [JsonProperty("CreatedDate")]
        public DateTimeOffset CreatedDate { get; set; }

        [JsonProperty("ModifiedDate")]
        public DateTimeOffset ModifiedDate { get; set; }

        [JsonProperty("Code")]
        public string Code { get; set; }

        [JsonProperty("AliasFullName")]
        public string AliasFullName { get; set; }

        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }

        [JsonProperty("Logo")]
        public string Logo { get; set; }

        [JsonProperty("StoreType")]
        public long StoreType { get; set; }

        [JsonProperty("tblCustomerUnregistereds")]
        public object[] TblCustomerUnregistereds { get; set; }

        [JsonProperty("tblKerryDeliveryArea_Province_Mapping")]
        public object[] TblKerryDeliveryAreaProvinceMapping { get; set; }

        [JsonProperty("tblKerryDeliveryService_Store_Mapping")]
        public object[] TblKerryDeliveryServiceStoreMapping { get; set; }

        [JsonProperty("tblKerryShippingPrice_Store_Mapping")]
        public object[] TblKerryShippingPriceStoreMapping { get; set; }

        [JsonProperty("tblOrders")]
        public object[] TblOrders { get; set; }

        [JsonProperty("tblPackageInfoes")]
        public object[] TblPackageInfoes { get; set; }

        [JsonProperty("tblRecipientsInfoes")]
        public RecipientsInfo[] TblRecipientsInfoes { get; set; }

        [JsonProperty("tblSenders")]
        public object[] TblSenders { get; set; }

        [JsonProperty("tblStoreType")]
        public object TblStoreType { get; set; }

        [JsonProperty("tblTypeAcount")]
        public object TblTypeAcount { get; set; }

        [JsonProperty("tblWarehouse")]
        public object TblWarehouse { get; set; }

        [JsonProperty("tblStoreCustomers")]
        public object TblStoreCustomers { get; set; }

        [JsonProperty("tblStoreEmployees")]
        public object[] TblStoreEmployees { get; set; }
    }

    public partial class RecipientsInfo
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("FullName")]
        public string FullName { get; set; }

        [JsonProperty("Add1")]
        public string Add1 { get; set; }

        [JsonProperty("Add2")]
        public string Add2 { get; set; }

        [JsonProperty("CityId")]
        public string CityId { get; set; }

        [JsonProperty("DistrictId")]
        public string DistrictId { get; set; }

        [JsonProperty("Phone")]
        public string Phone { get; set; }

        [JsonProperty("StoreId")]
        public long StoreId { get; set; }

        [JsonProperty("StoreCustomerId")]
        public long StoreCustomerId { get; set; }

        [JsonProperty("WardId")]
        public string WardId { get; set; }

        [JsonProperty("CreateDate")]
        public long CreateDate { get; set; }

        [JsonProperty("UpdateDate")]
        public object UpdateDate { get; set; }

        [JsonProperty("FullAddress")]
        public string FullAddress { get; set; }

        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("CountryId")]
        public long CountryId { get; set; }

        [JsonProperty("tblStoreAccount", NullValueHandling = NullValueHandling.Ignore)]
        public TblStoreAccount TblStoreAccount { get; set; }
    }

    public partial class ResultGetInfoCustomer
    {
        public static ResultGetInfoCustomer FromJson(string json) => JsonConvert.DeserializeObject<ResultGetInfoCustomer>(json, LiveStreamStore.Lib.Models.Api.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this ResultGetInfoCustomer self) => JsonConvert.SerializeObject(self, LiveStreamStore.Lib.Models.Api.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
