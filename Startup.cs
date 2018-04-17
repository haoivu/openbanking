using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using openbanking.Data;
using openbanking.Models;
using openbanking.Services;
using Newtonsoft.Json.Linq;


namespace openbanking
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

            // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            //     {
            //         options.TokenValidationParameters = new TokenValidationParameters
            //         {
            //             ValidateIssuer = true,
            //             ValidateAudience = true,
            //             ValidateLifetime = true,
            //             ValidateIssuerSigningKey = true,
            //             ValidIssuer = Configuration["NordeaOB:Issuer"],
            //             ValidAudience = Configuration["NordeaOB:Issuer"],
            //             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["NordeaOB:key"]))
            //         };
            //     });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            //services.AddDbContext<ApplicationDbContext>(options =>
             //   options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                //Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                //Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                //User settings
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                // If the LoginPath isn't set, ASP.NET Core defaults 
                // the path to /Account/Login
                options.LoginPath = "/Account/Login";
                // If the AccessDeniedPath isn't set, ASP.NET Core defaults 
                // the path to /Account/AccessDenied.
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc()
                    .AddSessionStateTempDataProvider();

            services.AddSession();
        }

        public async Task CreateUsersAndRoles(IServiceScope serviceScope){
            var userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
            var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

            // First create the admin role
            await roleManager.CreateAsync(new IdentityRole("Admin"));

            // Then add one admin user
            var adminUser = new ApplicationUser { UserName = "admin@uia.no", Email = "admin@uia.no"};
            await userManager.CreateAsync(adminUser, "Password1.");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();

                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var db = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                    
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                    
                    // Add regular test data here
                    db.AccountList.Add(new Models.APIViewModels.AccountsModel(1, "NO", "NOK", "Hao Nhien Vu", "SHEKKITILI", "Current", 5172395, 5172395, 5172395));
                    db.AccountList.Add(new Models.APIViewModels.AccountsModel(2, "NO", "NOK", "Celine Abigael Tomren", "SHEKKITILI", "Current", 123485, 123485, 123485));
                    db.AccountList.Add(new Models.APIViewModels.AccountsModel(3, "NO", "NOK", "Lene Åsebø Berg", "SHEKKITILI", "Current", 6141552, 6141552, 6141552));

                    db.SaveChanges();

                    CreateUsersAndRoles(serviceScope).Wait();
                }
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseSession();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
