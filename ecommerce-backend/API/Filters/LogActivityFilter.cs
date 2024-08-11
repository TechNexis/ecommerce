using Microsoft.AspNetCore.Mvc.Filters;

namespace ecommerce_backend.Filters
{
    public class LogActivityFilter (ILogger<LogActivityFilter>logger) : IActionFilter
    {
        private readonly ILogger _logger=logger;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var actionName = context.ActionDescriptor.DisplayName;
            var arguments = context.ActionArguments;

            _logger.LogInformation($"Executing action: {actionName}");

            foreach (var argument in arguments)
            {
                _logger.LogInformation($"Argument: {argument.Key} = {argument.Value}");
            }
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //how to close the gate 
          
        }

       
    }
}
