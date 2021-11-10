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
    public class CommentController : Controller
    {
        private readonly DPContext _context;
        public CommentController(DPContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            //CommentModel c = new CommentModel();
            //c.IdUser = "2";
            //c.IdSong = 1;
            //c.Description = "rtyubbcs"+ DateTime.Now.ToString("dd/MM/yyyy");
            //c.Time = DateTime.Now;
            //_context.Comment.Add(c);
            //_context.SaveChanges();

            //List<CommentModel> cg = await _context.Comment.OrderByDescending(m => m.Time).ToListAsync();
            //List<CommentModel> Comment =await( from c in _context.Comment
            //              join u in _context.User on c.IdUser equals u.Id
            //              join s in _context.Song on c.IdSong equals s.Id
            //              select new CommentModel
            //              {
            //                  Id=c.Id,
            //                  User = u,
            //                  Song = s,
            //                  Description=c.Description,
            //                  Time=c.Time,
            //        }).OrderByDescending(m => m.Time).ToListAsync();
            return View(await (from c in _context.Comment
                               join u in _context.User on c.IdUser equals u.Id
                               join s in _context.Song on c.IdSong equals s.Id
                               select new CommentModel
                               {
                                   Id = c.Id,
                                   User = u,
                                   Song = s,
                                   Content = c.Content,
                                   Time = c.Time,
                               }).OrderByDescending(m => m.Time).ToListAsync());
        }
        public async Task<IActionResult>Delete(int id)
        {
            var comment = await _context.Comment.FindAsync(id);
             _context.Comment.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
