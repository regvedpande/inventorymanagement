using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace RegvedInventoryDB.Filters
{
    public class CustomActionFilter : IActionFilter
    {
        private readonly ILogger<CustomActionFilter> _logger;

        public CustomActionFilter(ILogger<CustomActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogDebug("Executing action {Controller}.{Action}",
                context.RouteData.Values["controller"],
                context.RouteData.Values["action"]);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogDebug("Completed action {Controller}.{Action}",
                context.RouteData.Values["controller"],
                context.RouteData.Values["action"]);
        }
    }
}
