using DDMusic.Areas.Admin.Data;
using DDMusic.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PlaylistController : Controller
    {
        private readonly DPContext _context;
        public PlaylistController(DPContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var AllPlaylist = _context.Playlist.ToList();
            return View(AllPlaylist);
        }
        public IActionResult CreatePlaylist()
        {
            ViewData["IdSong"] = new SelectList(_context.Song, "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Playlist model, List<int> idSong, IFormFile ful)
        {
            if (ModelState.IsValid)
            {
                if (idSong.Count != 0)
                {
                    //Add Master để lấy id
                    _context.Add(model);
                    await _context.SaveChangesAsync();
                    if (ful != null)
                    {
                        var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/img/playlist", model.Id + "." + ful.FileName.Split(".")[ful.FileName.Split(".").Length - 1]);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await ful.CopyToAsync(stream);
                        }
                        model.Image = model.Id + "." + ful.FileName.Split(".")[ful.FileName.Split(".").Length - 1];
                        _context.Update(model);
                        await _context.SaveChangesAsync();

                    }

                    //Lưu tất cả bài hát đã chọn vào playlistDetail
                    foreach (var item in idSong)
                    {
                        PlaylistDetail playlistDetail = new PlaylistDetail();
                        playlistDetail.IdPlaylist = model.Id;
                        playlistDetail.IdSong = item;
                        _context.Add(playlistDetail);
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["IdSong"] = new SelectList(_context.Song, "Id", "Name");
            return View("CreatePlaylist");
        }
        public IActionResult EditPlaylist(int id)
        {
            //Tìm playlist 
            var playlist = _context.Playlist.Find(id);
            return View(playlist);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Image")] Playlist model, IFormFile ful)
        {
            if (id != 0)
            {

                return View("EditPlaylist", model);

            }
            if (ModelState.IsValid)
            {
                if (ful != null)
                {
                    string t = model.Id + "." + ful.FileName.Split(".")[ful.FileName.Split(".").Length - 1];
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/playlist", model.Image);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/song", t);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await ful.CopyToAsync(stream);
                    }
                    model.Image = t;
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("EditPlaylist", model);
        }
        public IActionResult DetailPlaylist(int id)
        {
            var playlist = _context.Playlist.Find(id);
            ViewBag.NamePlaylist = playlist.Name;
            ViewBag.IdPlaylist = playlist.Id;
            return View();
        }
        public async Task<IActionResult> Delete(int id)
        {
            var Playlist = await _context.Playlist.FindAsync(id);
            _context.Playlist.Remove(Playlist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> GetSongFromPlaylist(int id)
        {
            var Song = _context.Song;
            var DetailPlaylist = _context.PlaylistDetail.Where(m => m.IdPlaylist == id).ToList();
            List<SongModel> SongFromPlaylist = new List<SongModel>();
            foreach (var item in DetailPlaylist)
            {
                SongModel songModel = new SongModel();
                songModel = _context.Song.Find(item.IdSong);
                var singer = _context.Singer.Find(songModel.IdSinger);
                songModel.Singer = singer;
                SongFromPlaylist.Add(songModel);
            }

            ViewData["IdSong"] = new SelectList(_context.Song, "Id", "Name");
            ViewBag.IdPlaylist = id;
            return PartialView("_GetSongFromPlaylist", SongFromPlaylist);
        }
        [HttpPost]
        public async Task<IActionResult> InsertSongToPlaylist(int IdPlaylist, List<int> IdSong)
        {
            foreach (var item in IdSong)
            {
                var PlaylistDetailExist = _context.PlaylistDetail.Where(m => m.IdSong == item).ToList();
                  if (PlaylistDetailExist.Count == 0)
                {
                    PlaylistDetail playlistDetail = new PlaylistDetail();
                    playlistDetail.IdPlaylist = IdPlaylist;
                    playlistDetail.IdSong = item;
                    _context.Add(playlistDetail);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("DetailPlaylist", "Playlist", new { area = "Admin", id = IdPlaylist });
        }
        [HttpPost]
        public async Task<IActionResult> RemoveSongFromPlaylist(int idSong, int idPlaylist)
        {
            var PlaylistDetail = _context.PlaylistDetail.Where(m => m.IdSong == idSong).ToList();
            _context.PlaylistDetail.Remove(PlaylistDetail[0]);
            await _context.SaveChangesAsync();


            var DetailPlaylist = _context.PlaylistDetail.Where(m => m.IdPlaylist == idPlaylist).ToList();
            List<SongModel> SongFromPlaylist = new List<SongModel>();
            foreach (var item in DetailPlaylist)
            {
                SongModel songModel = new SongModel();
                songModel = _context.Song.Find(item.IdSong);
                var singer = _context.Singer.Find(songModel.IdSinger);
                songModel.Singer = singer;
                SongFromPlaylist.Add(songModel);
            }
            // return PartialView("_GetSongFromPlaylist", SongFromPlaylist);
            return RedirectToAction("DetailPlaylist", "Playlist", new { area = "Admin", id = idPlaylist });
        }
    }
}
