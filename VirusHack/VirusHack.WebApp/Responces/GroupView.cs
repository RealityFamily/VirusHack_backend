using System;
using System.Collections.Generic;
using VirusHack.WebApp.Models;

namespace VirusHack.WebApp.Responces
{
    public class GroupView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<User> Students { get; set; }
    }
}