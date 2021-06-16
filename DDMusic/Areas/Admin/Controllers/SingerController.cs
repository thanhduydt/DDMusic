using DDMusic.Areas.Admin.Data;
using DDMusic.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DDMusic.Areas.Admin.Controllers
{
    public class SingerController : Controller
    {
        private readonly DPContext _context;
        public SingerController(DPContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Singer.ToListAsync());
        }
        public IActionResult CreateSinger()
        {
            ViewData["ListCountry"] = new SelectList(SingerModel.GetCountry());
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Image,Name,BirthDay,Description,Country")] SingerModel singerModel, IFormFile ful)
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
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(singerModel);
        }
        public async Task<IActionResult> EditSinger(int id)
        {
            var Singer = await _context.Singer.FindAsync(id);
            ViewData["ListCountry"] = new SelectList(SingerModel.GetCountry());
            return View(Singer);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Image,Name,BirthDay,Description,Country")] SingerModel singerModel, IFormFile ful)
        {
            if (id != singerModel.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (ful != null)
                    {
                        string t = singerModel.Id + "." + ful.FileName.Split(".")[ful.FileName.Split(".").Length - 1];
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/singer", singerModel.Image);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/singer", t);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await ful.CopyToAsync(stream);
                        }
                        singerModel.Image = t;
                        _context.Update(singerModel);
                        await _context.SaveChangesAsync();

                    }
                    else
                    {
                        _context.Update(singerModel);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SingerExists(singerModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(singerModel);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var SingerModel = await _context.Singer.FindAsync(id);
            _context.Singer.Remove(SingerModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
            private bool SingerExists(int id)
        {
            return _context.Singer.Any(e => e.Id == id);
        }
    }
}
