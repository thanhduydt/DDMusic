using DDMusic.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using DDMusic.Areas.Admin.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DDMusic.Areas.Admin.Controllers
{
    public class SongController : Controller
    {
        private readonly DPContext _context;

        public SongController(DPContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var Song = _context.Song.Include(s => s.Singers);
            return View(await Song.ToListAsync());
        }
        public IActionResult CreateSong()
        {
            ViewData["ListGenre"] = new SelectList(SongModel.GetAllGerne());
            ViewData["IdSinger"] = new SelectList(_context.Singer, "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,IdSinger,ReleaseDate,URLImg,URLMusic,Genre,Lyric")] SongModel song, IFormFile ful, IFormFile fulMusic)
        {
            if (ModelState.IsValid)
            {
                //Admin tạo bài hát thì luôn được cho phép hiển thị lên frontend
                song.Accept = true;
                //Khởi tạo số view cho bài hát mới là 0
                song.CountView = 0;
                _context.Add(song);
                await _context.SaveChangesAsync();
                if (ful != null)
                {
                    var path = Path.Combine(
                    Directory.GetCurrentDirectory(), "wwwroot/img/song", song.Id + "." + ful.FileName.Split(".")[ful.FileName.Split(".").Length - 1]);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await ful.CopyToAsync(stream);
                    }
                    song.URLImg = song.Id + "." + ful.FileName.Split(".")[ful.FileName.Split(".").Length - 1];
                    _context.Update(song);
                    await _context.SaveChangesAsync();

                }
                if (fulMusic != null)
                {
                    //Bỏ dấu
                    string NameMusic = RemoveUnicode(song.Name);
                    //Bỏ khoảng cách
                    NameMusic = NameMusic.Replace(" ", String.Empty);
                    var path = Path.Combine(
                    Directory.GetCurrentDirectory(), "wwwroot/audio", NameMusic + "." + fulMusic.FileName.Split(".")[fulMusic.FileName.Split(".").Length - 1]);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await ful.CopyToAsync(stream);
                    }
                    song.URLMusic = NameMusic + "." + fulMusic.FileName.Split(".")[fulMusic.FileName.Split(".").Length - 1];
                    _context.Update(song);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdSinger"] = new SelectList(_context.Singer, "Id", "Name");
            ViewData["ListGenre"] = new SelectList(SongModel.GetAllGerne());
            ViewBag.Alert = "Tạo mới bài hát không thành công, vui lòng thử lại";
            return View(song);
        }
        public async Task<IActionResult> EditSong(int id)
        {
            ViewData["IdSinger"] = new SelectList(_context.Singer, "Id", "Name");
            ViewData["ListGenre"] = new SelectList(SongModel.GetAllGerne());
            var song = await _context.Song.FindAsync(id);
            return View(song);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,IdSinger,ReleaseDate,URLImg,URLMusic,Genre,Lyric,Accept,CountView")] SongModel song, IFormFile ful)
        {
            if (id != song.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (ful != null)
                    {
                        string t = song.Id + "." + ful.FileName.Split(".")[ful.FileName.Split(".").Length - 1];
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/song", song.URLImg);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/song", t);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await ful.CopyToAsync(stream);
                        }
                        song.URLImg = t;
                        _context.Update(song);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        _context.Update(song);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SongExists(song.Id))
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
            return View(song);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var SongModel = await _context.Song.FindAsync(id);
            _context.Song.Remove(SongModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool SongExists(int id)
        {
            return _context.Song.Any(e => e.Id == id);
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
