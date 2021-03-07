using System;
using System.Collections.Generic;
using System.Text;

namespace LiveStreamStore.Lib.Models.SearchFilter
{
    public class SearchLiveStreamFilter
    {
        public int StoreId { get; set; }
        public int Top { get; set; } = 10;
        public int Page { get; set; } = 1;
        public DateTime StartDate { get; set; } = new DateTime(2020, 1, 1);
        public DateTime EndDate { get; set; } = DateTime.UtcNow;
    }
}
