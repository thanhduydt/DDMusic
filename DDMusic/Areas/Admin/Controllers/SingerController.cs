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
using Microsoft.AspNetCore.Authorization;

namespace DDMusic.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
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
                singerModel.NameUnsigned= RemoveUnicode(singerModel.Name);
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
                    singerModel.NameUnsigned = RemoveUnicode(singerModel.Name);
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
        public static string RemoveUnicode(string text)
        {
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
                                            "đ",
                                            "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
                                            "í","ì","ỉ","ĩ","ị",
                                            "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
                                            "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
                                            "ý","ỳ","ỷ","ỹ","ỵ",};
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
                                            "d",
                                            "e","e","e","e","e","e","e","e","e","e","e",
                                            "i","i","i","i","i",
                                            "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
                                            "u","u","u","u","u","u","u","u","u","u","u",
                                            "y","y","y","y","y",};
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return text;
        }

    }
}
