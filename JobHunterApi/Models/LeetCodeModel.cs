using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobHunterApi.Models
{
    public class LeetCodeModel
    {
        public int problem_id{ get; set; }
        public string problem_link { get; set; }="";
        public string problem_notes { get; set; }="";
        public string user_id { get; set; }="";
        public string solved_date { get; set; }="";

    }
}