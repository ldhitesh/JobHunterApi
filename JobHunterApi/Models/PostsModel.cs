using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobHunterApi.Models
{
    public class PostsModel
    {
        public int post_id { get; set; }
        public string title { get; set; }
        public string author { get; set; } 
        public string summary { get; set; }
        public string user_id { get; set; }
        public string posted_date { get; set; }
    }
}