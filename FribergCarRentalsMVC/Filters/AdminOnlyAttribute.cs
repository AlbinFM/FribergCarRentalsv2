using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FribergCarRentalsMVC.Filters
{
    public class AdminOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var http = context.HttpContext;
            var roles = http.Session.GetString("Roles") ?? "";

            var isAdmin = roles.Split(',')
                .Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase));

            if (!isAdmin)
            {
                context.Result = new RedirectToActionResult("AdminLogin", "AdminAuth", null);
            }
        }
    }
}