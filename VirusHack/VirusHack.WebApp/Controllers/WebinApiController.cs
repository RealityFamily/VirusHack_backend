using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using VirusHack.WebApp.Attributes;
using VirusHack.WebApp.Models;
using VirusHack.WebApp.Models.AssociationEntities;
using VirusHack.WebApp.Responces;
using VirusHack.WebApp.WebinarAPI;

namespace VirusHack.WebApp.Controllers
{
    [ApiController]
    public class WebinApiController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly AppDatabaseContext context;
        private readonly IConfiguration configuration;

        public WebinApiController(UserManager<User> userManager,
            AppDatabaseContext context,
            IConfiguration configuration)
        {
            this.userManager = userManager;
            this.context = context;
            this.configuration = configuration;
        }


        [HttpPost("webin/create")]
        public async Task<IActionResult> CreateWebinar([FromBody] CreateWebinarEventRequest req)
        {

            var client = new RestClient("https://userapi.webinar.ru/v3/events");
            var request = new RestRequest(Method.POST);
            request.AddHeader("x-auth-token", "8368b48e9ebc600cf40f52cd9b007c5e");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("name", req.Name);
            request.AddParameter("access", req.Access);
            var dateTime = DateTime.Parse(req.StartDate);
            request.AddParameter("startsAt[date][year]", dateTime.Year);
            request.AddParameter("startsAt[date][month]", dateTime.Month);
            request.AddParameter("startsAt[date][day]", dateTime.Day);
            request.AddParameter("startsAt[time][hour]", dateTime.Hour);
            request.AddParameter("startsAt[time][minute]", dateTime.Minute);
            request.AddParameter("description", $"{req.Groups[0]}");

            IRestResponse response = client.Execute(request);
            var json = JsonConvert.DeserializeObject<CreateWebinarEventResponce>(response.Content.ToString());

            var client1 = new RestClient($"https://userapi.webinar.ru/v3/events/{json.EventId}/sessions");
            var request1 = new RestRequest(Method.POST);
            request1.AddHeader("x-auth-token", "8368b48e9ebc600cf40f52cd9b007c5e");
            request1.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request1.AddParameter("name", req.Name);
            request1.AddParameter("access", req.Access);

            IRestResponse response1 = client1.Execute(request);
            var json1 = JsonConvert.DeserializeObject<CreateWebinarSessionResponce>(response1.Content.ToString());

            List<GroupWebinar> groups = new List<GroupWebinar>();
            foreach (var name in req.Groups)
            {
                var res = context.Groups.FirstOrDefault(n => n.Name == name);
                groups.Add(new GroupWebinar { GroupId = res.Id });
            }


            var teacher = await userManager.FindByEmailAsync(req.Email);
            var webinar = new Webinar
            {
                Discipline = req.Name,
                StartTime = DateTime.Parse(req.StartDate),
                Groups = groups,
                SiteEventId = json.EventId,
                EventSessionId = json1.EventSessionId,
                TypeLesson = LessonType.Lecture,
                LessonStatus = LessonStatus.Future,
                Teacher = teacher
            };
            await context.Webinars.AddAsync(webinar);
            await context.SaveChangesAsync();
            return Ok(response1.Content);
        }

        [HttpGet("webin/{webinar_id}/connect")]
        public async Task<IActionResult> GetWebinarConnection(string token, Guid webinar_Id)
        {
            var userId = await ReadTokenAsync(token);
            var user = await userManager.FindByIdAsync(userId.ToString());
            var webinar = await context.Webinars.FindAsync(webinar_Id);


            var client = new RestClient($"https://userapi.webinar.ru/v3/eventsessions/{webinar.EventSessionId}/register");
            var request = new RestRequest(Method.POST);
            request.AddHeader("x-auth-token", "8368b48e9ebc600cf40f52cd9b007c5e");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("email", user.Email);
            request.AddParameter("name", user.FirstName);
            request.AddParameter("secondName", user.LastName);
            request.AddParameter("isAutoEnter", "true");

            IRestResponse response = client.Execute(request);
            var json = JsonConvert.DeserializeObject<WebinarConnectResponse>(response.Content.ToString());

            return new JsonResult(new { link = json.Link, ParticipationId = json.ParticipationId });
        }

        [HttpGet("webin/{participationId}/delete")]
        public async Task<IActionResult> DeleteUserFromWebinar(string participationId)
        {
            var client = new RestClient("https://userapi.webinar.ru/v3/participations/delete");
            var request = new RestRequest(Method.POST);
            request.AddHeader("x-auth-token", "8368b48e9ebc600cf40f52cd9b007c5e");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("participationIds[0]", participationId);
            request.AddParameter("role", "ADMIN");

            IRestResponse response = client.Execute(request);
            return Ok(response);
        }

