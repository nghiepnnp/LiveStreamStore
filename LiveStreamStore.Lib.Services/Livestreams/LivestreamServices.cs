using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models;
using LiveStreamStore.Lib.Models.Enums;
using LiveStreamStore.Lib.Models.SearchFilter;
using LiveStreamStore.Lib.Services.Caching;
using LiveStreamStore.Lib.Services.Orders;

namespace LiveStreamStore.Lib.Services.Livestreams
{
    public class LivestreamServices : ILivestreamService
    {
        private ICacheService _CacheService;
        private readonly IOrderService _OrderService;
        public LivestreamServices(ICacheService cacheService, IOrderService orderService) {
            _CacheService = cacheService;
            _OrderService = orderService;
        }

        public List<ResultLiveStreamFilter> GetListLiveStreamByUserStore(SearchLiveStreamFilter searchFilter)
        {
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var result = context.ResultLiveStreamFilter.FromSqlRaw("EXECUTE dbo.SP_GetLiveStreamByFilter @StoreId = {0}, @Top = {1}, @Page = {2}, @StartDate = {3}, @EndDate ={4}", searchFilter.StoreId, searchFilter.Top, searchFilter.Page, searchFilter.StartDate, searchFilter.EndDate).ToList();
                    result.ForEach(x =>
                    {
                        x.TotalOrderPreSale = _OrderService.CountTotalOrderPreSaleByIdLiveStream(x.Id);
                        x.TotalOrderSaleLive = _OrderService.CountTotalOrderSaleLiveByIdLiveStream(x.Id);
                        x.TotalProductPreSale = _OrderService.CountTotalProductPreSaleByIdLiveStream(x.Id);
                        x.TotalProductSaleLive = _OrderService.CountTotalProductSaleLiveByIdLiveStream(x.Id);
                    });
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LiveStream GetLiveStreamById(int id)
        {
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    return context.LiveStream.Where(x => x.Id == id).Include(x => x.Product).Include(x => x.User.Store).SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ErrorObject InsertLiveStream(LiveStream liveStream)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    liveStream.Status = (short)ELiveStreamStatus.Active;
                    liveStream.CreatedDateUtc = DateTime.UtcNow;
                    context.LiveStream.Add(liveStream);
                  
                    return context.SaveChanges() > 0 ? error.SetData(liveStream) : error.Failed("Failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ErrorObject InsertCopyLiveStream(LiveStream liveStream)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var products = context.Product.Where(x => x.LiveStreamId == liveStream.Id && x.Status != (short)EProductStatus.Deleted).ToList();
                    var ii =  products;
                    if (products.Count() == 0)
                    {   
                        return error.Failed("Oh!! Không có gì để sao chép.");
                    }

                    LiveStream liveS = new LiveStream();
                    liveS.UserId = liveStream.UserId;
                    liveS.Link = liveStream.Link;
                    liveS.Description = liveStream.Description;
                    liveS.CreatedDateUtc = DateTime.UtcNow;
                    liveS.Status = (short)ELiveStreamStatus.Active;

                    context.LiveStream.Add(liveS);
                    error.SetData(liveS);
                    return context.SaveChanges() > 0 ? error : error.Failed("Thất bại");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ErrorObject UpdateLiveStream(LiveStream liveStream)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var livestreamOld = context.LiveStream.Where(x => x.Id == liveStream.Id).FirstOrDefault();
                    livestreamOld.Link = liveStream.Link;
                    livestreamOld.Description = liveStream.Description;
                    context.LiveStream.Update(livestreamOld);
                    return context.SaveChanges() > 0 ? error : error.Failed("Failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ErrorObject SetPresale(int livestreamId)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var livestream = context.LiveStream.Where(x => x.Id == livestreamId).FirstOrDefault();
                    livestream.OpenPreSale = !livestream.OpenPreSale ?? true;
                    return context.SaveChanges() > 0 ? error.SetData(livestream.OpenPreSale) : error.Failed("Failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CheckIsStreamming(int livestreamId)
        {
            try
            {
                var cacheKey= _CacheService.CreateCacheKey(CacheKeyName.IsStreaming, livestreamId);
                var liveStream = _CacheService.Get<LiveStream>(cacheKey);
                if (liveStream == null)
                {
                    using (var context = new LiveStreamStoreContext())
                    {
                        liveStream = context.LiveStream.FirstOrDefault(x => x.Id == livestreamId);
                        if (liveStream.IsStreaming == true)
                        {
                            _CacheService.Set(cacheKey, liveStream);
                        }
                        return liveStream.IsStreaming ?? false;
                    }
                }
                return liveStream?.IsStreaming ?? true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ErrorObject UpdateIsStreamingLiveStream(int liveStreamId, string link ,bool isStreaming)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var livestream = context.LiveStream.Where(x => x.Id == liveStreamId).FirstOrDefault();
                    if (isStreaming)
                    {
                        livestream.LivestreamStartTime = DateTime.UtcNow;
                        livestream.Link = link;
                    }
                    else
                    {
                        _CacheService.RemoveByPrefix(CacheKeyName.LiveStreamPrefix);
                    }
                    livestream.IsStreaming = isStreaming;
                    error.SetData(livestream);
                    return context.SaveChanges() > 0 ? error : error.Failed("Failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ErrorObject UpdateStatusLiveStream(int id, ELiveStreamStatus eLiveStreamStatus)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var livestream = context.LiveStream.Where(x => x.Id == id).FirstOrDefault();
                    livestream.Status = (short)eLiveStreamStatus;
                    return context.SaveChanges() > 0 ? error : error.Failed("Failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<LiveStream> GetLiveStreamByStreaming(string storeCode)
        {
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    return context.LiveStream.Where(x => x.OpenPreSale == true && x.User.Store.Code == storeCode).Include(x => x.Product)
                        .Include(x => x.User).OrderByDescending(x => x.CreatedDateUtc).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetUserIdLiveStreamByLiveStreamId(int liveStreamId)
        {
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    return context.LiveStream.FirstOrDefault(x => x.Id == liveStreamId).UserId ?? 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 

        public List<LiveStream> GetLiveStreamByStoreCode(string StoreCode)
        {
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    var result = context.User.Where(x => x.Store.Code == StoreCode).Include(x => x.LiveStream).FirstOrDefault();
                    return context.LiveStream.Where(x => x.UserId == result.Id).Include(x => x.Product).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
