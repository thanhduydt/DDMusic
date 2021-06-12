using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Controllers
{
    public class SingerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateSinger()
        {
            return View();
        }
        public IActionResult EditSinger()
        {
            return View();
        }
    }
}
