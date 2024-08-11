using System.Diagnostics;

namespace ecommerce_backend.Middlewares
{
    public class ProfilingMiddleware(ILogger<ProfilingMiddleware>logger,RequestDelegate next)
    {
        private readonly ILogger<ProfilingMiddleware> _logger=logger;
        private readonly RequestDelegate _next=next;

        public async Task Invoke(HttpContext context) {
            var stopwatch = new Stopwatch();
          await   _next(context);
             stopwatch.Stop();
            _logger.LogInformation($"Request {context.Request.Path} took {stopwatch.ElapsedMilliseconds} to execute");
        
        }
    }
}
