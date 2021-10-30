using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.API.Code
{
    public class APIJWTAttribute : ActionFilterAttribute
    {
        private readonly JWT _JWT;

        public APIJWTAttribute()
        {
            _JWT = new JWT();          
        }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            var req = context.HttpContext.Request;
            var auth = req.Headers["AccessToken"];

            if (_JWT.Verify(auth))
                // Yêu cầu hợp lệ
                return base.OnActionExecutionAsync(context, next);


            return Task.Run(() =>
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
            });
        }

    }
}
