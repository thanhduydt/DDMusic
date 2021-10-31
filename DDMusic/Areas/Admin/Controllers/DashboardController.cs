using DDMusic.Areas.Admin.API.Model;
using DDMusic.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        public readonly string endPoint = String.Empty;
        public readonly string accessKey = String.Empty;
        public readonly string secretKey = String.Empty;
        private readonly IConfiguration _configuration;
        public DashboardController(IConfiguration configuration)
        {
            _configuration = configuration;
            endPoint = _configuration.GetValue<string>("Jwt:EndPoint");
            accessKey = _configuration.GetValue<string>("Jwt:AccessKey");
            secretKey = _configuration.GetValue<string>("Jwt:SecretKey");
        }

        //Dashboard Top 10 song
        public async Task<IActionResult> Index()
        {
            if (!String.IsNullOrWhiteSpace(endPoint) && !String.IsNullOrWhiteSpace(accessKey) && !String.IsNullOrWhiteSpace(secretKey))
            {
                //string accessToken = await GetToken();
                //List<SongModel> topSongs = new List<SongModel>();
                //using (var client = new HttpClient())
                //{
                //    client.BaseAddress = new Uri(endPoint);
                //    string urlPath = "api/song/gettopsong";
                //    //Add AccessToken vào header
                //    client.DefaultRequestHeaders.Add("AccessToken", accessToken);

                //    var response = await client.GetAsync(urlPath);


                //    string result = response.Content.ReadAsStringAsync().Result;

                //    topSongs = JsonConvert.DeserializeObject<List<SongModel>>(result);

                //}
                //ViewBag.TopSong = topSongs;
                //await GetViewSongOfDay(DateTime.Now.Date);

            }

            return View();
        }
        
        //Hàm lấy
        public async Task<List<ViewSongOfDayDetail>> GetViewSongOfDay(DateTime date)
        {
            string accessToken = await GetToken();
            List<ViewSongOfDayDetail> viewSongOfDays = new List<ViewSongOfDayDetail>();

            string urlPath = "api/song/GetViewSongOfDay";
            var client = new RestClient(endPoint);
            var request = new RestRequest(urlPath, Method.GET);
            request.AddHeader("AccessToken", accessToken);
            request.AddParameter("date", date.Date.ToString());
            IRestResponse response = client.Execute(request);


            viewSongOfDays = JsonConvert.DeserializeObject<List<ViewSongOfDayDetail>>(response.Content);

            return viewSongOfDays;
        }

        public async Task<string> GetToken()
        {
            string AccessToken = String.Empty;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(endPoint);
                //Lấy AccessToken
                string urlPath = "api/token/gettoken";
                var requestData = new ApiTokenRequest();
                requestData.AccessKey = accessKey;
                requestData.SecretKey = secretKey;

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
                var data = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(urlPath, data);

                string result = response.Content.ReadAsStringAsync().Result;

                ApiTokenResponse apiToken = JsonConvert.DeserializeObject<ApiTokenResponse>(result);

                AccessToken = apiToken.Token;
            }
            return AccessToken;
        }
    }
}
