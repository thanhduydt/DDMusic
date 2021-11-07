using DDMusic.Areas.Admin.API.Code;
using DDMusic.Areas.Admin.Data;
using DDMusic.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.API.Controllers
{
    [Area("Admin"), Route("api/song/[action]")]
    [APIJWTAttribute]
    public class SongAPIController : ControllerBase
    {
        private readonly DPContext _context;
        public SongAPIController(DPContext context)
        {
            _context = context;
        }
        [HttpGet, ProducesResponseType(200)]
        public async Task<List<SongModel>> GetTopSong()
        {
            try
            {
                List<SongModel> Songs = _context.Song.Include(s => s.Singer).OrderByDescending(m => m.CountView).Take(10).ToList();
                return Songs == null ? new List<SongModel>() : Songs;
            }
            catch
            {
                return new List<SongModel>();
            }
        }
        [HttpGet, ProducesResponseType(200)]
        public async Task<List<ViewSongOfDayDetail>> GetViewSongOfDay(string date)
        {
            try
            {
                DateTime dateTime = Convert.ToDateTime(date);
                List<ViewSongOfDayDetail> viewSongOfDayDetails = new List<ViewSongOfDayDetail>();
                 var viewSongOfDays = _context.ViewSongOfDay.Where(m => m.Date == dateTime.Date).ToList();
                if(viewSongOfDays.Count != 0 || viewSongOfDays != null)
                {
                    viewSongOfDayDetails = _context.ViewSongOfDayDetail.Where(m => m.IdViewSongOfDay == viewSongOfDays.FirstOrDefault().Id).Include(s => s.Song).OrderByDescending(m => m.CountView).Take(10).ToList();
                }
                return viewSongOfDayDetails == null ? new List<ViewSongOfDayDetail>() : viewSongOfDayDetails;
            }
            catch
            {
                return new List<ViewSongOfDayDetail>();
            }
        }

        //[HttpGet, ProducesResponseType(200)]
        //public async Task<List<ViewSongOfDayDetail>> GetViewSongOfDay(string min_date, string max_date)
        //{
        //    try
        //    {
        //        DateTime min_Date = Convert.ToDateTime(min_date);
        //        DateTime max_Date = Convert.ToDateTime(max_date);
        //        List<ViewSongOfDayDetail> viewSongOfDayDetails = new List<ViewSongOfDayDetail>();
        //        var viewSongOfDays = _context.ViewSongOfDay.Where(m => m.Date >= min_Date.Date && m.Date <= max_Date.Date).ToList();
        //        if (viewSongOfDays.Count != 0 || viewSongOfDays != null)
        //        {
        //            viewSongOfDayDetails = _context.ViewSongOfDayDetail.Where(m => m.IdViewSongOfDay == viewSongOfDays.FirstOrDefault().Id).Include(s => s.Song).ToList();
        //        }
        //        return viewSongOfDayDetails == null ? new List<ViewSongOfDayDetail>() : viewSongOfDayDetails;
        //    }
        //    catch
        //    {
        //        return new List<ViewSongOfDayDetail>();
        //    }
        //}

    }
}
