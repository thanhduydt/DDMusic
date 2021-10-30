using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.API.Model
{
    public class ApiTokenRequest
    {
        /// <summary>
        /// Khóa truy cập
        /// </summary>
        public string AccessKey { get; set; }
        /// <summary>
        /// Khóa bí mật
        /// </summary>
        public string SecretKey { get; set; }
    }
}
