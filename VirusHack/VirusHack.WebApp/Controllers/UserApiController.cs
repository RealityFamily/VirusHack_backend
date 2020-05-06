using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using VirusHack.WebApp.Attributes;
using Microsoft.AspNetCore.Authorization;
using VirusHack.WebApp.Models;
using VirusHack.WebApp.Responces;
using VirusHack.WebApp.Requests;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace VirusHack.WebApp.Controllers
{
    //[Authorize(AuthenticationSchemes = "Bearer")]
    public class UserApiController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly AppDatabaseContext context;
        private readonly IConfiguration configuration;

        public UserApiController(UserManager<User> userManager,
            AppDatabaseContext context,
            IConfiguration configuration)
        {
            this.userManager = userManager;
            this.context = context;
            this.configuration = configuration;
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserInfo(string token)
        {
            if (token != null)
            {
                var userId = await ReadTokenAsync(token);
                var user = await userManager.FindByIdAsync(userId.ToString());
                var group = await context.Groups.FindAsync(user.GroupId);
                if (user != null)
                {
                    return new JsonResult(new
                    {
                        Id = userId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Group = group.Name,
                        UserStatus = user.UserStatus.ToString()
                    });
                }
                return BadRequest("User not found");
            }
            return BadRequest("No token");
        }

        [HttpPost("user/login")]
        public async Task<IActionResult> LoginUser([FromBody]LoginRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Login);

            if (await userManager.CheckPasswordAsync(user, request.Password))
            {
                var now = DateTime.UtcNow;
                var jwt = new JwtSecurityToken(
                    issuer: configuration.GetSection("JwtOptions")["Issuer"],
                    audience: configuration.GetSection("JwtOptions")["Audience"],
                    notBefore: now,
                    claims: GetIdentity(request).GetAwaiter().GetResult().Claims,
                    expires: now.Add(TimeSpan.FromMinutes(2)),
                    signingCredentials: GetSecretKey()
                    );

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                return new JsonResult(new { Token = encodedJwt });
            }
            return BadRequest("Invalid username or password");
        }

        public static string ReadToken(string jwtInput)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtOutput = string.Empty;

            if (!jwtHandler.CanReadToken(jwtInput))
            {
                return "Invalid token format";
            }

            var token = jwtHandler.ReadJwtToken(jwtInput);
            var jwtPayload = token.Claims.FirstOrDefault(c => c.Type == "UserId");
            return jwtPayload.Value;
        }
        public static async Task<string> ReadTokenAsync(string jwtInput)
        {
            return await Task.Run(() =>
            {
                return ReadToken(jwtInput);
            });
        }
        private async Task<ClaimsIdentity> GetIdentity(LoginRequest user)
        {
            var result = await userManager.FindByEmailAsync(user.Login);
            if (result != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, result.Email),
                    new Claim("UserId", result.Id.ToString())
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                    claims, JwtBearerDefaults.AuthenticationScheme,
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);

                return claimsIdentity;
            }
            return null;
        }
        private SigningCredentials GetSecretKey()
        {
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetSection("JwtOptions")["Key"]));
            var secretKey = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            return secretKey;
        }
    }
}