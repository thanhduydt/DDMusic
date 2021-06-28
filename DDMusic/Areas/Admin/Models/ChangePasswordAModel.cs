using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.Models
{
    public class ChangePasswordAModel
    {
        [Display(Name ="Nhập UserName hoặc Email")]
        public string EmailOrUserName { get; set; }
        [Display(Name ="Nhập mật khẩu mới")]
        public string Password { get; set; }
    }
}
