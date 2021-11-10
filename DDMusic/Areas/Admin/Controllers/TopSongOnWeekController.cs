using DDMusic.Areas.Admin.Data;
using DDMusic.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
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
            //Lay danh sach Top Song trong tuan
            var TopSongOnWeek =await _context.TopSongOnWeek.ToListAsync();
            int result = 0;
            if (TopSongOnWeek.Count > 0)
            {
                //Lay 1 Top Song trong tuan moi nhat 
                var TopSongOnWeekSingle = TopSongOnWeek.OrderByDescending(m => m.TimeRestart).First();
                //So sánh Ngày hiện tại với ngày được tạo Top (Top sẽ được cộng thêm 7 ngày)
                 result = DateTime.Compare(DateTime.Now.Date, TopSongOnWeekSingle.TimeRestart.AddDays(7).Date);
            } 
            //Nếu Top rỗng hoặc Ngày hiện tại trễ hơn hoặc bằng với ngày tạo Top ( Top đã cộng thêm 7 ngày) Thì tạo mới 1 top song theo tuần
            if (TopSongOnWeek.Count == 0 || result >= 0)
            {
                //Lấy ra 10 bài hát có số lượt view cao nhất
                var AllSong = await _context.Song.ToListAsync();
                var Song = AllSong.Where(m => m.Accept == true).OrderByDescending(m => m.CountView).Take(10);

                //Tao top 1 -> 10
                int i = 1;
                // Tạo Top theo tuần 
                TopSongOnWeek topSongOnWeek = new TopSongOnWeek();
                topSongOnWeek.TimeRestart = DateTime.Now.Date;
                _context.Add(topSongOnWeek);
                await _context.SaveChangesAsync();
                //Tạo chi tiết top tuần
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
                   
                }
                //Lấy danh sách chi tiết top tuần vừa tạo
                var topSongOnWeekDetails = await _context.TopSongOnWeekDetail.ToListAsync();
                var topSongOnWeekDetail2 = topSongOnWeekDetails.Where(m => m.IdTopSongOnWeek == topSongOnWeek.Id);
                //Danh sách chi tiết top tuần sẽ trả lại cho giao diện
                List<TopSongOnWeekDetail> topSongWeek2 = new List<TopSongOnWeekDetail>();
                //Add Singer vào Song và Add Song vào topSongOnWeekDetail
                foreach (var item in topSongOnWeekDetail2)
                {
                    TopSongOnWeekDetail topSong = new TopSongOnWeekDetail();
                    //Lấy bài hát theo id đã lưu trong chi tiết top tuần 
                    var song = await _context.Song.FindAsync(item.IdSong);
                    //Lấy ca sĩ theo id đã lưu trong bài hát
                    var singer = await _context.Singer.FindAsync(song.IdSinger);
                    //Gán tất cả vào 1 chi tiết top tuần
                    song.Singer = singer;
                    topSong = item;
                    topSong.Song = song;
                    //Gắn vào danh sách chi tiết top tuân để trả về giao diện
                    topSongWeek2.Add(topSong);
                }
                //Ngày tạo của top tuần
                ViewBag.Date = topSongOnWeek.TimeRestart;
                // Trả về view
                return View(topSongWeek2);
            }
            // Lấy tất cả chi tiết top tuần
            var topSongOnWeekDetailAll = await _context.TopSongOnWeekDetail.ToListAsync();
            // Lấy Top tuần mới nhất
            var TopSongOnWeekSingle1 = TopSongOnWeek.OrderByDescending(m => m.TimeRestart).First();
            // Lấy chi tiết top tuần theo id của top tuần mới nhất
            var topSongOnWeekDetail = topSongOnWeekDetailAll.Where(m => m.IdTopSongOnWeek == TopSongOnWeekSingle1.Id);
            //Tạo danh sách chi tiết top tuần để trả về giao diện
            List<TopSongOnWeekDetail> topSongWeek = new List<TopSongOnWeekDetail>();
            //Add Singer vào Song và Add Song vào topSongOnWeekDetail
            foreach(var item in topSongOnWeekDetail)
            {
                TopSongOnWeekDetail topSong = new TopSongOnWeekDetail(); 
                // Tìm bài hát theo id trong chi tiết top tuần
                var song = await _context.Song.FindAsync(item.IdSong);
                // Tìm ca sĩ theo id tại bài hát trong chi tiết top tuần
                var singer = await _context.Singer.FindAsync(song.IdSinger);
                // Gán vào 1 chi tiết top tuần 
                song.Singer = singer;
                topSong = item;
                topSong.Song = song;
                // Gắn chi tiết top tuần vào danh sách chi tiết top tuần để trả về giao diện
                topSongWeek.Add(topSong);
            }
            // Lấy ngày tạo của top tuần mới nhất
            ViewBag.Date = TopSongOnWeekSingle1.TimeRestart;
            //Trả về view
            return View(topSongWeek);
            
        }  
    }
}
