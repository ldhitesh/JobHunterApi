namespace JobHunterApi.Models
{
    public class RegisterModel
    {
        public int Id { get; set; }
        public string Username { get; set; }=string.Empty;
        public string Email { get; set; } =string.Empty;
        public string Password { get; set; }=string.Empty;
        public string ConfirmPassword { get; set; }=string.Empty;
        public string PaymentStatus { get; set; }=string.Empty;
        public string VerificationToken { get; set; }=string.Empty;
        public string AccountVerificationStatus { get; set; }="Not Verified";
        
    }
}