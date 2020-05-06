using Microsoft.AspNetCore.Identity;
using System;
using System.Runtime.Serialization;

namespace VirusHack.WebApp.Models
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserStatus UserStatus { get; set; }
        public Guid GroupId { get; set; }
        public Group Group { get; set; }
    }
}
