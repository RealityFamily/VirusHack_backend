using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirusHack.WebApp.Models;

namespace VirusHack.WebApp
{
    public class AppDatabaseContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
    {
        public AppDatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> SystemUsers { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Webinar> Webinars { get; set; }
    }
}
