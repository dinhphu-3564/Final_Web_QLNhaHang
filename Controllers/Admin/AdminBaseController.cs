using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web_QLNhaHang.Controllers.Admin
{
    public class AdminBaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var employeeId = HttpContext.Session.GetInt32("EmployeeId");
            var role = HttpContext.Session.GetString("Role");

            if (employeeId == null || string.IsNullOrEmpty(role))
            {
                context.Result = RedirectToAction("Login", "AdminAuth");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}

