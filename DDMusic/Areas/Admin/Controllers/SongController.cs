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
using Microsoft.AspNetCore.Authorization;

namespace DDMusic.Areas.Admin.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class SongController : Controller
    {
        private readonly DPContext _context;

        public SongController(DPContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var AllSong = _context.Song.Where(m => m.Accept == true);
            return View(AllSong);
        }
        //[HttpGet]
        //public JsonResult GetAlbumOfSinger(List<int> idSinger)
        //{
        //    var List = new SelectListItem();

        //    //List<SingerModel> List = new List<SingerModel>();
        //    //var List = _context.Album.Where(m => m.IdSinger == idSinger).Select(a => new SelectListItem()
        //    //{
        //    //    Value = a.Id.ToString(),
        //    //    Text = a.Name
        //    //}).ToList();
        //    return Json(List);
        //}
        public IActionResult CreateSong()
        {
            ViewData["ListGenre"] = new SelectList(SongModel.GetAllGerne());
            ViewData["IdSinger"] = new SelectList(_context.Singer, "Id", "Name");
            return View();
        }
        public async Task<IActionResult> AcceptSong()
        {
            var Song = _context.Song;
            var AllSong = await Song.ToListAsync();
            var SongNotAccept = AllSong.Where(m => m.Accept == false);
            return View(SongNotAccept);
        }
        public async Task<IActionResult> Accept(int id)
        {
            var SongModel = await _context.Song.FindAsync(id);
            SongModel.Accept = true;
            _context.Update(SongModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AcceptSong));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,ReleaseDate,URLImg,URLMusic,Genre,Lyric,IdAlbum")] SongModel song, IFormFile ful, IFormFile fulMusic, List<int> IdSinger)
        {
            if (ModelState.IsValid)
            {
                //Admin tạo bài hát thì luôn được cho phép hiển thị lên frontend
                song.Accept = true;
                song.CountLike = 0;
                if (song.IdAlbum == 0)
                {
                    song.IdAlbum = null;
                }
                //Khởi tạo số view cho bài hát mới là 0
                song.CountView = 0;
                song.NameUnsigned = RemoveUnicode(song.Name);
                _context.Add(song);
                await _context.SaveChangesAsync();
                if (IdSinger != null)
                {
                    foreach (var item in IdSinger)
                    {
                        var singer = await _context.Singer.FindAsync(item);
                        if (item != IdSinger.LastOrDefault())
                        {
                            song.NameSinger += singer.Name + ", ";
                        }
                        else
                        {
                            song.NameSinger += singer.Name;
                        }
                    }
                }
                song.NameUnsignedSinger = RemoveUnicode(song.NameSinger.Trim());
                _context.Update(song);
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
                        await fulMusic.CopyToAsync(stream);
                    }
                    song.URLMusic = NameMusic + "." + fulMusic.FileName.Split(".")[fulMusic.FileName.Split(".").Length - 1];
                    _context.Update(song);
                    await _context.SaveChangesAsync();
                }


                if (IdSinger != null)
                {
                    foreach (var item in IdSinger)
                    {
                        SingerOfSong singerOfSong = new SingerOfSong();
                        singerOfSong.IdSinger = item;
                        singerOfSong.IdSong = song.Id;
                        _context.Add(singerOfSong);
                        await _context.SaveChangesAsync();
                    }
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ReleaseDate,URLImg,URLMusic,Genre,Lyric,Accept,CountView,CountLike,IdAlbum")] SongModel song, IFormFile ful, IFormFile fulMusic)
        {
            if (id != song.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {

                    song.NameUnsigned = RemoveUnicode(song.Name);
                    if (song.IdAlbum == 0)
                    {
                        song.IdAlbum = null;
                    }
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
                    }
                    if (fulMusic != null)
                    {
                        //Bỏ dấu
                        string NameMusic = RemoveUnicode(song.Name);
                        //Bỏ khoảng cách
                        NameMusic = NameMusic.Replace(" ", String.Empty);
                        string t = NameMusic + "." + fulMusic.FileName.Split(".")[fulMusic.FileName.Split(".").Length - 1];
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/audio", song.URLMusic);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/audio", t);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await fulMusic.CopyToAsync(stream);
                        }
                        song.URLMusic = t;
                    }

                    _context.Update(song);
                    await _context.SaveChangesAsync();

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
        public async Task<IActionResult> SingerOfSong(int id)
        {
            var song = await _context.Song.FindAsync(id);
            ViewBag.NameSong = song.Name;
            ViewBag.IdSong = id;
            return View("Detail");
        }
        public async Task<IActionResult> GetSingerOfSong(int id)
        {
            ViewData["Singers"] = new SelectList(_context.Singer, "Id", "Name");
            ViewBag.IdSong = id;
            return PartialView("_SingerOfSong", _context.SingerOfSong.Include(m => m.Singer).Where(m => m.IdSong == id));
            // return View( );
        }

        public async Task<IActionResult> AddSinger(List<int> idSingers, int idSong)
        {
            foreach (var item in idSingers)
            {
                var singerOfSong = _context.SingerOfSong.Where(m => m.IdSinger == item && m.IdSong == idSong).FirstOrDefault();
                if (singerOfSong == null)
                {
                    var singer = await _context.Singer.FindAsync(item);
                    if (singer != null)
                    {
                        singerOfSong = new SingerOfSong();
                        singerOfSong.IdSong = idSong;
                        singerOfSong.IdSinger = item;
                        _context.Add(singerOfSong);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            var song = _context.Song.Find(idSong);
            var ListSinger = _context.SingerOfSong.Include(m => m.Singer).Where(m => m.IdSong == idSong).ToList();
            if (song != null)
            {
                song.NameSinger = String.Empty;
                foreach (var item in ListSinger)
                {
                    if (item != ListSinger.LastOrDefault())
                    {
                        song.NameSinger += item.Singer.Name + ", ";
                    }
                    else
                    {
                        song.NameSinger += item.Singer.Name;
                    }
                }
                song.NameUnsignedSinger = RemoveUnicode(song.NameSinger.Trim());
                _context.Update(song);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("SingerOfSong", "Song", new { area = "Admin", id = idSong });
        }
        public async Task<IActionResult> RemoveSinger(int id, int idSong)
        {
            var singerOfSong = await _context.SingerOfSong.FindAsync(id);
            if (singerOfSong != null)
            {
                _context.Remove(singerOfSong);
                await _context.SaveChangesAsync();
            }
            var song = _context.Song.Find(idSong);
            var ListSinger = _context.SingerOfSong.Include(m => m.Singer).Where(m => m.IdSong == idSong).ToList();
            if (song != null)
            {
                song.NameSinger = String.Empty;
                foreach (var item in ListSinger)
                {
                    if (item != ListSinger.LastOrDefault())
                    {
                        song.NameSinger += item.Singer.Name + ", ";
                    }
                    else
                    {
                        song.NameSinger += item.Singer.Name;
                    }
                }
                song.NameUnsignedSinger = RemoveUnicode(song.NameSinger.Trim());
                _context.Update(song);
                await _context.SaveChangesAsync();
            }
            ViewBag.IdSong = idSong;
            ViewData["Singers"] = new SelectList(_context.Singer, "Id", "Name");
            return PartialView("_SingerOfSong", _context.SingerOfSong.Include(m => m.Singer).Where(m => m.IdSong == idSong));
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
