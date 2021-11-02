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
            return View();
        }

        public async Task<IActionResult> GetTopSong()
        {
            if (!String.IsNullOrWhiteSpace(endPoint) && !String.IsNullOrWhiteSpace(accessKey) && !String.IsNullOrWhiteSpace(secretKey))
            {
                string accessToken = await GetToken();
                List<SongModel> topSongs = new List<SongModel>();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(endPoint);
                    string urlPath = "api/song/gettopsong";
                    //Add AccessToken vào header
                    client.DefaultRequestHeaders.Add("AccessToken", accessToken);

                    var response = await client.GetAsync(urlPath);


                    string result = response.Content.ReadAsStringAsync().Result;

                    topSongs = JsonConvert.DeserializeObject<List<SongModel>>(result);

                }

                #region Mapping ListSongModel to DashboardModel
                DashboardModel dashboard = new DashboardModel();
                DataSet dataSet = new DataSet();
                var colors = new string[]
                {

                    "#255E91", "#997300", "#626262", "#9E480E", "#264478", "#70AD47",
                    "#5B9BD5", "#FFC000", "#A5A5A5", "#ED7D31"
                };
                int i = 0;
                foreach (var item in topSongs)
                {
                    dashboard.labels.Add(item.Name);
                    dataSet.data.Add(item.CountView);
                    dataSet.backgroundColor.Add(colors[i]);
                    i++;
                }
                dashboard.datasets.Add(dataSet);

                #endregion
                //await GetViewSongOfDay(DateTime.Now.Date);
                var resultjson = JsonConvert.SerializeObject(dashboard);
                return Content(resultjson, "application/json");
            }
            return null;
        }

        //Hàm lấy theo ngày
        public async Task<IActionResult> GetViewSongOfDay()
        {
            string accessToken = await GetToken();
            List<ViewSongOfDayDetail> viewSongOfDays = new List<ViewSongOfDayDetail>();

            string urlPath = "api/song/GetViewSongOfDay";
            var client = new RestClient(endPoint);
            var request = new RestRequest(urlPath, Method.GET);
            request.AddHeader("AccessToken", accessToken);
            request.AddParameter("date", DateTime.Now.Date.ToString());
            IRestResponse response = client.Execute(request);

            viewSongOfDays = JsonConvert.DeserializeObject<List<ViewSongOfDayDetail>>(response.Content);

            #region Mapping List<ViewSongOfDayDetail> to DashboardModel
            DashboardModel dashboard = new DashboardModel();
            DataSet dataSet = new DataSet();

            foreach (var item in viewSongOfDays)
            {
                dashboard.labels.Add(item.Song.Name);
                dataSet.data.Add(item.CountView);
            }
            dashboard.datasets.Add(dataSet);

            #endregion

            var resultjson = JsonConvert.SerializeObject(dashboard);
            return Content(resultjson, "application/json");
        }
        #region chưa dùng
        //public async Task<List<ViewSongOfDayDetail>> GetViewSongOfDay(DateTime min_date, DateTime max_date)
        //{
        //    string accessToken = await GetToken();
        //    List<ViewSongOfDayDetail> viewSongOfDays = new List<ViewSongOfDayDetail>();

        //    string urlPath = "api/song/GetViewSongOfDay";
        //    var client = new RestClient(endPoint);
        //    var request = new RestRequest(urlPath, Method.GET);
        //    request.AddHeader("AccessToken", accessToken);
        //    request.AddParameter("min_date", min_date.Date.ToString());
        //    request.AddParameter("max_date", max_date.Date.ToString());
        //    IRestResponse response = client.Execute(request);


        //    viewSongOfDays = JsonConvert.DeserializeObject<List<ViewSongOfDayDetail>>(response.Content);

        //    return viewSongOfDays;
        //}

        #endregion

        public async Task<IActionResult> GetCountNewAccountModels()
        {
            string accessToken = await GetToken();
            List<CountNewAccountModel> countNewAccountModels = new List<CountNewAccountModel>();

            string urlPath = "api/user/GetCountNewAccount";
            var client = new RestClient(endPoint);
            var request = new RestRequest(urlPath, Method.GET);
            request.AddHeader("AccessToken", accessToken);
            request.AddParameter("min_date", DateTime.Now.AddDays(-7).Date.ToString());
            request.AddParameter("max_date", DateTime.Now.Date.ToString());
            IRestResponse response = client.Execute(request);

            countNewAccountModels = JsonConvert.DeserializeObject<List<CountNewAccountModel>>(response.Content);

            #region Mapping List<CountNewAccountModel> to DashboardModel
            DashboardModel dashboard = new DashboardModel();
            DataSet dataSet = new DataSet();
            foreach (var item in countNewAccountModels.OrderBy(m => m.Date))
            {
                dashboard.labels.Add(String.Format("{0:dd/MM}",item.Date));
                dataSet.data.Add(item.Count);
            }
            dashboard.datasets.Add(dataSet);

            #endregion

            var resultjson = JsonConvert.SerializeObject(dashboard);
            return Content(resultjson, "application/json");

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
