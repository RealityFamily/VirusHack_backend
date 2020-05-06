using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirusHack.WebApp.Models;

namespace VirusHack.WebApp.Responces
{
    public class WebinarsResponce
    {
        public Guid Id { get; set; }
        public string Discipline { get; set; }
        public List<GroupView> Groups { get; set; }
        public TeacherView Teacher { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public List<UserView> Present { get; set; }
        public string TypeLesson { get; set; }
        public string LessonStatus { get; set; }
        public List<File> Files { get; set; }
    }
}
