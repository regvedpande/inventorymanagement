using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace RegvedInventoryDB.Filters
{
    public class CustomResultFilter : IResultFilter
    {
        private readonly ILogger<CustomResultFilter> _logger;

        public CustomResultFilter(ILogger<CustomResultFilter> logger)
        {
            _logger = logger;
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            _logger.LogDebug("Result executing for {Controller}.{Action}",
                context.RouteData.Values["controller"],
                context.RouteData.Values["action"]);
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            _logger.LogDebug("Result executed for {Controller}.{Action}",
                context.RouteData.Values["controller"],
                context.RouteData.Values["action"]);
        }
    }
}
