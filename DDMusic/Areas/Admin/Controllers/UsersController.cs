using DDMusic.Areas.Admin.Data;
using DDMusic.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        private readonly DPContext _context;
        public UsersController(DPContext context)
        {
            _context = context;
        }
        public IActionResult Users()
        {
            return View();
        }
        public IActionResult CreateUsers()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Birthday,UserName,Password,Name,URLImg,Address,PhoneNumber,Email,Gender")] UserModel userModel)
        {

            if (ModelState.IsValid)
            {
                _context.Add(userModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userModel);
        }
        public IActionResult EditUsers()
        {
            return View();
        }
    }
}
