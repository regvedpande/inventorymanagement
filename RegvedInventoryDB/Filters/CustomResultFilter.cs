using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace RegvedInventoryDB.Filters
{
    public class CustomResultFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            Debug.WriteLine("Result is about to execute.");
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            Debug.WriteLine("Result executed.");
        }
    }
}
