
using Core.Authentication;

namespace API.Errors
{
    //this class handles the middleware exception
    public class ApiExceptionResponse : ApiResponse
    {
        public string? Details { get; set; }

        public ApiExceptionResponse(int statusCode, string? message = null, string? details = null) :
            base(statusCode, message)
        {
            Details = details;
        }
    }
}
