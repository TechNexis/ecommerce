using Core.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce_backend.Controllers
{
    [Route("error/{code}")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        // -- Here Swagger Doesn't Work Because End Point (Error) Not Have Any Method (Get|Post..) And We Can't Put Method 
        // -- Because We Don't Call This End Point But Application Will Do It
        // -- So To Solving This Problem We Write
        public ActionResult Error(int code)
        {
            return  new ObjectResult(new ApiResponse(code));
        }
    }
}
