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
    [Area("Admin")]
    public class TopSongOnWeekController : Controller
    {
        private readonly DPContext _context;

        public TopSongOnWeekController(DPContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var TopSongOnWeek =await _context.TopSongOnWeek.ToListAsync();
            var TopSongOnWeekSinger = TopSongOnWeek.OrderByDescending(m => m.TimeRestart).First();
            var result = DateTime.Compare(DateTime.Now.Date, TopSongOnWeekSinger.TimeRestart.AddDays(7).Date);
            if (TopSongOnWeekSinger == null || result >= 0)
            {
                var AllSong = await _context.Song.ToListAsync();
                var Song = AllSong.OrderByDescending(m => m.CountView).Take(10);

                //Tao top 1 -> 10
                int i = 1;
                TopSongOnWeek topSongOnWeek = new TopSongOnWeek();
                topSongOnWeek.TimeRestart = DateTime.Now.Date;
                _context.Add(topSongOnWeek);
                await _context.SaveChangesAsync();

                foreach (var item in Song.ToList())
                {
                    TopSongOnWeekDetail topSongOnWeekDetail1 = new TopSongOnWeekDetail();
                    topSongOnWeekDetail1.IdTopSongOnWeek = topSongOnWeek.Id;
                    topSongOnWeekDetail1.IdSong = item.Id;
                    topSongOnWeekDetail1.Top = i;
                    _context.Add(topSongOnWeekDetail1);
                    await _context.SaveChangesAsync();
                    //Tang top
                    i++;
                    //result = true;
                }
                var topSongOnWeekDetails = await _context.TopSongOnWeekDetail.ToListAsync();
                var topSongOnWeekDetail2 = topSongOnWeekDetails.Where(m => m.IdTopSongOnWeek == topSongOnWeek.Id);
                List<TopSongOnWeekDetail> topSongWeek2 = new List<TopSongOnWeekDetail>();
                //Add Singer vào Song và Add Song vào topSongOnWeekDetail
                foreach (var item in topSongOnWeekDetail2)
                {
                    TopSongOnWeekDetail topSong = new TopSongOnWeekDetail();
                    var song = await _context.Song.FindAsync(item.IdSong);
                    var singer = await _context.Singer.FindAsync(song.IdSinger);
                    song.Singer = singer;
                    topSong = item;
                    topSong.Song = song;
                    topSongWeek2.Add(topSong);
                }
                ViewBag.Date = TopSongOnWeekSinger.TimeRestart;
                return View(topSongWeek2);
            }
            var topSongOnWeekDetailAll = await _context.TopSongOnWeekDetail.ToListAsync();
            var topSongOnWeekDetail = topSongOnWeekDetailAll.Where(m => m.IdTopSongOnWeek == TopSongOnWeekSinger.Id);
            List<TopSongOnWeekDetail> topSongWeek = new List<TopSongOnWeekDetail>();
            //Add Singer vào Song và Add Song vào topSongOnWeekDetail
            foreach(var item in topSongOnWeekDetail)
            {
                TopSongOnWeekDetail topSong = new TopSongOnWeekDetail(); 
                var song = await _context.Song.FindAsync(item.IdSong);
                var singer = await _context.Singer.FindAsync(song.IdSinger);
                song.Singer = singer;
                topSong = item;
                topSong.Song = song;
                topSongWeek.Add(topSong);
            }
            ViewBag.Date = TopSongOnWeekSinger.TimeRestart;
            return View(topSongWeek);
            
        }  
    }
}
