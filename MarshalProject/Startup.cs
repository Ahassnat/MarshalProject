using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarshalProject.Data;
using MarshalProject.Model;
using MarshalProject.Repository;
using MarshalProject.Repository.Area;
using MarshalProject.Repository.Shelter;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace MarshalProject
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
            services.AddMvc().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling =
                ReferenceLoopHandling.Ignore;
            });
            services.AddControllers();

            services.AddDbContext<DataContext>(opt =>

                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                sqlServerOptions => sqlServerOptions.UseNetTopologySuite())

          );
            //services.AddIdentityCore<ApplicationUser>()
            //   .AddRoles<ApplicationRole>()
            //   .AddRoleManager<RoleManager<ApplicationRole>>()
            //   .AddSignInManager<SignInManager<ApplicationUser>>()
            //   .AddRoleValidator<RoleValidator<ApplicationRole>>()
            //   .AddEntityFrameworkStores<DataContext>();

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders()
                    .AddRoles<ApplicationRole>(); ;

            services.AddTransient<IAdminRepo, AdminRepo>();
            services.AddTransient<IAreaRepo, AreaRepo>();
            services.AddTransient<IShelterRepo, ShelterRepo>();
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
            });


            services.AddAuthentication(options =>
            {

                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
         .AddCookie(options =>
         {
             options.Cookie.HttpOnly = true;
             options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
             options.SlidingExpiration = true;
             options.LogoutPath = "/Account/Logout";
             options.Cookie.SameSite = SameSiteMode.Lax;
             options.Cookie.IsEssential = true;
         });

            //services.AddAuthentication();
            // services.AddAuthorization();
            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("AdminRole", policy => policy.RequireRole("Admin"));
                opt.AddPolicy("CivilAndAdminRole", policy => policy.RequireRole("Admin", "CivilDefense"));
                opt.AddPolicy("CivilRole", policy => policy.RequireRole("CivilDefense"));

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));
           
           
            app.UseAuthentication();
            app.UseCookiePolicy();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
