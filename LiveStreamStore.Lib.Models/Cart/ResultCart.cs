using System;
using System.Collections.Generic;
using System.Text;

namespace LiveStreamStore.Lib.Models.Cart
{
    public class ResultCart
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int Limited { get; set; }
        public int LiveStreamId { get; set; }
        public string StoreCode { get; set; }
    }
}
