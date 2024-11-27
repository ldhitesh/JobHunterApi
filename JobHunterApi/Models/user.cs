using Microsoft.AspNetCore.Identity;

namespace JobHunterApi.Models
{
    public class User:IdentityUser
    {
        public string UserName { get; set; }=string.Empty;
        public string Password { get; set; }=string.Empty;
    }

}