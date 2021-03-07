using System;
using System.Collections.Generic;
using System.Text;

namespace LiveStreamStore.Lib.Models.Api
{
    public class AuthToken
    {
        public string access_token { set; get; } 
        public DateTime expires_in { get; set; }
        public string refresh_token { get; set; }
 

    }
}
