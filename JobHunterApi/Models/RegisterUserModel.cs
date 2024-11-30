using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobHunterApi.Models
{
    public class RegisterUserModel
    {
        public string Username { get; set; }=string.Empty;
        public string Email { get; set; } =string.Empty;
    }
}