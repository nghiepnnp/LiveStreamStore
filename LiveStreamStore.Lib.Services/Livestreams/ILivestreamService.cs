using System;
using System.Collections.Generic;
using System.Text;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models;
using LiveStreamStore.Lib.Models.Enums;
using LiveStreamStore.Lib.Models.SearchFilter;

namespace LiveStreamStore.Lib.Services.Livestreams
{
    public interface ILivestreamService
    {
        List<ResultLiveStreamFilter> GetListLiveStreamByUserStore(SearchLiveStreamFilter searchFilter);
        LiveStream GetLiveStreamById(int id);
        ErrorObject InsertLiveStream(LiveStream liveStream);
        ErrorObject InsertCopyLiveStream(LiveStream liveStream);
        ErrorObject UpdateLiveStream(LiveStream liveStream);
        bool CheckIsStreamming(int livestreamId);
        ErrorObject UpdateIsStreamingLiveStream(int liveStreamId,string link, bool isStreaming);
        ErrorObject UpdateStatusLiveStream(int id, ELiveStreamStatus eLiveStreamStatus);
        List<LiveStream> GetLiveStreamByStreaming(string storeCode);
        List<LiveStream> GetLiveStreamByStoreCode(string StoreCode);
        ErrorObject SetPresale(int livestreamId);
        int GetUserIdLiveStreamByLiveStreamId(int liveStreamId);
    }
}
