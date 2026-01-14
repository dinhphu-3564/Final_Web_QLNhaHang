using Microsoft.AspNetCore.Mvc;

namespace Web_QLNhaHang.Controllers.Admin
{
    public class InfoController : AdminBaseController
    {
        public IActionResult About()
        {
            return View();
        }
        
        public IActionResult Contact()
        {
            return View();
        }
    }
}

