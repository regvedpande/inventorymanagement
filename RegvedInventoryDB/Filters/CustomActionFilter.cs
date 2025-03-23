using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace RegvedInventoryDB.Filters
{
    public class CustomActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            Debug.WriteLine("Action Starting: " + context.ActionDescriptor.DisplayName);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            Debug.WriteLine("Action Finished: " + context.ActionDescriptor.DisplayName);
        }
    }
}
