using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace API.Filters
{
    public class LogSensitiveActionAttribute:ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            Debug.WriteLine("sensitive action executed");
        }
    }
}