        [HttpGet("webin/{webinar_id}/static")]
        public async Task<IActionResult> GetWebinarStatic(string token, Guid webinar_Id)
        {
            var result = await context.Webinars.FindAsync(webinar_Id);
            var eventId = result.SiteEventId;

            var client = new RestClient($"https://userapi.webinar.ru/v3/stats/users?from=2020-04-01&to=2020-06-01&eventId={eventId}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-auth-token", "8368b48e9ebc600cf40f52cd9b007c5e");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            IRestResponse response = client.Execute(request);
            var json = JsonConvert.DeserializeObject<List<StatisticResponse>>(response.Content.ToString());

            var webinars = context.Webinars.Include(w => w.Groups).ThenInclude(g => g.Group);
            var webinar = webinars.Include(w => w.Teacher).FirstOrDefault(w => w.Id == webinar_Id);
            //List<UserView> userViews
            //foreach (var item in collection)
            //{

            //}
            var res = new WebinarsResponce
            {
                Id = webinar.Id,
                Discipline = webinar.Discipline,
                Groups = webinar.Groups.Select(g => new GroupView { Id = g.Group.Id, Name = g.Group.Name }).ToList(),
                Teacher = new TeacherView
                {
                    Id = webinar.Teacher.Id,
                    FirstName = webinar.Teacher.FirstName,
                    LastName = webinar.Teacher.LastName,
                    Email = webinar.Teacher.Email,
                    UserStatus = webinar.Teacher.UserStatus.ToString()
                },
                StartTime = webinar.StartTime.ToString(),
                EndTime = json[0].EventSessions[0].EndsAt,
                Present = null,
                TypeLesson = webinar.TypeLesson.ToString(),
                LessonStatus = webinar.LessonStatus.ToString(),
                Files = null
            };
            return new JsonResult(result);
        }

        [HttpGet("webin/day")]
        public async Task<IActionResult> GetDayWebinars(string token)
        {
            if (token != null)
            {
                var userId = await ReadTokenAsync(token);
                var user = await userManager.FindByIdAsync(userId.ToString());
                var groupId = user.Group.Id;
                var now = DateTime.Now.Date;

                var webinars = context.Webinars.Include(w => w.Groups).ThenInclude(g => g.Group).ThenInclude(s => s.Students);
                var weekWebinars = webinars.Where(w => w.StartTime.Date == now);

                return new JsonResult(weekWebinars.Select(w => new WebinarsResponce
                {
                    Id = w.Id,
                    Discipline = w.Discipline,
                    Groups = w.Groups.Select(g => new GroupView { Id = g.Group.Id, Name = g.Group.Name }).ToList(),
                    Teacher = new TeacherView
                    {
                        Id = w.Teacher.Id,
                        FirstName = w.Teacher.FirstName,
                        LastName = w.Teacher.LastName,
                        Email = w.Teacher.Email,
                        UserStatus = w.Teacher.UserStatus.ToString()
                    },
                    StartTime = w.StartTime.ToString(),
                    EndTime = w.EndTime.ToString(),
                    Present = null,
                    TypeLesson = w.TypeLesson.ToString(),
                    Files = null
                }));
            }
            return BadRequest("No token");
        }

        [HttpGet("webin")]
        public async Task<IActionResult> GetWeekWebinars(string token)
        {
            if (token != null)
            {
                var userId = await ReadTokenAsync(token);
                var user = await userManager.FindByIdAsync(userId.ToString());
                var groupId = user.GroupId;
                var now = DateTime.Now.Date;
                var week = DateTime.Now.Date.AddDays(7);

                var webinars = context.Webinars.Include(w => w.Groups).ThenInclude(g => g.Group).ThenInclude(s => s.Students);
                var weekWebinars = webinars.Where(w => w.StartTime.Date >= now && w.StartTime.Date <= week).Include(t => t.Teacher);
                var result = weekWebinars.Select(w => new WebinarsResponce
                {
                    Id = w.Id,
                    Discipline = w.Discipline,
                    Groups = w.Groups.Select(g => new GroupView { Id = g.Group.Id, Name = g.Group.Name }).ToList(),
                    Teacher = new TeacherView
                    {
                        Id = w.Teacher.Id,
                        FirstName = w.Teacher.FirstName,
                        LastName = w.Teacher.LastName,
                        Email = w.Teacher.Email,
                        UserStatus = w.Teacher.UserStatus.ToString()
                    },
                    StartTime = w.StartTime.ToString(),
                    EndTime = w.EndTime.ToString(),
                    Present = null,
                    TypeLesson = w.TypeLesson.ToString(),
                    LessonStatus = w.LessonStatus.ToString(),
                    Files = null
                });
                return new JsonResult(result);
            }
            return BadRequest("No token");
        }



        //[HttpGet("webin/{webinar_id}/files/{file_id}")]
        //public async Task<IActionResult> GetWebinarFileToDownload([FromQuery][Required()]string token, [FromRoute][Required]Guid? webinarId, [FromRoute][Required]Guid? fileId)
        //{
        //   
        //}

        //[HttpGet("webin/{webinar_id}/files")]
        //public async Task<IActionResult> GetWebinarFiles([FromQuery][Required()]string token, [FromRoute][Required]Guid? webinarId)
        //{
        //   
        //}

        //[HttpPost("webin/{webinar_id}/files")]
        //public async Task<IActionResult> PostWebinarFiles([FromQuery][Required()]string token, [FromRoute][Required]Guid? webinarId, [FromForm][Required()]File uploadedFile)
        //{
        //   
        //}

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
    }
}