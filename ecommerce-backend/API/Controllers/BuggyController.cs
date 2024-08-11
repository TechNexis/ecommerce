using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Tnef;

namespace ecommerce_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuggyController(ILogger<BuggyController> logger) : ControllerBase
    {
        private readonly ILogger<BuggyController> _logger = logger;


        [HttpGet("nullReference")]
        public ActionResult GetNullReferenceException()
        {
            string value = null;
            _logger.LogInformation("Attempting to get length of null string ");
            return Ok(value.Length);
        }
        [HttpGet("DivideByZero")]
        public ActionResult GetDivideByZeroException(int num)
        {
            int divisor = 0;
            _logger.LogInformation("Attempting to divide by zero");
            return Ok(num / divisor);
        }
        [HttpGet("NotFound")]
        public ActionResult GotNotFoundException()
        {
            _logger.LogInformation("throwing a not found exception");
            return NotFound("This resource was not found");
        }
    }

}
