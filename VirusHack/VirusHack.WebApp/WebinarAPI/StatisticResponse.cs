using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirusHack.WebApp.WebinarAPI
{
    public class StatisticResponse
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string SecondName { get; set; }
        public string PatrName { get; set; }
        public string Phone { get; set; }
        public string Sex { get; set; }
        public List<EventSession> EventSessions { get; set; }
        public string Organization { get; set; }
        public string Position { get; set; }
    }
}
