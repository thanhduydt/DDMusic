using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.API.Model
{
    public class ApiTokenResponse
    {
        public string Token { get; set; }
        public string ErrorCode { get; set; }

        public ApiTokenResponse()
        {
            this.Token = string.Empty;
            this.ErrorCode = ApiCode.FATAL_ERROR;
        }

        public ApiTokenResponse(string errorCode)
        {
            this.ErrorCode = errorCode;
            this.Token = string.Empty;
        }

        public ApiTokenResponse(string errorCode, string token) : this(errorCode)
        {
            this.Token = token;
        }
    }
}
