using System.Runtime.CompilerServices;

namespace ecommerce_backend.Middlewares
{
    public class RateLimitingMIddleWare(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;
        private static int _counter = 0;
        private static DateTime _lastRequestData = DateTime.Now;

        public async Task Invoke(HttpContext context)
        {
            _counter++;
            if (DateTime.Now.Subtract(_lastRequestData).Seconds > 10)
            {
                _counter = 1;
                _lastRequestData = DateTime.Now;
                await _next(context);
            }
            else
            {
                if (_counter > 5)
                {
                    _lastRequestData = DateTime.Now;
                    await context.Response.WriteAsync("Rate limit exceeded");
                }
                else
                {
                    _lastRequestData = DateTime.Now;
                    await _next(context);
                }

            }
        }
    }
}
