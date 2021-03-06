using DDMusic.Areas.Admin.Data;
using DDMusic.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
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
   [Authorize(Roles="Admin")]
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
                //Kiểm tra Emal có tồn tại
                var checkEmail = _context.Users.FirstOrDefault(s => s.Email == userModel.Email);
                //Kiểm tra User có tồn tại 
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
                    //Không cần xác nhận qua Email
                    userModel.EmailConfirmed = true;
                    //Tạo mới User
                    await _userManager.CreateAsync(userModel,userModel.PasswordHash);
                    if(ful!=null){
                        //Thêm hình
                        var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/img/user-img", userModel.Id + "." + ful.FileName.Split(".")[ful.FileName.Split(".").Length - 1]);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await ful.CopyToAsync(stream);
                        }
                        userModel.URLImg = userModel.Id + "." + ful.FileName.Split(".")[ful.FileName.Split(".").Length - 1];                 
                    }
                    //Cập nhật thay đổi User
                    await _userManager.UpdateAsync(userModel);
                    return RedirectToAction(nameof(Users));
                }
            }
            return View(userModel);
        }
        public async Task<IActionResult> EditUsers(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Lấy thông tin User
            var user = await _userManager.FindByIdAsync(id);
            //Gán thông tin User từ csdl vào Model
            EditUserModel editUserModel = new EditUserModel();
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
            return View(editUserModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>EditUsers([Bind("Id,Name,Birthday,UserName,URLImg,Address,PhoneNumber,Email,Gender")] EditUserModel editUserModel, IFormFile ful)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    UserModel userModel = await _userManager.FindByIdAsync(editUserModel.Id);
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
                        //Cập nhật Hình ảnh
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
                    //Cập nhật thông tin User vào csdl
                    var userEdit=   await _userManager.UpdateAsync(userModel);
                    //Kiểm tra cập nhật thông tin thành công không.
                    if(userEdit.Errors.Count()!=0)
                    {
                        //Xảy ra lỗi
                        foreach(var e in userEdit.Errors)
                             {
                            if(e.Code== "DuplicateEmail")
                            {
                                ViewBag.eEmail = "Email "+userModel.Email+" đã tồn tại";
                            }
                            if(e.Code== "DuplicateUserName")
                            {
                                ViewBag.eUserName = "UserName "+userModel.UserName+" đã tồn tại";
                            }
                        }
                        return View(editUserModel);
                    }

                }
                catch (DbUpdateConcurrencyException)
                {

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
            //Xóa playlist của user đã tạo
            foreach(var item in _context.Playlist)
            {
                if (item.IdUser == id)
                {
                    _context.Playlist.Remove(item);
                   // await _context.SaveChangesAsync();
                }
            }
            //Xóa bài hát user đã upload
            foreach (var item in _context.Song)
            {
                if (item.IdUser == id)
                {
                    _context.Song.Remove(item);
              //      await _context.SaveChangesAsync();
                }
            }
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Users));
        }
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>ChangePassword(ChangePasswordAModel model)
        {
            //Kiểm tra UserName có tồn tại
            var userN =await _userManager.FindByNameAsync(model.EmailOrUserName);
            //Kiểm tra Email có tồn tại
            var userE = await _userManager.FindByNameAsync(model.EmailOrUserName);
            if (userN!=null)
          {
                //Tạo mã xác thực User
               var t= await _userManager.GeneratePasswordResetTokenAsync(userN);
                await _userManager.ResetPasswordAsync(userN, t, model.Password);
                ViewBag.Success = "Cập nhật mật khẩu thành công";
            }
        else if(userE!=null)
            {
                var t = await _userManager.GeneratePasswordResetTokenAsync(userN);
                await _userManager.ResetPasswordAsync(userE, t, model.Password);
                ViewBag.Success = "Cập nhật mật khẩu thành công";
            }
            else
            {
                ViewBag.eEmailOrUserName = "UserName,Email không hợp lệ";
            }
            return View();
        }
        public async Task<IActionResult> CreateDataUser()
        {
            for(int i=1;i<16;i++)
            {

                UserModel user = new UserModel();
                user.Id = i.ToString();
                user.UserName = "thanhduy" + i;
                user.Email = "ttduy" + i+".@gmail.com";
                user.EmailConfirmed = true;
              await  _userManager.CreateAsync(user,"tduy"+i);
            }
            
            return RedirectToAction(nameof(Users));
        }
    }
}
