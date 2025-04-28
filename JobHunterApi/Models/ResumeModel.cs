using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobHunterApi.Models
{
    public class ResumeModel
    {
        public int ResumeDataId { get; set; }

        public string user_id { get; set; }

        public string details { get; set; }  
        
        public string resumecreated {get; set;}
    }
}