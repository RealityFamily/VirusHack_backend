using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using VirusHack.WebApp.Models;

namespace VirusHack.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
//#if DEBUG
//            services.AddDbContext<AppDatabaseContext>(config =>
//                config.UseSqlServer(Configuration.GetConnectionString("VirusHacKDb"),
//                options => options.MigrationsAssembly("VirusHack.WebApp")));
//#else
            services.AddDbContext<AppDatabaseContext>(config =>
                config.UseNpgsql(Configuration.GetConnectionString("Postgres"),
                options => options.MigrationsAssembly("VirusHack.WebApp")));
//#endif
            services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 0;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<AppDatabaseContext>()
            .AddDefaultTokenProviders();

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("JwtOptions")["Key"]));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
              .AddJwtBearer(options =>
              {
                  options.RequireHttpsMetadata = false;
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidIssuer = Configuration.GetSection("JwtOptions")["Issuer"],

                      ValidateAudience = true,
                      ValidAudience = Configuration.GetSection("JwtOptions")["Audience"],

                      ValidateLifetime = true,

                      IssuerSigningKey = key,

                      ValidateIssuerSigningKey = true
                  };
              });

            services.AddAuthorization();

            services.AddMvc(config => config.EnableEndpointRouting = false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true
            });

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseMvc();
        }
    }
}
