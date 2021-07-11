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
        [Route("the-loai/{routingDetail}")]
        public async Task<IActionResult> GenreAsync(string routingDetail)
        {
            string Genre = null;
            switch (routingDetail)
            {
                case "edm":
                    Genre = "EDM";
                    break;
                case "acoustic":
                    Genre = "Acoustic";
                    break;
                case "pop":
                    Genre = "Pop";
                    break;
                case "ballad":
                    Genre = "Ballad";
                    break;
            }
            var SingerOfSong = _context.Song.Include(s => s.Singer);
            var AllSong = await SingerOfSong.ToListAsync();
            var Song = AllSong.Where(m => m.Genre == Genre && m.Accept == true).OrderByDescending(m => m.Id);
            var NewSong = Song.Take(12);
            var SongOfGenre = Song.Skip(12);
            if (NewSong.Count() > 0)
            {
                ViewBag.NewSong = NewSong;
            }
            if (SongOfGenre.Count() > 0)
            {
                ViewBag.SongOfGenre = SongOfGenre;
            }
            ViewBag.Title = Genre;

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
            foreach (var item in TopSongOnWeekDetail)
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

            var AllTopSongOnMonth = await _context.TopSongOnMonth.ToListAsync();
            var TopSongOnMonth = AllTopSongOnMonth.OrderByDescending(m => m.TimeRestart).First();
            var AllTopSongOnMonthDetail = await _context.TopSongOnMonthDetail.ToListAsync();
            var TopSongOnMonthDetail = AllTopSongOnMonthDetail.Where(m => m.IdTopSongOnMonth == TopSongOnMonth.Id);
            //Gán song cho vào TopSongOnWeekDetail
            List<TopSongOnMonthDetail> topSongOnMonthDetails = new List<TopSongOnMonthDetail>();
            foreach (var item in TopSongOnMonthDetail)
            {
                TopSongOnMonthDetail TopOnMonth = new TopSongOnMonthDetail();
                TopOnMonth = item;
                var song = await _context.Song.FindAsync(TopOnMonth.IdSong);
                var singer = await _context.Singer.FindAsync(song.IdSinger);
                song.Singer = singer;
                TopOnMonth.Song = song;
                topSongOnMonthDetails.Add(TopOnMonth);
            }
            ViewBag.TopSongOnMonth = topSongOnMonthDetails;
            return View();
        }
        [Route("bai-hat/{id}")]
        public async Task<IActionResult> SongDetail(int id)
        {
            var song = await _context.Song.FindAsync(id);
            var singer = await _context.Singer.FindAsync(song.IdSinger);
            song.Singer = singer;
            var AllSong = await _context.Song.ToListAsync();
            var AllSongOfGenre = AllSong.Where(m => m.Id != song.Id && m.Genre == song.Genre);
            var random = new Random();
            var GetRelatedSongs = AllSongOfGenre.OrderBy(m => random.Next()).Take(4);
            List<SongModel> RelatedSongs = new List<SongModel>();
            foreach (var item in GetRelatedSongs)
            {
                SongModel relatedSong = new SongModel();
                relatedSong = item;
                SingerModel singer1 = await _context.Singer.FindAsync(relatedSong.IdSinger);
                RelatedSongs.Add(relatedSong);
            }
            ViewBag.RelatedSongs = RelatedSongs;
            ViewBag.ListComment = await (from c in _context.Comment
                                         join u in _context.User on c.IdUser equals u.Id
                                         where c.IdSong == id
                                         select new CommentModel {
                                             Id = c.Id,
                                             Content = c.Content,
                                             Time = c.Time,
                                             User = u,
                                         }).OrderByDescending(m => m.Time).ToListAsync();
            return View(song);
        }
        [HttpPost]
        public async Task<IActionResult> AddComment(string txtComment,int idSong)
        {
            //Thông tin User đăng nhập
            string idUser = _userManager.GetUserId(User);
            //Tạo mới item Comment
            CommentModel commentModel = new CommentModel();
            commentModel.IdSong = idSong;
            commentModel.IdUser = idUser;
            commentModel.Content = txtComment;
            commentModel.Time = DateTime.Now;
            _context.Comment.Add(commentModel);
            _context.SaveChanges();
            List<CommentModel> listComment = await (from c in _context.Comment
                                                    join u in _context.User on c.IdUser equals u.Id
                                                    where c.IdSong == idSong
                                                    select new CommentModel
                                                    {
                                                        Id = c.Id,
                                                        Content = c.Content,
                                                        Time = c.Time,
                                                        User = u,
                                                    }).OrderByDescending(m => m.Time).ToListAsync();
            return PartialView("_CommentPartial",listComment);
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
        [Route("ca-si/{routingDetail}")]
        public async Task<IActionResult> Singer(string routingDetail)
        {
            string country = "";
            switch (routingDetail)
            {
                case "viet-nam":
                    country = "Việt Nam"; break;
                case "au-my":
                    country = "Âu Mỹ"; break;
                case "han-quoc":
                    country = "Hàn Quốc"; break;
            }
            var AllSinger = await _context.Singer.ToListAsync();
            var SingerOfCountry = AllSinger.Where(m => m.Country == country);
            if (SingerOfCountry.Count() > 0)
            {
                ViewBag.Singer = SingerOfCountry;
            }
            ViewBag.Title = country;
            return View();
        }
        [Route("thong-tin-ca-si/{id}")]
        public async Task<IActionResult> SingleDetail(int id)
        {
            var Singer = await _context.Singer.FindAsync(id);
            var AllSong = await _context.Song.ToListAsync();
            var SongOfSinger = AllSong.Where(m => m.IdSinger == id);

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
        [HttpGet]
        public JsonResult ListName(string term)
        {
            //List<string> nSong= _context.Song.Where(m => m.Name.Contains(term)).Select(m => m.Name).ToList();
            //var data = nSong;
            var song = (from c in _context.Song
                        where c.Name.Contains(term)
                        select new Search
                        {
                            label = c.Name,
                            category = "Bài Hát",
                        });
            var singer = (from sg in _context.Singer
                          where sg.Name.Contains(term)
                          select new Search
                          {
                              label = sg.Name,
                              category = "Ca sĩ"
                          });
            var data = new List<Search>();
            foreach (var item in song)
            {
                data.Add(item);
            }
            foreach (var item in singer)
            {
                data.Add(item);
            }
            return Json(new
            {
                data = data,
                status = true

            });

        }
        public IActionResult Search()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>Search(string txtSearch)
        {
            //ViewBag.listSong = await _context.Song.Where(m => m.Name.Contains(txtSearch)).ToListAsync();
            //ViewBag.listAlbum=await _context.Album.Where(m=>m.Name.Contains(txtSearch)).ToListAsync();
            return View();
        }
        //[HttpGet]
        //public async Task<IActionResult> Comment(string txtC)
        //{
            
        //    return PartialView("_CommentPartial",);
        //}
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
