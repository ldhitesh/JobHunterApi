

namespace JobHunterApi.Models
{
    public class RequestBody
    {
      
        public RegisterModel ?RegisterData { get; set; }
        public EmailModel ?EmailVerificationData { get; set; }
    }
}