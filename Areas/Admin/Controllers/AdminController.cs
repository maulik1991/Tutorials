using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Tutorials.Admin.Controllers
{
    public class AdminController : BaseAdminController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}