using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobHunterApi.Models
{
    public class RepliesModel
    {
        
        public int post_id { get; set; }
        public int reply_id { get; set; }
        public string user_id { get; set; }
        public string reply_summary { get; set; } 
        public string replied_on { get; set; }
        public string username { get; set; }
        public string replyprofilepic{ get; set; }


    }
}