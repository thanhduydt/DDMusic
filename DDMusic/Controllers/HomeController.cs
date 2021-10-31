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
using System.Text.Json;
using Newtonsoft.Json;
using NAudio.Wave;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            //12 Bài hát mới nhất
            var NewSong = _context.Song.Include(m => m.Singer).Take(12).OrderByDescending(m => m.Id).Where(m => m.Accept == true).ToList();
            ViewBag.NewSong = NewSong;
            //12 Album mới nhất
            var Albums = _context.Album.Include(m => m.Singer).OrderByDescending(m => m.Id).ToList();
            List<AlbumModel> NewAlbum = new List<AlbumModel>();
            int point = 0;
            foreach (var item in Albums)
            {
                //Lấy 12 album mới nhất
                if (point == 12)
                {
                    break;
                }
                var song = _context.Song.Where(m => m.IdAlbum == item.Id);
                if (song.Count() != 0)
                {
                    NewAlbum.Add(item);
                    point++;
                }
            }
            ViewBag.NewAlbum = NewAlbum;
            //12 Playlist mới nhất
            var Playlists = _context.Playlist.OrderByDescending(m => m.Id).ToList();
            List<Playlist> NewPlaylist = new List<Playlist>();
            point = 0;
            foreach (var item in Playlists)
            {
                //Lấy 12 playlist 
                if (point == 12)
                {
                    break;
                }
                var playlistDetail = _context.PlaylistDetail.Where(m => m.IdPlaylist == item.Id);
                if (playlistDetail.Count() != 0)
                {
                    NewPlaylist.Add(item);
                    point++;
                }

            }
            ViewBag.NewPlaylist = NewPlaylist;
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
            var GetRelatedSongs = AllSongOfGenre.OrderBy(m => random.Next()).Take(10);
            List<SongModel> RelatedSongs = new List<SongModel>();
            RelatedSongs.Add(song);
            foreach (var item in GetRelatedSongs)
            {
                SongModel relatedSong = new SongModel();
                relatedSong = item;
                SingerModel singer1 = await _context.Singer.FindAsync(relatedSong.IdSinger);
                RelatedSongs.Add(relatedSong);
            }
            ViewBag.listSong = JsonConvert.SerializeObject(RelatedSongs);
            ViewBag.Title = "Những bài hát liên quan";

            return View(song);
        }
        [HttpPost]
        public IActionResult ReactSong(int IdSong)
        {
            bool result = false;
            //Thông tin User đăng nhập
            string IdUser = _userManager.GetUserId(User);

            var reactSongs = _context.ReactSong.ToList().Where(m => m.IdSong == IdSong && m.IdUser == IdUser);
            var Song = _context.Song.Find(IdSong);
            //Kiểm tra tồn tại
            if (reactSongs.ToList().Count != 0)
            {
                var reactSong = reactSongs.First();
                if (reactSong.React == false)
                {
                    reactSong.React = true;
                    Song.CountLike++;
                    result = true;
                }
                else
                {
                    reactSong.React = false;
                    result = false;
                    Song.CountLike--;

                }

                _context.Update(reactSong);
                _context.Update(Song);

            }
            //Chưa có thì tạo mới
            else
            {
                ReactSong react = new ReactSong();
                react.IdSong = IdSong;
                react.IdUser = IdUser;
                react.React = true;
                result = true;
                Song.CountLike++;
                _context.Add(react);
                _context.Update(Song);
            }
            _context.SaveChanges();
            return Content(result.ToString(), "application/json");
        }
        [HttpGet]
        public IActionResult GetReactSong(int IdSong)
        {
            bool result = false;
            //Thông tin User đăng nhập
            string IdUser = _userManager.GetUserId(User);
            if (IdUser != null)
            {
                var reactSongs = _context.ReactSong.ToList().Where(m => m.IdSong == IdSong && m.IdUser == IdUser);
                if (reactSongs.ToList().Count != 0)
                {
                    if (reactSongs.ToList().First().React == true)
                        result = true;
                }
            }
            return Content(result.ToString(), "application/json");
        }

        [HttpGet]
        public JsonResult GetTimeSongAsync(int idSong)
        {
            string Url = _context.Song.FindAsync(idSong).Result.URLMusic;
            AudioFileReader wf = new AudioFileReader("wwwroot/audio/" + Url);
            TimeSpan span = wf.TotalTime;
            int time = (int)span.TotalMilliseconds;
            return Json(new { timeSong = time });
        }
        [HttpPost]
        public async Task<IActionResult> AddComment(string txtComment, int idSong)
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
            List<CommentModel> listComment = await GetListComment(idSong);
            return PartialView("_CommentPartial", listComment);
        }
        [HttpGet]
        public async Task<IActionResult> LoadComment(int idSong)
        {
            List<CommentModel> listComment = await GetListComment(idSong);
            return PartialView("_CommentPartial", listComment);
        }
        public async Task<List<CommentModel>> GetListComment(int idSong)
        {
            List<CommentModel> listComment = await _context.Comment.Include(m => m.User)
      .Where(m => m.IdSong == idSong).OrderByDescending(m => m.Time).ToListAsync();
            return listComment;
        }
        [HttpPost]
        public async Task<string> AddView(int idSong)
        {
            if (idSong >= 0)
            {
                var song = await _context.Song.FindAsync(idSong);
                song.CountView++;
                _context.Song.Update(song);
                await _context.SaveChangesAsync();

                //Tạo mới hoặc cập nhật thống kê
                await UpdateViewSongOfDay(idSong);
                HttpContext.Session.Remove("idSong");
            }
            return "";
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
        public async Task<IActionResult> SingerDetail(int id)
        {
            var Singer = await _context.Singer.FindAsync(id);
            var AllSong = await _context.Song.ToListAsync();
            var SongOfSinger = AllSong.Where(m => m.IdSinger == id);
            var AllAlbumOfSinger = _context.Album.Where(m => m.IdSinger == id).ToList();
            List<AlbumModel> AlbumOfSinger = new List<AlbumModel>();
            foreach (var item in AllAlbumOfSinger)
            {
                var song = _context.Song.Where(m => m.IdAlbum == item.Id && m.Accept == true);
                if (song.Count() != 0)
                {
                    AlbumOfSinger.Add(item);
                }
            }
            if (AlbumOfSinger.ToList().Count != 0)
                ViewBag.AlbumOfSinger = AlbumOfSinger;
            if (SongOfSinger.ToList().Count != 0)
                ViewBag.SongOfSinger = SongOfSinger;

            return View(Singer);
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
                    UserModel userModel = await _userManager.FindByIdAsync(editUserModel.Id);
                    userModel.Name = editUserModel.Name;
                    userModel.UserName = editUserModel.UserName;
                    userModel.PhoneNumber = editUserModel.PhoneNumber;
                    userModel.Gender = editUserModel.Gender;
                    userModel.Birthday = editUserModel.Birthday;
                    userModel.Address = editUserModel.Address;
                    userModel.URLImg = editUserModel.URLImg;
                    if (ful != null)
                    {
                        //Cập nhật Hình ảnh
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
                    //Cập nhật thông tin User vào csdl
                    var userEdit = await _userManager.UpdateAsync(userModel);
                    //Kiểm tra cập nhật thông tin thành công không.
                    if (userEdit.Errors.Count() != 0)
                    {
                        //Xảy ra lỗi
                        foreach (var e in userEdit.Errors)
                        {
                            if (e.Code == "DuplicateUserName")
                            {
                                ViewBag.eUserName = "Tên đăng nhập " + userModel.UserName + " đã tồn tại";
                            }
                        }
                        return View(editUserModel);
                    }
                    ViewBag.Message = "Cập nhật thông tin thành công.";
                    editUserModel.URLImg = userModel.URLImg;
                    return View(editUserModel);

                }
                catch (DbUpdateConcurrencyException)
                {

                }
            }
            return View(editUserModel);
        }
        [HttpGet]
        public JsonResult ListName(string term)
        {
            term = RemoveUnicode(term);
            var song = (from c in _context.Song
                        where c.NameUnsigned.Contains(term)
                        select new Search
                        {
                            label = c.Name,
                            category = "Bài Hát",
                        });
            var singer = (from sg in _context.Singer
                          where sg.NameUnsigned.Contains(term)
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
        [HttpGet]
        public async Task<IActionResult> Search(string txtSearch)
        {
            txtSearch = RemoveUnicode(txtSearch);
            //   Tìm kiếm bài hát theo tên bài và tên ca sĩ
            ViewBag.listSong = await (from s in _context.Song.Include(m => m.Singer)
                                      where s.NameUnsigned.Contains(txtSearch) || s.Singer.NameUnsigned.Contains(txtSearch)
                                      select s).Take(5).ToListAsync();
            //   Tìm kiếm album theo tên bài và tên ca sĩ
            ViewBag.listAlbum = await (from a in _context.Album.Include(m => m.Singer)
                                       where a.NameUnsigned.Contains(txtSearch) || a.Singer.NameUnsigned.Contains(txtSearch)
                                       select a).Take(10).ToListAsync();
            ViewBag.txtSearch = txtSearch.ToString();
            return View();
        }
        public async Task<IActionResult> AllSong(string txtSearch)
        {
            //   Tìm kiếm bài hát theo tên bài và tên ca sĩ
            List<SongModel> listSong = await (from s in _context.Song.Include(m => m.Singer)
                                              where s.NameUnsigned.Contains(txtSearch) || s.Singer.NameUnsigned.Contains(txtSearch)
                                              select s).ToListAsync();
            return View(listSong);
        }
        public async Task<IActionResult> AllAlbum(string txtSearch)
        {
            //   Tìm kiếm album theo tên album và tên ca sĩ
            List<AlbumModel> listAlbum = await (from a in _context.Album.Include(m => m.Singer)
                                                where a.NameUnsigned.Contains(txtSearch) || a.Singer.NameUnsigned.Contains(txtSearch)
                                                select a).ToListAsync();
            return View(listAlbum);
        }
        public IActionResult UploadSong()
        {
            GetListSingerAndAlbum();
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
                ViewBag.Message = "Upload bài hát thành công.";
            }
            GetListSingerAndAlbum();
            return View();

        }
        public void GetListSingerAndAlbum()
        {
            ViewData["ListGenre"] = new SelectList(SongModel.GetAllGerne());
            ViewData["IdSinger"] = new SelectList(_context.Singer, "Id", "Name");
        }
        [Route("album")]
        public IActionResult Album()
        {
            var AllAlbum = _context.Album.Include(m => m.Singer).ToList();
            List<AlbumModel> Albums = new List<AlbumModel>();
            foreach (var item in AllAlbum)
            {
                var song = _context.Song.Where(m => m.IdAlbum == item.Id);
                if (song.Count() != 0)
                {
                    Albums.Add(item);
                }
            }
            //Lấy 8 album mới nhất
            var NewAlbum = Albums.OrderByDescending(m => m.Id).Take(8).ToList();

            //Lấy tất cả album ngoài 8 album mới nhất
            var Album = Albums.OrderByDescending(m => m.Id).Skip(8).ToList();

            ViewBag.NewAlbum = NewAlbum;
            ViewBag.Album = Album;

            return View();
        }
        [Route("album/{id}")]
        public IActionResult AlbumDetail(int id)
        {
            //Lấy album
            var Album = _context.Album.Find(id);
            //Lấy tất cả bài hát trong album
            var SongOfAlbum = _context.Song.Where(m => m.IdAlbum == id).Include(m => m.Singer).ToList();
            var Song = SongOfAlbum[0];
            ViewBag.listSong = JsonConvert.SerializeObject(SongOfAlbum);
            ViewBag.Title = "Những bài hát thuộc album: " + Album.Name;

            return View("SongDetail", Song);
        }
        [Route("playlist")]
        public IActionResult Playlist()
        {
            ////Lấy 8 Playlist mới nhất
            //var NewPlaylists = _context.Playlist.OrderByDescending(m => m.Id).Take(8).ToList();
            ////Lấy tất cả playlist
            //var Playlists = _context.Playlist.OrderByDescending(m => m.Id).Skip(8).ToList();
            //Lấy tất cả playlist
            var AllPlaylists = _context.Playlist.ToList();
            //Kiểm tra Playlist rỗng 
            List<Playlist> Playlists = new List<Playlist>();
            foreach (var item in AllPlaylists)
            {
                var PlaylistDetail = _context.PlaylistDetail.Where(m => m.IdPlaylist == item.Id);
                if (PlaylistDetail.Count() != 0)
                {
                    Playlists.Add(item);
                }
            }
            //Sắp xếp playlist theo Id giảm dần
            //Lấy 8 playlist mới nhất
            var NewPlaylists = Playlists.OrderByDescending(m => m.Id).Take(8).ToList();
            //Lấy tất cả playlist trừ 8 playlist đã lấy
            var Playlist = Playlists.OrderByDescending(m => m.Id).Skip(8).ToList();

            ViewBag.NewPlaylists = NewPlaylists;
            ViewBag.Playlists = Playlist;

            return View();
        }
        [Route("playlist/{id}")]
        public IActionResult PlaylistDetail(int id)
        {
            //Lấy PlaylistDetail 
            var PlaylistDetail = _context.PlaylistDetail.Include(m => m.Song).Where(m => m.IdPlaylist == id).ToList();
            List<SongModel> ListSong = new List<SongModel>();
            foreach (var item in PlaylistDetail)
            {
                SongModel song = new SongModel();
                song = item.Song;
                song.Singer = _context.Singer.Find(song.IdSinger);
                ListSong.Add(song);
            }
            var Playlist = _context.Playlist.Find(id);
            ViewBag.listSong = JsonConvert.SerializeObject(ListSong);
            ViewBag.Title = "Những bài hát thuộc Playlist: " + Playlist.Name;
            return View("SongDetail", ListSong[0]);
        }
        [Route("playlistUser")]
        public IActionResult PlayListUser()
        {
            List<Playlist> playlists = new List<Playlist>();
            var allPlayList = _context.Playlist.ToList();
            var user = _userManager.GetUserAsync(User);
            foreach (var item in allPlayList)
            {
                if (item.IdUser == user.Result.Id)
                {
                    var playlistDetail = _context.PlaylistDetail.Where(m => m.IdPlaylist == item.Id).ToList();
                    if (playlistDetail.Count > 0)
                    {
                        var img = _context.Song.Find(playlistDetail[0].IdSong);
                        item.Image = img.URLImg;
                    }
                    playlists.Add(item);
                }
            }
            return View(playlists);
        }
        [Route("playlistUser/{idPlayList}")]
        public IActionResult PagePlayListDetail(int idPlayList)
        {
            var allPlayListDetail = _context.PlaylistDetail.Include(m => m.Song).Include(m => m.Song.Singer)
                .Where(m => m.IdPlaylist == idPlayList).ToList();
            if (allPlayListDetail.Count != 0)
            {
                List<SongModel> listSong = new List<SongModel>();
                foreach (var item in allPlayListDetail)
                {
                    listSong.Add(item.Song);
                }
                ViewBag.listSong = JsonConvert.SerializeObject(listSong);
                ViewBag.Title = "Những bài hát thuộc Playlist: ";
                return View("SongDetail", listSong[0]);
            }
            else
            {
                return RedirectToAction("PlayListUser");
            }
        }
        [Route("infoPlayList/{idPlayList}")]
        public IActionResult InfoPlayList(int idPlayList)
        {
            var playList = _context.Playlist.Find(idPlayList);
            var playListDetail = _context.PlaylistDetail.Where(m => m.IdPlaylist == idPlayList).ToList();
            if (playListDetail.Count > 0)
            {
                var song = _context.Song.Find(playListDetail[0].IdSong);
                playList.Image = song.URLImg;
            }
            //Xóa cookie idPlayList
            Response.Cookies.Delete("idPlayList");
            //Tạo cookie idPlayList
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddMilliseconds(10000000);
            Response.Cookies.Append("idPlayList", idPlayList.ToString(), option);
            return View(playList);
        }
        [Route("playListUsers/{idSong}")]
        public IActionResult PagePlayListDetail1(int idSong)
        {
            var song = _context.Song.Find(idSong);
            var singer = _context.Singer.Find(song.IdSinger);
            song.Singer = singer;
            //Get cookie idPlayList
            int idPlayList = int.Parse(Request.Cookies["idPlayList"]);
            var allPlayListDetail = _context.PlaylistDetail.Include(m => m.Song).Include(m => m.Song.Singer).Where(m => m.IdPlaylist == idPlayList).ToList();
            List<SongModel> listSong = new List<SongModel>();
            listSong.Add(song);
            foreach (var item in allPlayListDetail)
            {
                if (item.IdSong != song.Id)
                {
                    listSong.Add(item.Song);
                }
            }
            ViewBag.listSong = JsonConvert.SerializeObject(listSong);
            ViewBag.Title = "Những bài hát thuộc Playlist: ";
            return View("SongDetail", listSong[0]);
        }
        public async Task<IActionResult> CreatePlayListUser(string txtName)
        {
            //Get thông tin user
            var user = _userManager.GetUserAsync(User);
            //Tạo PlayList mới
            Playlist model = new Playlist();
            model.Name = txtName;
            model.IdUser = user.Result.Id;
            _context.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToRoute(new
            {
                controller = "Home",
                action = "infoPlayList",
                idPlayList = model.Id
            });
        }
        public async Task<IActionResult> EditPlayListUser(int idPlayList, string txtName)
        {
            Playlist playList = new Playlist();
            playList = _context.Playlist.Find(idPlayList);
            playList.Name = txtName;
            _context.Update(playList);
            await _context.SaveChangesAsync();
            return RedirectToRoute(new
            {
                controller = "Home",
                action = "InfoPlayList",
                idPlayList = idPlayList
            });
        }
        public async Task<IActionResult> RemovePlayListUser(int idPlayList)
        {
            var playPlist = _context.Playlist.Find(idPlayList);
            _context.Playlist.Remove(playPlist);
            await _context.SaveChangesAsync();
            return RedirectToAction("PlayListUser");
        }
        [HttpPost]
        public async Task<IActionResult> AddSongFromPlayListUser(List<SongModel> listSuggestedSong, int idSong)
        {
            //Get cookie idPlayList
            int idPlayList = idPlayList = int.Parse(Request.Cookies["idPlayList"]); ;
            //Thêm Song vào PlayList
            PlaylistDetail model = new PlaylistDetail();
            model.IdPlaylist = idPlayList;
            model.IdSong = idSong;
            _context.Add(model);
            await _context.SaveChangesAsync();
            //Xóa Song đã thêm vào PlayList khỏi listSuggestedSong
            foreach (SongModel item in listSuggestedSong)
            {
                if (item.Id == idSong)
                {
                    listSuggestedSong.Remove(item);
                    break;
                }
            }
            //Thêm 1 Song vào listSuggestedSong
            //Get danh sách bài hát có trong PlayList
            var detailPlayList = _context.PlaylistDetail.Include(m => m.Song)
                .Where(m => m.IdPlaylist == idPlayList).Include(m => m.Song.Singer).ToList();
            //Tạo danh sách bài hát có các bài hát trong PlayList và SuggestedSongList
            List<SongModel> list = new List<SongModel>();
            foreach (var item in listSuggestedSong)
            {
                list.Add(item);
            }
            foreach (var item in detailPlayList)
            {
                list.Add(item.Song);
            }
            //Get tất cả Song từ database
            var listSong = _context.Song.Include(m => m.Singer).ToList();
            //Thêm Song vào SuggestedSongList
            foreach (var item in listSong)
            {
                int count = list.Count;
                foreach (var i in list)
                {
                    if (item.Id != i.Id)
                    {
                        count--;
                    }
                }
                if (count == 0)
                {
                    listSuggestedSong.Add(item);
                }
                if (listSuggestedSong.Count == 5)
                {
                    break;
                }
            }
            return PartialView("_SuggestedSongListPartial", listSuggestedSong);
        }
        [HttpGet]
        public async Task<IActionResult> RemoveSongFromPlayListUser(int idPlayListDetail)
        {
            var detailPlayList = _context.PlaylistDetail.Find(idPlayListDetail);
            _context.PlaylistDetail.Remove(detailPlayList);
            await _context.SaveChangesAsync();
            return RedirectToAction("LoadListSongFromPlayListUser", detailPlayList.IdPlaylist);
        }
        [HttpGet]
        public IActionResult LoadListSongFromPlayListUser()
        {
            int idPlayList = int.Parse(Request.Cookies["idPlayList"]);
            List<PlaylistDetail> listSong = _context.PlaylistDetail
                .Where(m => m.IdPlaylist == idPlayList).Include(m => m.Song).Include(m => m.Song.Singer).ToList();
            return PartialView("_ListSongPartial", listSong);
        }
        [HttpGet]
        public IActionResult LoadSuggestedSongList()
        {
            int idPlayList = int.Parse(Request.Cookies["idPlayList"]);
            //Get danh sách bài hát trong PlayList
            var listSong = _context.PlaylistDetail.Where(m => m.IdPlaylist == idPlayList).ToList();
            //Get tất cả các bài hát trong database
            var allSong = _context.Song.Include(m => m.Singer).ToList();
            //Tạo list gợi ý
            List<SongModel> listSuggestedSong = new List<SongModel>();
            foreach (var item in allSong)
            {
                int count = listSong.Count;
                foreach (var i in listSong)
                {
                    if (item.Id != i.IdSong)
                    {
                        count--;
                    }
                }
                if (count == 0)
                {
                    listSuggestedSong.Add(item);
                }
                if (listSuggestedSong.Count > 4)
                {
                    break;
                }
            }
            return PartialView("_SuggestedSongListPartial", listSuggestedSong);
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

        public async Task<bool> UpdateViewSongOfDay(int IdSong)
        {
            bool result = false;
            if (IdSong != 0)
            {
                //Lấy thống kê bài hát theo ngày hiện có
                var existViewSongOfDay = _context.ViewSongOfDay.Where(m => m.Date == DateTime.Now.Date).ToList();
                if (existViewSongOfDay.Count == 0 || existViewSongOfDay == null)
                {
                    //Tạo mới Thống kê bài hát theo ngày
                    ViewSongOfDay viewSongOfDay = new ViewSongOfDay();
                    //Chỉ lưu ngày không lưu thời gian
                    viewSongOfDay.Date = DateTime.Now.Date;
                    _context.Add(viewSongOfDay);
                    await _context.SaveChangesAsync();
                    //Tạo chi tiết thống kê bài hát
                    if (viewSongOfDay.Id != 0)
                    {
                        ViewSongOfDayDetail viewSongOfDayDetail = new ViewSongOfDayDetail();
                        viewSongOfDayDetail.IdSong = IdSong;
                        viewSongOfDayDetail.IdViewSongOfDay = viewSongOfDay.Id;
                        //Tạo mới số lượt nghe
                        viewSongOfDayDetail.CountView = 1;
                        _context.Add(viewSongOfDayDetail);
                        await _context.SaveChangesAsync();
                        result = true;
                    }
                }
                else
                {
                    //Tìm kiếm chi tiết thống kê bài hát
                    var existViewSongOfDayDetails = _context.ViewSongOfDayDetail.Where(m => m.IdSong == IdSong && m.IdViewSongOfDay == existViewSongOfDay.FirstOrDefault().Id).ToList();
                    if(existViewSongOfDayDetails.Count == 0 || existViewSongOfDayDetails == null)
                    {
                        ViewSongOfDayDetail viewSongOfDayDetail = new ViewSongOfDayDetail();
                        viewSongOfDayDetail.IdSong = IdSong;
                        viewSongOfDayDetail.IdViewSongOfDay = existViewSongOfDay.FirstOrDefault().Id;
                        //Tạo mới số lượt nghe
                        viewSongOfDayDetail.CountView = 1;
                        _context.Add(viewSongOfDayDetail);
                        await _context.SaveChangesAsync();
                        result = true;
                    }
                    else
                    {
                        var existViewSongOfDayDetail = existViewSongOfDayDetails.FirstOrDefault();
                        if (existViewSongOfDayDetail.CountView != 0)
                            existViewSongOfDayDetail.CountView++;
                        else
                            existViewSongOfDayDetail.CountView = 1;

                        _context.Update(existViewSongOfDayDetail);
                        await _context.SaveChangesAsync();

                        result = true;
                    }
                }
            }

            return result;
        }
    }
}
