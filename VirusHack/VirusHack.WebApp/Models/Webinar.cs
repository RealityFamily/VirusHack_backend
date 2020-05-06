using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using VirusHack.WebApp.Models.AssociationEntities;

namespace VirusHack.WebApp.Models
{
    public class Webinar
    {
        public Guid Id { get; set; }
        public string SiteEventId { get; set; }
        public string EventSessionId { get; set; }
        public string Discipline { get; set; }
        public List<GroupWebinar> Groups { get; set; }
        public User Teacher { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public List<User> Present { get; set; }
        public LessonType TypeLesson { get; set; }
        public LessonStatus LessonStatus { get; set; }
        public List<File> Files { get; set; }
    }
}
