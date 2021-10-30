using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.API.Code
{
    public class JWT
    {
        public static readonly string tokenKey = "005444bb-8679-4a19-8bea-ee5b630bd6a1";
        public static readonly string accessKey = "3c155498-865d-43db-8ef7-9080d6ccfcc5";
        public static readonly string secretkey = "45c098c9-5d0d-4580-9bc9-f69fa48238d2";
        private const double EXPIRY_DURATION_MINUTES = 30;     

        public bool Verify(string AccessToken)
        {
            bool result = false;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(tokenKey);
                tokenHandler.ValidateToken(AccessToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var AccessKey = jwtToken.Claims.First(x => x.Type == "accesskey").Value.ToString();
                var SecretKey = jwtToken.Claims.First(x => x.Type == "secretkey").Value.ToString();

                result = CheckKey (AccessKey, SecretKey);
            }
            catch
            {
                //result = false;
            }
            return result;
        }

        public string sign()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(tokenKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("accesskey", accessKey),
                    new Claim("secretkey", secretkey)
                }),
                Expires = DateTime.Now.AddMinutes(EXPIRY_DURATION_MINUTES),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool CheckKey( string accesskey, string secretkey)
        {
            bool result = false;
            if (!String.IsNullOrWhiteSpace(accesskey) && !String.IsNullOrWhiteSpace(secretkey))
            {
                if (accesskey.ToLower() == accessKey && secretkey.ToLower() == secretkey)
                {
                    result = true;
                }
            }
            return result;
        }

    }
}
