using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LiveStreamStore.Lib.Models.Api
{
    public class CustomerInfoLiveStream
    {
      
        public int Stt { get; set; }

        public string Code { get; set; }


        public string FullName { get; set; }

        [Required]
        public string Phone { get; set; } 


        public string Address { get; set; }


        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string Message { get; set; }
        public string ProvinceName { get; set; }
        [Required]
        public string ProvinceId { get; set; }
        public string DistrictName { get; set; }
        [Required]
        public string DistrictId { get; set; }
        public string WardName { get; set; }
        [Required]
        public string WardId { get; set; }
        [Required]
        public int StoreId { get; set; }
        [Required]
        public string Email { get; set; }



    }
}
