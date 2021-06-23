using DDMusic.Areas.Admin.Data;
using DDMusic.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Controllers
{
    public class TopSongOnWeekController : Controller
    {
        private readonly DPContext _context;

        public TopSongOnWeekController(DPContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            Renew();
            return View();
        }
        public async Task<bool> Renew()
        {
            bool result = false;
            var AllSong = await _context.Song.ToListAsync();
            var Song = AllSong.OrderByDescending(m => m.CountView).Take(10);
            foreach(var item in Song.ToList())
            {
                //Tao top 1 -> 10
                int i = 1;
                TopSongOnWeek topSongOnWeek = new TopSongOnWeek();
                topSongOnWeek.Top = i;
                topSongOnWeek.SongId = item.Id;
                topSongOnWeek.TimeRestart = DateTime.Now.Date;
                _context.Add(topSongOnWeek);
                await _context.SaveChangesAsync();
                //Tang top
                i++;
                result = true;
            }
            
            return result;
        }
    }
}
