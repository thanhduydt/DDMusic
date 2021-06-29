using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DDMusic.Models;
using Microsoft.AspNetCore.Identity;
using DDMusic.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.EntityFrameworkCore;
using DDMusic.Areas.Admin.Data;

namespace DDMusic.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private DPContext _context;
        public HomeController(ILogger<HomeController> logger, UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager,
       DPContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        //    _signInManager = signInManager;
            _context = context;
        }
        
        //public HomeController(DPContext context)
        //{
        //    this._context = context;
        //}
        public IActionResult Index()
        {
            return View();
        }
        [Route("the-loai")]
        public IActionResult Genre()
        {
            return View();
        }
        [Route("bang-xep-hang")]
        public async Task<IActionResult> TopSongAsync()
        {
            var AllTopSongOnWeek = await _context.TopSongOnWeek.ToListAsync();
            var TopSongOnWeek = AllTopSongOnWeek.OrderByDescending(m => m.TimeRestart).First();
            var AllTopSongOnWeekDetail = await _context.TopSongOnWeekDetail.ToListAsync();
            var TopSongOnWeekDetail = AllTopSongOnWeekDetail.Where(m => m.IdTopSongOnWeek == TopSongOnWeek.Id);
            //Gán song cho vào TopSongOnWeekDetail
            List<TopSongOnWeekDetail> topSongOnWeekDetails = new List<TopSongOnWeekDetail>();
            foreach(var item in TopSongOnWeekDetail)
            {
                TopSongOnWeekDetail TopOnWeek = new TopSongOnWeekDetail();
                TopOnWeek = item;
                var song = await _context.Song.FindAsync(TopOnWeek.IdSong);
                var singer = await _context.Singer.FindAsync(song.IdSinger);
                song.Singer = singer;
                TopOnWeek.Song = song;
                topSongOnWeekDetails.Add(TopOnWeek);
            }
            ViewBag.TopSongOnWeek = topSongOnWeekDetails;
            return View();
        }

        public IActionResult SongDetail()
        {
            return View();
        }
        [Route("dang-nhap")]
        public IActionResult Login()
        {
            return View();
        }
        [Route("dang-ky")]
        public IActionResult Register()
        {
            return View();
        }
        [Route("ca-si")]
        public IActionResult Singer()
        {
            return View();
        }
        //[Route("thong-tin-tai-khoan")]
        public IActionResult PersonalPage()
        {
            //Lấy thông tin User đang đăng nhập gán vào Model
            UserModel user = _userManager.GetUserAsync(User).Result;
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
            return View(editUserModel);
        }
        [HttpPost]
        public async Task<IActionResult> PersonalPage([Bind("Id,Name,Birthday,UserName,URLImg,Address,PhoneNumber,Email,Gender")] EditUserModel editUserModel, IFormFile ful)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Kiểm tra UserName có tồn tại
                    if(_userManager.FindByNameAsync(editUserModel.UserName)!=null&&_userManager.GetUserName(User)!=editUserModel.UserName)
                    {
                        ViewBag.eUserName = editUserModel.UserName + " đã tồn tại.";
                        return View(editUserModel);
                    }
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
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/user-img", editUserModel.URLImg);
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
                return RedirectToAction(nameof(Index));
            }
            return View(editUserModel);
        }
        public async Task<IActionResult> UploadSong()
        {
            //Lấy danh sách Singer
            ViewBag.listSinger = await _context.Singer.ToListAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UploadSong([Bind("Name,IdSinger,URLImg,URLMusic,Lyric")] SongModel model, IFormFile ful, IFormFile fulMusic)
        {
            if (ModelState.IsValid)
            {
                //Lấy thông tin User đang đăng nhập
                var user = _userManager.GetUserAsync(User);
                model.IdUser = user.Result.Id;
                //User tạo bài hát thì không được hiển thị lên frontend
                model.Accept = false;
                //Khởi tạo số view cho bài hát mới là 0
                model.CountView = 0;
                _context.Add(model);
                await _context.SaveChangesAsync();
                if (ful != null)
                {
                    var path = Path.Combine(
                    Directory.GetCurrentDirectory(), "wwwroot/img/song", model.Id + "." + ful.FileName.Split(".")[ful.FileName.Split(".").Length - 1]);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await ful.CopyToAsync(stream);
                    }
                    model.URLImg = model.Id + "." + ful.FileName.Split(".")[ful.FileName.Split(".").Length - 1];
                    _context.Update(model);
                    await _context.SaveChangesAsync();

                }
                if (fulMusic != null)
                {
                    //Bỏ dấu
                      string NameMusic = RemoveUnicode(model.Name);
                    //Bỏ khoảng cách
                    NameMusic = NameMusic.Replace(" ", String.Empty);
                    var path = Path.Combine(
                    Directory.GetCurrentDirectory(), "wwwroot/audio", NameMusic + "." + fulMusic.FileName.Split(".")[fulMusic.FileName.Split(".").Length - 1]);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await fulMusic.CopyToAsync(stream);
                    }
                    model.URLMusic = NameMusic + "." + fulMusic.FileName.Split(".")[fulMusic.FileName.Split(".").Length - 1];
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(UploadSong));
            
        }
        [Route("album")]
        public IActionResult Album()
        {
            return View();
        }
        [Route("playlist")]
        public IActionResult Playlist()
        {
            return View();
        }
        [Route("lien-he")]
        public IActionResult Contact()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public static string RemoveUnicode(string text)
        {
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
                                            "đ",
                                            "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
                                            "í","ì","ỉ","ĩ","ị",
                                            "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
                                            "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
                                            "ý","ỳ","ỷ","ỹ","ỵ",};
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
                                            "d",
                                            "e","e","e","e","e","e","e","e","e","e","e",
                                            "i","i","i","i","i",
                                            "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
                                            "u","u","u","u","u","u","u","u","u","u","u",
                                            "y","y","y","y","y",};
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return text;
        }
    }
}
