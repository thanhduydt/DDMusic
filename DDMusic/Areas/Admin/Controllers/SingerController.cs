using DDMusic.Areas.Admin.Data;
using DDMusic.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Controllers
{
    public class SingerController : Controller
    {
        private readonly DPContext _context;
        public SingerController(DPContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateSinger()         
        {
            //ViewBag.ListGerne = Ge
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Image,Name,BirthDay,Description")] SingerModel singerModel, IFormFile ful)
        {
            if (ModelState.IsValid)
            {
                _context.Add(singerModel);
                await _context.SaveChangesAsync();
                var path = Path.Combine(
                   Directory.GetCurrentDirectory(), "wwwroot/img/singer", singerModel.Id + "." + ful.FileName.Split(".")[ful.FileName.Split(".").Length - 1]);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await ful.CopyToAsync(stream);
                }
                singerModel.Image = singerModel.Id + "." + ful.FileName.Split(".")[ful.FileName.Split(".").Length - 1];
                _context.Update(singerModel);
                return RedirectToAction(nameof(Index));
            }
            return View(singerModel);
        }
        public IActionResult EditSinger()
        {
            return View();
        }
    }
}
