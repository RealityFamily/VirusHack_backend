using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirusHack.WebApp.WebinarAPI
{
    public class Connection
    {
        public string Joined { get; set; }
        public string Leaved { get; set; }
        public long Duration { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Platform { get; set; }
    }
}
