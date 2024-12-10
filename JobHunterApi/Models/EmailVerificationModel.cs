using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobHunterApi.Models
{
    public class EmailVerificationModel
    {
         public string Email { get; set; }=string.Empty;
        public string VerificationToken { get; set; }=string.Empty;
        public string Status { get; set; }="Not Verified";

    }
}