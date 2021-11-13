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
    public class TopSongOnMonthController : Controller
    {
        private readonly DPContext _context;

        public TopSongOnMonthController(DPContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            #region Logic cũ
            ////Lay danh sach Top Song thang
            //var TopSongOnMonth = await _context.TopSongOnMonth.ToListAsync();
            //int result = 0;
            //if (TopSongOnMonth.Count > 0)
            //{
            //    //Lay 1 Top Song trong Month moi nhat 
            //    var TopSongOnMonthSingle = TopSongOnMonth.OrderByDescending(m => m.TimeRestart).First();
            //    //So sánh Ngày hiện tại với ngày được tạo Top (Top sẽ được cộng thêm 1 tháng)
            //    result = DateTime.Compare(DateTime.Now.Date, TopSongOnMonthSingle.TimeRestart.AddMonths(1).Date);
            //}
            ////Nếu Top rỗng hoặc Ngày hiện tại trễ hơn hoặc bằng với ngày tạo Top ( Top đã cộng thêm 1 tháng) Thì tạo mới 1 top song theo tháng
            //if (TopSongOnMonth.Count == 0 || result >= 0)
            //{
            //    //Lấy ra 10 bài hát có số lượt view cao nhất
            //    var AllSong = await _context.Song.ToListAsync();
            //    var Song = AllSong.Where(m => m.Accept == true).OrderByDescending(m => m.CountView).Take(10);

            //    //Tao top 1 -> 10
            //    int i = 1;
            //    // Tạo Top theo tháng
            //    TopSongOnMonth topSongOnMonth = new TopSongOnMonth();
            //    topSongOnMonth.TimeRestart = DateTime.Now.Date;
            //    _context.Add(topSongOnMonth);
            //    await _context.SaveChangesAsync();
            //    //Tạo chi tiết top tháng
            //    foreach (var item in Song.ToList())
            //    {
            //        TopSongOnMonthDetail topSongOnMonthDetail1 = new TopSongOnMonthDetail();
            //        topSongOnMonthDetail1.IdTopSongOnMonth = topSongOnMonth.Id;
            //        topSongOnMonthDetail1.IdSong = item.Id;
            //        topSongOnMonthDetail1.Top = i;
            //        _context.Add(topSongOnMonthDetail1);
            //        await _context.SaveChangesAsync();
            //        //Tang top
            //        i++;

            //    }
            //    //Lấy danh sách chi tiết top tháng vừa tạo
            //    var topSongOnMonthDetails = await _context.TopSongOnMonthDetail.ToListAsync();
            //    var topSongOnMonthDetail2 = topSongOnMonthDetails.Where(m => m.IdTopSongOnMonth == topSongOnMonth.Id);
            //    //Danh sách chi tiết top tháng sẽ trả lại cho giao diện
            //    List<TopSongOnMonthDetail> topSongMonth2 = new List<TopSongOnMonthDetail>();
            //    //Add Singer vào Song và Add Song vào topSongOnMonthDetail
            //    foreach (var item in topSongOnMonthDetail2)
            //    {
            //        TopSongOnMonthDetail topSong = new TopSongOnMonthDetail();
            //        //Lấy bài hát theo id đã lưu trong chi tiết top tuần 
            //        var song = await _context.Song.FindAsync(item.IdSong);
            //        //Lấy ca sĩ theo id đã lưu trong bài hát
            //        var singer = await _context.Singer.FindAsync(song.IdSinger);
            //        //Gán tất cả vào 1 chi tiết top tuần
            //        song.Singer = singer;
            //        topSong = item;
            //        topSong.Song = song;
            //        //Gắn vào danh sách chi tiết top tuân để trả về giao diện
            //        topSongMonth2.Add(topSong);
            //    }
            //    //Ngày tạo của top tuần
            //    ViewBag.Date = topSongOnMonth.TimeRestart;
            //    // Trả về view
            //    return View(topSongMonth2);
            //}
            //// Lấy tất cả chi tiết top tuần
            //var topSongOnMonthDetailAll = await _context.TopSongOnMonthDetail.ToListAsync();
            //// Lấy Top tháng mới nhất
            //var TopSongOnMonthSingle1 = TopSongOnMonth.OrderByDescending(m => m.TimeRestart).First();
            //// Lấy chi tiết top tháng theo id của top tháng mới nhất
            //var topSongOnMonthDetail = topSongOnMonthDetailAll.Where(m => m.IdTopSongOnMonth == TopSongOnMonthSingle1.Id);
            ////Tạo danh sách chi tiết top tháng để trả về giao diện
            //List<TopSongOnMonthDetail> topSongMonth = new List<TopSongOnMonthDetail>();
            ////Add Singer vào Song và Add Song vào topSongOnMonthDetail
            //foreach (var item in topSongOnMonthDetail)
            //{
            //    TopSongOnMonthDetail topSong = new TopSongOnMonthDetail();
            //    // Tìm bài hát theo id trong chi tiết top tháng
            //    var song = await _context.Song.FindAsync(item.IdSong);
            //    // Tìm ca sĩ theo id tại bài hát trong chi tiết top tháng
            //    var singer = await _context.Singer.FindAsync(song.IdSinger);
            //    // Gán vào 1 chi tiết top tháng 
            //    song.Singer = singer;
            //    topSong = item;
            //    topSong.Song = song;
            //    // Gắn chi tiết top tháng vào danh sách chi tiết top tuần để trả về giao diện
            //    topSongMonth.Add(topSong);
            //}
            //// Lấy ngày tạo của top tháng mới nhất
            //ViewBag.Date = TopSongOnMonthSingle1.TimeRestart;
            ////Trả về view
            //return View(topSongMonth);
            #endregion
            
            DateTime firstDayOfMonth = DateTime.Now;
            int intDay = firstDayOfMonth.Day - 1;
            List<TopSongOnMonthDetail> topSongOnMonthDetails = new List<TopSongOnMonthDetail>();
            if(intDay != 2)
            {
                firstDayOfMonth = firstDayOfMonth.AddDays(-intDay);
            }
            var topSongOnMonth = _context.TopSongOnMonth.Where(m => m.TimeRestart == firstDayOfMonth.Date).FirstOrDefault();
            if(topSongOnMonth != null)
            {
                topSongOnMonthDetails = _context.TopSongOnMonthDetail.Include(m => m.Song).Where(m => m.IdTopSongOnMonth == topSongOnMonth.Id).ToList();
            }
            else
            {
                topSongOnMonth.TimeRestart = firstDayOfMonth;
                _context.Add(topSongOnMonth);
                await _context.SaveChangesAsync();

                var viewSongOfMonth = _context.ViewSongOfMonth.Where(m => m.Date == firstDayOfMonth.AddMonths(-1).Date).FirstOrDefault();
                if(viewSongOfMonth == null)
                {
                    viewSongOfMonth = _context.ViewSongOfMonth.Where(m => m.Date == firstDayOfMonth.Date).FirstOrDefault();
                }
                if(viewSongOfMonth != null)
                {
                    List<ViewSongOfMonthDetail> viewSongOfMonthDetails = _context.ViewSongOfMonthDetail.Where(m => m.IdViewSongOfMonth == viewSongOfMonth.Id).OrderByDescending(m => m.CountView).Take(10).ToList();
                    int top = 1;
                    foreach(var item in viewSongOfMonthDetails)
                    {
                        TopSongOnMonthDetail topSongOnMonthDetail = new TopSongOnMonthDetail();
                        topSongOnMonthDetail.Top = top;
                        topSongOnMonthDetail.IdTopSongOnMonth = topSongOnMonth.Id;
                        topSongOnMonthDetail.IdSong = item.IdSong;
                        _context.Add(topSongOnMonthDetail);
                        await _context.SaveChangesAsync();
                        top++;
                    }
                }
                topSongOnMonthDetails = _context.TopSongOnMonthDetail.Include(m => m.Song).Where(m => m.IdTopSongOnMonth == topSongOnMonth.Id).ToList();
            }
            return View(topSongOnMonthDetails);
        }
    }
}
