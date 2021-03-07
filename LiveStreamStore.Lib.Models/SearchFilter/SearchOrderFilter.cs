using System;
using System.Collections.Generic;
using System.Text;

namespace LiveStreamStore.Lib.Models.SearchFilter
{
    public class SearchOrderFilter
    {
        public int Top { get; set; } = 10;
        public int Page { get; set; } = 1;
        public int IdLiveStream { get; set; }
        public int AddressId { get; set; } = 999;
    }
}
