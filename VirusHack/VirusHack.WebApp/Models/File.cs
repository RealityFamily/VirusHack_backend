using System;
using System.Runtime.Serialization;

namespace VirusHack.WebApp.Models
{
    public class File
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public Webinar Webinar { get; set; }
    }
}
