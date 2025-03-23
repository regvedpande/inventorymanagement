using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RegvedInventoryDB.Filters
{
    public class CustomAuthorizationFilter : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            
            bool isAuthorized = true;
            if (!isAuthorized)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
        }
    }
}
