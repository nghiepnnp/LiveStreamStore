using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LiveStreamStore.Lib.Models.Api
{
    public  class LiveStreamOrderItem
    {
        public long Id { get; set; }
        [Required]
        public int StoreId { get; set; }
        [Required]
        public string PackageCode { get; set; }
        public DateTime LiveStreamDate { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string CategoryName { get; set; }
        [Required]
        public decimal Weight { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        [Required]
        public string Source { get; set; } // Nguồn gọi api presale or livestream
      
        public int Status { get; set; }
        public string Message { get; set; }
    }
}
