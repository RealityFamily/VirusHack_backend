using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using VirusHack.WebApp.Models.AssociationEntities;

namespace VirusHack.WebApp.Models
{
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<User> Students { get; set; }
        public List<GroupWebinar> Webinars { get; set; }
    }
}
