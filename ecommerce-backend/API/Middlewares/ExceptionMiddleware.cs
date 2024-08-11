using API.Errors;
using System.Net;
using System.Text.Json;

namespace API.Middlewares
{
  public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleware> logger;
        private readonly IHostEnvironment env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            this.next = next;
            this.logger = logger;
            this.env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next.Invoke(httpContext);
            }
            catch (Exception ex)
            {
                // -- If We In Production Environment: We Log This Error In Database Or Files

                // -- If We In Development Environment: We Log This Error In Console Like This
                logger.LogError(ex, ex.Message);

                // -- After Logged Exception: The Frontend Developer Waiting From Us Response
                // -- And Response Consists Of Header And Body
                // .... This is What We Need To Send In Header
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                // .... This is What We Need To Send In Body
                // .... If We In Development Environment: We Send (Status Code, Exception Message, Exception Details) To FrontEnd
                // .... If We In Production Environment: We Send (Status Code) To Client And We Send (Exception Message, Exception Details) To Database | File
                var response = env.IsDevelopment() ?
                    new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString()) :
                    new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);

                // Here We Create Options To Make Json Be Camel Case And We Send This Options To JsonSerializer 
                var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                var json = JsonSerializer.Serialize(response, options);

                // .... Here We Send Body To Response
                await httpContext.Response.WriteAsync(json);
            }
        }
    }
}