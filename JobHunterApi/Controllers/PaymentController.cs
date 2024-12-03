using System.Web;
using JobHunterApi.Models;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace JobHunterApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {

        private const string SecretKey = "sk_test_51QRTA0GrGPg4V8UdtKTPXWtCHB57zpiyPFkX9LCh0bLDGYuKoEju52f1hZcbUWULzA9d5UYRoaRP2AfG2PoLTahq00MDlXNQts";

        public PaymentController()
        {
            StripeConfiguration.ApiKey = SecretKey;
        }

        [HttpPost("create-checkout-session")]
        public ActionResult CreateCheckoutSession([FromBody] RegisterModel registerdetails)
        {

            var registerDetailsJson = Newtonsoft.Json.JsonConvert.SerializeObject(registerdetails);
            var encodedDetails = HttpUtility.UrlEncode(registerDetailsJson);
            var successUrl="http://localhost:4200/registerform?registerdetails="+encodedDetails;
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = 1000, // Amount in cents (e.g., $20.00)
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Job Hunter",
                                Images = new List<string>
                                {
                                    "https://idoxacx.sufydely.com/JobHunterLogo.png" 
                                }
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = "http://localhost:4200/registerform/registerdetails=fail",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return Ok(new { sessionId = session.Id });
        }
    }
}