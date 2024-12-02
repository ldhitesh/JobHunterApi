using Microsoft.AspNetCore.Mvc;

namespace JobHunterApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {

        [HttpPost]
        public IActionResult MakePayment()
        {
            return Ok();
        }
    }
}