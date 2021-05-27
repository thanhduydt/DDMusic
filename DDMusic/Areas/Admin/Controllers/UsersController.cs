using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Users()
        {
            return View();
        }
        public IActionResult CreateUsers()
        {
            return View();
        }
        public IActionResult EditUsers()
        {
            return View();
        }
    }
}
