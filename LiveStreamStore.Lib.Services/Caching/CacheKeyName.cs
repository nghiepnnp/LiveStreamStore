
namespace LiveStreamStore.Lib.Services.Caching
{
    public static class CacheKeyName
    {

        #region Web

        /// <summary>
        /// Key for comment in streaming
        /// </summary>
        /// /// <remarks>
        /// {0} : Id livestream
        /// </remarks>      
        /// 
        public static string CustomerPrefix => "Customer";

        public static CacheKey AllCustomer => new CacheKey("AllCustomer", 24*60, CustomerPrefix);

        /// <summary>
        /// Key for comment in streaming
        /// </summary>
        /// /// <remarks>
        /// {0} : Id livestream
        /// </remarks>      
        /// 
        public static string LiveStreamPrefix => "LiveStream";

        public static CacheKey IsStreaming => new CacheKey("IsStreaming-{0}", 6 * 60, LiveStreamPrefix);

        #endregion


    }
}
