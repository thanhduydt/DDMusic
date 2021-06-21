using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using DDMusic.Areas.Admin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using DDMusic.Views.Shared.Components.MessagePage;

namespace DDMusic.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<UserModel> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<UserModel> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            if (!_signInManager.IsSignedIn(User)) return RedirectToPage("/Index");


            await _signInManager.SignOutAsync();
            _logger.LogInformation("Người dùng đăng xuất");
            return RedirectToAction(nameof(Index));

            //return ViewComponent(MessagePage.COMPONENTNAME,
            //new MessagePage.Message()
            //{
            //title = "Đã đăng xuất",
            //htmlcontent = "Đăng xuất thành công",
            //urlredirect = (returnUrl != null) ? returnUrl : Url.Page("/Index")
            //}
            //);
        }
    }
}
