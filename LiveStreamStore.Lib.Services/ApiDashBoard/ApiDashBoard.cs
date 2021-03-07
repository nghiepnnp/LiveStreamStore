using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LiveStreamStore.Lib.Models;
using LiveStreamStore.Lib.Models.Api;

namespace LiveStreamStore.Lib.Services.ApiDashBoard
{
    public class ApiDashBoard : IApiDashBoard
    {
       
        public AuthToken authTokens { get; set; } = null;
        private readonly IConfiguration _IConfiguration;
        private static string UserName;
        private static string PassWord;
        private static string Domain;
        public ApiDashBoard(IConfiguration configuration)
        {
            _IConfiguration = configuration;
            UserName = _IConfiguration["ApiDashBoard:Account:UserName"];
            PassWord = _IConfiguration["ApiDashBoard:Account:PassWord"];
            Domain = _IConfiguration["ApiDashBoard:Account:Domain"];
        }
        public  ErrorObject GetAccessToken(string url)
        {
            var err = new ErrorObject(Error.SUCCESS);
            using (var client = new HttpClient())
            {
                var dict = new Dictionary<string, string>();
                dict.Add("username", UserName);
                dict.Add("password", PassWord);
                dict.Add("grant_type", "password");

                var req = new HttpRequestMessage(HttpMethod.Post, url+"/token") { Content = new FormUrlEncodedContent(dict) };
                var res =  client.SendAsync(req).Result;
                var content = res.Content.ReadAsStringAsync().Result;
                if (res.StatusCode == HttpStatusCode.OK)
                {             
                    Dictionary<string, object> rs = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                 
                    DateTime start = DateTime.Now;
                    var exp = start.AddSeconds(int.Parse(rs["expires_in"].ToString()));// VN: utc + 7
                    authTokens = new AuthToken()
                    {
                        access_token = rs["access_token"].ToString(),
               
                        expires_in = exp,
                        refresh_token = rs["refresh_token"].ToString()
                    };
                    

                }
                else err.Failed(res);
                //var login = JsonConvert.SerializeObject(new { username = username, password = password , grant_type = "password" });
                //var dataAccesToken = new StringContent(login, Encoding.UTF8, "application/x-www-form-urlencoded");
                //var responseAccesToken = client.PostAsync(url + "/token", dataAccesToken).Result;


                //var content = responseAccesToken.Content.ReadAsStringAsync().Result;
                //if (responseAccesToken.StatusCode == HttpStatusCode.OK)
                //{

                //    authTokens = JsonConvert.DeserializeObject<AuthToken>(content);


                //}
                //else err.Failed(res);
            }
            return err;
        }
        public ErrorObject RefreshToken(string url)
        {
            var err = new ErrorObject(Error.SUCCESS);
            using (var client = new HttpClient())
            {
                var dict = new Dictionary<string, string>();
                dict.Add("refresh_token", authTokens.refresh_token);
                dict.Add("client_id", UserName);
                dict.Add("grant_type", "refresh_token");

                var req = new HttpRequestMessage(HttpMethod.Post, url + "/token") { Content = new FormUrlEncodedContent(dict) };
                var res = client.SendAsync(req).Result;
                var content = res.Content.ReadAsStringAsync().Result;

                if (res.StatusCode == HttpStatusCode.OK)
                {
                    Dictionary<string, object> rs = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                    DateTime start = DateTime.Now;
                    var exp = start.AddSeconds(int.Parse(rs["expires_in"].ToString()));// VN: utc + 7
                    authTokens = new AuthToken()
                    {
                        access_token = rs["access_token"].ToString(),
                        expires_in = exp,
                        refresh_token = rs["refresh_token"].ToString()
                    };
                    

                }
                else err.Failed(content);
            }
            return err;
        }
       /// <summary>
       /// 
       /// </summary>
       /// <param name="domain"></param>
       /// <param name="path"></param>
       /// <param name="jsonparamater"></param>
       /// <param name="username"></param>
       /// <param name="password"></param>
       /// <returns></returns>
        public ErrorObject CallApi(string domain, string path, string jsonparamater)
        {
            var err = new ErrorObject(Error.SUCCESS);
            try
            {
                if (authTokens == null)
                {
                    var result = GetAccessToken(domain);
                    if (result.Code != Error.SUCCESS.Code)
                    {
                        return result;

                    }
                }
            
                using (var client = new HttpClient())
                {

                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authTokens.access_token);

                    var data = new StringContent(jsonparamater, Encoding.UTF8, "application/json");
                    var response = client.PostAsync(domain + path, data).Result;

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        if (authTokens.expires_in<=DateTime.Now)
                        {
                            if (RefreshToken(domain).Code ==Error.SUCCESS.Code)
                            {
                                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authTokens.access_token);                            
                                response = client.PostAsync(domain + path, data).Result;                             
                            }
                        }
                        else
                        {
                            return err.Failed("Token không chính xác");
                        }

                    }
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        var rs = JsonConvert.DeserializeObject<ErrorObject>(content);
                        //var resultgetinfo = ResultGetInfoCustomer.FromJson(content);
                        if (rs.Code == Error.SUCCESS.Code)
                        {
                            err.SetData(rs.Data);
                        }
                        else err.Failed(rs.Data);
                    }
                    else err.Failed(response.Content);
                }
            }
            catch (Exception ex)
            {
                err.Failed(ex.Message);
            }
            return err;
        }
        public ErrorObject ApiGetCustomerInfo(string Phone, int StoreId)
        {
            var jsonParameter = JsonConvert.SerializeObject(new { Phone = Phone, StoreId = StoreId });
            var result = CallApi(Domain, "/api/CustomerLiveStream/GetCustomerInfo", jsonParameter);
            if (result.Code == Error.SUCCESS.Code)
            {
                result.Data = JsonConvert.DeserializeObject<ResultGetInfoCustomer>(JsonConvert.SerializeObject(result.Data));
            }        
            return result;
        }
        public ErrorObject ApiCreateCustomer(CustomerInfoLiveStream customerInfoLiveStream)
        {
        
            var jsonParameter = JsonConvert.SerializeObject(customerInfoLiveStream);
            return CallApi(Domain, "/api/CustomerLiveStream/CreateCustomer", jsonParameter);
        }
        public ErrorObject ApiUpdateRecipient(RecipientsInfo recipientsInfo)
        {
            var jsonParameter = JsonConvert.SerializeObject(recipientsInfo);
            return CallApi(Domain, "/api/CustomerLiveStream/UpdateRecipient", jsonParameter);
        }
        public ErrorObject ApiCreateListOrder(List<LiveStreamOrderItem> liveStreamOrderItems)
        {
            var jsonParameter = JsonConvert.SerializeObject(liveStreamOrderItems);
            return CallApi(Domain, "/api/OrderLiveStream/CreateListOrder", jsonParameter);
        }
        public ErrorObject ApiCreateOrder(LiveStreamOrderItem liveStreamOrderItem)
        {
            var jsonParameter = JsonConvert.SerializeObject(liveStreamOrderItem);
            return CallApi(Domain, "/api/OrderLiveStream/CreateOrder", jsonParameter);
        }
    }

}
