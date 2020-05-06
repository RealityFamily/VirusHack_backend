using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirusHack.WebApp.WebinarAPI
{
    public class CreateWebinarEventRequest
    {
        public string Name { get; set; }
        public int Access { get; set; }
        public string StartDate { get; set; }
        public string[] Groups { get; set; } 

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }

    }
}
