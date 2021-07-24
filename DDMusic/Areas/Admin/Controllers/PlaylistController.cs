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
            ViewData["IdSong"] = new SelectList(_context.Song, "Id", "Name");
            return View("CreatePlaylist");
        }
    }
}
