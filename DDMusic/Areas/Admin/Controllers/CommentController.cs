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
    //[Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class CommentController : Controller
    {
        private readonly DPContext _context;
        public CommentController(DPContext context)
        {
            _context = context;
        }
        public List<CommentModel> GetListComment(bool accept)
        {
            List<CommentModel> listComment = new List<CommentModel>();
            listComment = _context.Comment.Where(m => m.Accept == accept).Include(m => m.Song).Include(m => m.User).OrderByDescending(m => m.Time).ToList();
            return listComment;
        }
        public async Task<IActionResult> Index()
        {
            return View(GetListComment(true));
        }
        [HttpPost]
        public async Task<IActionResult>Delete(int id,int page)
        {
            var comment = await _context.Comment.FindAsync(id);
             _context.Comment.Remove(comment);
            await _context.SaveChangesAsync();
            bool accept = true;
            if (page != 0)
            {
                accept = false;
            }
            ViewBag.page = page;
            return PartialView("_CommentListPartial", GetListComment(accept));
        }
        public IActionResult AcceptComment()
        {
            return View(GetListComment(false));
        }
        [HttpPost]
        public async Task<IActionResult> Accept(int id)
        {
            var comment =await _context.Comment.FindAsync(id);
            comment.Accept = true;
            _context.Update(comment); 
            await _context.SaveChangesAsync();
            return PartialView("_CommentListPartial", GetListComment(false));
        }
    }
}
