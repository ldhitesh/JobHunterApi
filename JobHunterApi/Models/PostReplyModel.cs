using System;
using System.Collections.Generic;

namespace JobHunterApi.Models
{
    public class PostReplyModel
    {
        public int post_id { get; set; }
        public string title { get; set; }
        public string author { get; set; } 
        public string summary { get; set; }
        public string user_id { get; set; }
        public string posted_date { get; set; }
        public string postprofilepic{ get; set; }
        public List<RepliesModel> replies { get; set; } = new List<RepliesModel>();
    }
}
