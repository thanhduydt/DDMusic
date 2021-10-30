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
    }
}
