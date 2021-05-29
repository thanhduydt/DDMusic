using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Controllers
{
    public class SongController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateSong()
        {
            return View();
        }
        public IActionResult EditSong()
        {
            return View();
        }
    }
}
