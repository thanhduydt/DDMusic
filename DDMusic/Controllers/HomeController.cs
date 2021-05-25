using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DDMusic.Models;

namespace DDMusic.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Route("the-loai")]
        public IActionResult Genre()
        {
            return View();
        }
        [Route("bang-xep-hang")]
        public IActionResult TopSong()
        {
            return View();
        }

        public IActionResult SongDetail()
        {
            return View();
        }
        [Route("dang-nhap")]
        public IActionResult Login()
        {
            return View();
        }
        [Route("dang-ky")]
        public IActionResult Register()
        {
            return View();
        }
        [Route("ca-si")]
        public IActionResult Singer()
        {
            return View();
        }
        [Route("thong-tin-tai-khoan")]
        public IActionResult PersonalPage()
        {
            return View();
        }
        public IActionResult UploadSong()
        {
            return View();
        }
        [Route("album")]
        public IActionResult Album()
        {
            return View();
        }
        [Route("playlist")]
        public IActionResult Playlist()
        {
            return View();
        }
        [Route("lien-he")]
        public IActionResult Contact()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
