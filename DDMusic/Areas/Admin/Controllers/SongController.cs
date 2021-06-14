using DDMusic.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Controllers
{
    public class SongController : Controller
    {
        //private readonly DPContext _context;

        //public ProductsController(DPContext context)
        //{
        //    _context = context;
        //}

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateSong()
        {
            return View();
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Name,Image,Price,Description,Status,Catid")] SongModel song, IFormFile ful)
        ////{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(song);
        //        await _context.SaveChangesAsync();
        //        var path = Path.Combine(
        //            Directory.GetCurrentDirectory(), "wwwroot/images/product", product.Id + "." + ful.FileName.Split(".")[ful.FileName.Split(".").Length - 1]);
        //        using (var stream = new FileStream(path, FileMode.Create))
        //        {
        //            await ful.CopyToAsync(stream);
        //        }
        //        product.Image = product.Id + "." + ful.FileName.Split(".")[ful.FileName.Split(".").Length - 1];
        //        _context.Update(product);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["Catid"] = new SelectList(_context.typeProduct, "Id", "Id", product.Catid);
        //    return View(product);
        //}
        public IActionResult EditSong()
        {
            return View();
        }
    }
}
