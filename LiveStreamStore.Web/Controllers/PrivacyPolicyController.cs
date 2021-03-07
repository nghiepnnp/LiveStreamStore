using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LiveStreamStore.Web.Controllers
{
    public class PrivacyPolicyController : Controller
    {
        // Chinh Sách Quyền Riêng Tư
        public IActionResult Index()
        {
            return View();
        }
    }
}
