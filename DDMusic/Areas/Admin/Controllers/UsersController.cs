using DDMusic.Areas.Admin.Data;
using DDMusic.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly DPContext _context;
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UsersController(DPContext context, UserManager<UserModel> userManager,RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Users()
        {
            ViewBag.ListUser = null;
            //     ViewBag.ListUser = await _context.User.ToListAsync();
            ViewBag.ListUser =await _userManager.Users.ToListAsync();
            return View();
        }
        public IActionResult CreateUsers(UserModel userModel)
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUsers([Bind("Name,Birthday,UserName,PasswordHash,Name,URLImg,Address,PhoneNumber,Email,Gender")] UserModel userModel,IFormFile ful)
        {
            
            if (ModelState.IsValid)
            {
                var checkEmail = _context.Users.FirstOrDefault(s => s.Email == userModel.Email);
                var checkUserName = _context.Users.FirstOrDefault(s => s.UserName == userModel.UserName);
                if (checkEmail != null && checkUserName != null)
                {
                    ViewBag.eEmail = "Email " + userModel.Email + " đã tồn tại.";
                    ViewBag.eUserName="Tên đăng nhập " + userModel.UserName + " đã tồn tại.";
                }
                else if (checkEmail != null)
                {
                    ViewBag.eEmail = "Email " + userModel.Email + " đã tồn tại.";
                }
                else if (checkUserName != null)
                {
                    ViewBag.eUserName = "Tên đăng nhập " + userModel.UserName + " đã tồn tại.";
                }
                else
                {
                    userModel.EmailConfirmed = true;
                    userModel.URLImg = "noimage.jpg";
                    await _userManager.CreateAsync(userModel,userModel.PasswordHash);
                    //_context.Add(userModel);
                    //await _context.SaveChangesAsync();
                    if(ful!=null){
                        var path = Path.Combine(
         Directory.GetCurrentDirectory(), "wwwroot/img/user-img", userModel.Id + "." + ful.FileName.Split(".")[ful.FileName.Split(".").Length - 1]);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await ful.CopyToAsync(stream);
                        }
                        userModel.URLImg = userModel.Id + "." + ful.FileName.Split(".")[ful.FileName.Split(".").Length - 1];
                        //_context.Update(userModel);
                        //await _context.SaveChangesAsync();                   
                    }
                    await _userManager.UpdateAsync(userModel);
                    return RedirectToAction(nameof(Users));
                }
                //return RedirectToAction(nameof(CreateUsers));
            }
            return View(userModel);
        }
        public async Task<IActionResult> EditUsers(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            EditUserModel editUserModel = new EditUserModel();
            // var user = await _context.Users.FindAsync(id);
            var user = await _userManager.FindByIdAsync(id);
            editUserModel.Id = user.Id;
            editUserModel.Name = user.Name;
            editUserModel.UserName = user.UserName;
            editUserModel.PhoneNumber = user.PhoneNumber;
            editUserModel.Email = user.Email;
            editUserModel.Gender = user.Gender;
            editUserModel.Birthday = user.Birthday;
            editUserModel.URLImg = user.URLImg;
            editUserModel.Address = user.Address;
            if (editUserModel == null)
            {
                return NotFound();
            }
            //var userClaims = await _userManager.GetClaimsAsync(user);
            //var userRoles = await _userManager.GetRolesAsync(user);
            
            return View(editUserModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>EditUsers(string id,[Bind("Id,Name,Birthday,UserName,URLImg,Address,PhoneNumber,Email,Gender")] EditUserModel editUserModel, IFormFile ful)
        {
            if(id!=editUserModel.Id)
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                try
                {
                    var userModel = await _userManager.FindByIdAsync(editUserModel.Id);
                    userModel.Name = editUserModel.Name;
                    userModel.UserName = editUserModel.UserName;
                    userModel.PhoneNumber = editUserModel.PhoneNumber;
                    userModel.Email = editUserModel.Email;
                    userModel.Gender = editUserModel.Gender;
                    userModel.Birthday = editUserModel.Birthday;
                    userModel.Address = editUserModel.Address;
                    userModel.URLImg = editUserModel.URLImg;
                    if (ful != null)
                    {
                        editUserModel.URLImg = "noimage.jpg";
                        string t = editUserModel.Id + "." + ful.FileName.Split(".")[ful.FileName.Split(".").Length - 1];
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/user-img",editUserModel.URLImg);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/user-img", t);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await ful.CopyToAsync(stream);
                        }
                        userModel.URLImg = t;               
                    }
                    await _userManager.UpdateAsync(userModel);
                }
                catch (DbUpdateConcurrencyException)
                {
                    //if (!UserExists(userModel.Id))
                    //{
                    //    return NotFound();
                    //}
                    //else
                    //{
                    //    throw;
                    //}
                }  
                return RedirectToAction(nameof(Users));
            }
            return View(editUserModel);
        }
        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);
            //var user = await _context.Users
            //    .FirstOrDefaultAsync(m => m.Id == id);
            //  _context.Users.Remove(user);
            //await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Users));
        }

    }
}
