using System;
using System.Collections.Generic;
using System.Text;

namespace LiveStreamStore.Lib.Models.SearchFilter
{
    public class SearchOrderTempFilter
    {
        public int Top { get; set; } = 10;
        public int Page { get; set; } = 1;
        public int Status { get; set; } = 0; // 0: Chưa chốt, 1: Đã chốt, -1: Đã xóa 
        public int IdLiveStream { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
