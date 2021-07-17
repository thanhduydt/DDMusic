using DDMusic.Areas.Admin.Data;
using DDMusic.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AlbumController : Controller
    {
        private readonly DPContext _context;

        public AlbumController(DPContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Album.Include(s => s.Singer));
        }
        public IActionResult CreateAlbum()
        {
            ViewData["IdSinger"] = new SelectList(_context.Singer, "Id", "Name");
            return View();
        }
        public async Task<IActionResult> EditAlbum(int id)
        {
            ViewData["IdSinger"] = new SelectList(_context.Singer, "Id", "Name");
            var album = await _context.Album.FindAsync(id);
            return View(album);
        }
        public IActionResult DetailAlbum(int id)
        {
            var album = _context.Album.Find(id);
            ViewBag.NameAlbum = album.Name;
            ViewBag.IdAlbum = id;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetSongFromAlbum(int id)
        {
            var song = await _context.Song.ToListAsync();
            var SongOfAlbum = song.Where(m => m.IdAlbum == id);
            List<SongModel> SongFromAlbum = new List<SongModel>();
            foreach(var item in SongOfAlbum)
            {
                SongModel songModel = new SongModel();
                var singer = _context.Singer.Find(item.IdSinger);
                songModel = item;
                songModel.Singer = singer;
                SongFromAlbum.Add(songModel);
            }
            return PartialView("_GetSongFromAlbum", SongFromAlbum);
        }
        [HttpPost]
        public async Task<IActionResult> RemoveSongFromAlbum(int id)
        {
            int? IdAlbum;
            var song = await _context.Song.FindAsync(id);
            IdAlbum = song.IdAlbum;
            song.IdAlbum = null;
            //var album = await _context.Album.FindAsync(song.IdAlbum);
            _context.SaveChanges();

            var AllSong = await _context.Song.ToListAsync();
            var SongOfAlbum = AllSong.Where(m => m.IdAlbum == IdAlbum);
            List<SongModel> SongFromAlbum = new List<SongModel>();
            foreach (var item in SongOfAlbum)
            {
                SongModel songModel = new SongModel();
                var singer = _context.Singer.Find(item.IdSinger);
                songModel = item;
                songModel.Singer = singer;
                SongFromAlbum.Add(songModel);
            }
            return PartialView("_GetSongFromAlbum", SongFromAlbum);
            //var AllSong = await _context.Song.ToListAsync();
            //var SongOfAlbum = AllSong.Where(m => m.IdAlbum == IdAlbum);
            //List<SongModel> SongFromAlbum = new List<SongModel>();
            //foreach (var item in SongOfAlbum)
            //{
            //    SongModel songModel = new SongModel();
            //    var singer = _context.Singer.Find(item.IdSinger);
            //    songModel = item;
            //    songModel.Singer = singer;
            //    SongFromAlbum.Add(songModel);
            //}
            //return PartialView("_GetSongFromAlbum", SongFromAlbum);
            //return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,IdSinger,Image")] AlbumModel album, IFormFile ful)
        {
            if (ModelState.IsValid)
            {
                album.NameUnsigned = RemoveUnicode(album.Name);
                _context.Add(album);
                await _context.SaveChangesAsync();
                if (ful != null)
                {
                    var path = Path.Combine(
                    Directory.GetCurrentDirectory(), "wwwroot/img/album", album.Id + "." + ful.FileName.Split(".")[ful.FileName.Split(".").Length - 1]);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await ful.CopyToAsync(stream);
                    }
                    album.Image = album.Id + "." + ful.FileName.Split(".")[ful.FileName.Split(".").Length - 1];
                    _context.Update(album);
                    await _context.SaveChangesAsync();

                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdSinger"] = new SelectList(_context.Singer, "Id", "Name");
            ViewBag.Alert = "Tạo mới album không thành công, vui lòng thử lại";
            return View(album);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Image,IdSinger")] AlbumModel album, IFormFile ful)
        {
            if (id != album.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {

                try
                {
                    album.NameUnsigned = RemoveUnicode(album.Name);
                    if (ful != null)
                    {
                        string t = album.Id + "." + ful.FileName.Split(".")[ful.FileName.Split(".").Length - 1];
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/album", album.Image);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/album", t);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await ful.CopyToAsync(stream);
                        }
                        album.Image = t;
                        _context.Update(album);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        _context.Update(album);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumExists(album.Id))
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
            return View(album);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var AlbumModel = await _context.Album.FindAsync(id);
            _context.Album.Remove(AlbumModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool AlbumExists(int id)
        {
            return _context.Album.Any(e => e.Id == id);
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
