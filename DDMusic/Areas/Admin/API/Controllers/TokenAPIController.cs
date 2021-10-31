using DDMusic.Areas.Admin.API.Code;
using DDMusic.Areas.Admin.API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.API.Controllers
{
    [Area("Admin"), Route("api/token/[action]")]
    [AllowAnonymous]
    public class TokenAPIController : ControllerBase
    {
        public JWT JWT;
        public TokenAPIController()
        {
            JWT = new JWT();
        }
        [HttpPost, ProducesResponseType(200)]
        public async Task<ApiTokenResponse> GetToken([FromBody] ApiTokenRequest model)
        {
            if (!ModelState.IsValid)
                return new ApiTokenResponse(ApiCode.INVALID_INPUT_DATA);

            try
            {
                if (model.AccessKey.ToLower() != JWT.accessKey || model.SecretKey.ToLower() != JWT.secretkey)
                    return new ApiTokenResponse(ApiCode.NOT_FOUND);

                var validToken = JWT.sign();
                var dateTest = DateTime.Now.Date;
                return String.IsNullOrWhiteSpace(validToken) ? new ApiTokenResponse() : new ApiTokenResponse(ApiCode.SUCCESS, validToken);
                //return new ApiTokenResponse();
            }
            catch
            {
                return new ApiTokenResponse();
            }
        }
    }
}
