using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirusHack.WebApp.Responces
{
    public class TeacherView
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserStatus { get; set; }
    }
}
