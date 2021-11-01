using DDMusic.Areas.Admin.API.Code;
using DDMusic.Areas.Admin.Data;
using DDMusic.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.API.Controllers
{

    [Area("Admin"), Route("api/user/[action]")]
    [APIJWTAttribute]
    public class UserAPIController : ControllerBase
    {
        private readonly DPContext _context;
        public UserAPIController(DPContext context)
        {
            _context = context;
        }
        [HttpGet, ProducesResponseType(200)]
        public async Task<List<CountNewAccountModel>> GetCountNewAccount(string min_date, string max_date)
        {
            try
            {
                DateTime min_Date = Convert.ToDateTime(min_date);
                DateTime max_Date = Convert.ToDateTime(max_date);
                List<CountNewAccountModel> CountNewAccounts = _context.CountNewAccount.Where(m => m.Date >= min_Date.Date && m.Date <= max_Date.Date).ToList();
                return CountNewAccounts == null ? new List<CountNewAccountModel>() : CountNewAccounts;
            }
            catch
            {
                return new List<CountNewAccountModel>();
            }
        }
    }
}