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
            // // Add authentication services
            // services.AddAuthentication(options => {
            //     options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //     options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //     options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            // })
            // .AddCookie()
            // .AddOAuth("NordeaOB", options => {
            //     // Configure the Nordea OB Client ID and Client Secret
            //     options.ClientId = Configuration["NordeaOB:X-IBM-Client-Id];
            //     options.ClientSecret = Configuration["NordeaOB:X-IBM-Client-Secret"];

            //     // Set the callback path, so Nordea OB will call back to http://localhost:5000/signin-nordeaob
            //     // Also ensure that you have added the URL as an Allowed Callback URL in your Nordea OB dashboard
            //     options.CallbackPath = new PathString("/signin-nordeaob");

            //     options.AuthorizationEndpoint = $"https://api.nordeaopenbanking.com/v1/authentication/";
            //     options.TokenEndpoint = $"https://api.nordeaopenbanking.com/v1/authentication/access_token/";
            //     options.UserInformationEndpoint = $"https://api.nordeaopenbanking.com/v1/assets";

            //     // To save the tokens to the Authentication Properties we need to set this to true
            //     // See code in OnTicketReceived event below to extract the tokens and save them as Claims
            //     options.SaveTokens = true;

            //     // Set scope to openid. See https://auth0.com/docs/scopes
            //     options.Scope.Clear();
            //     options.Scope.Add("openid");
            //     options.Scope.Add("profile");

            //     options.Events = new OAuthEvents
            //     {
            //         // When creating a ticket we need to manually make the call to the User Info endpoint to retrieve the user's information,
            //         // and subsequently extract the user's ID and email adddress and store them as claims
            //         OnCreatingTicket = async context =>
            //         {
            //             // Retrieve user info
            //             var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
            //             request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
            //             request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //             var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
            //             response.EnsureSuccessStatusCode();

            //             // Extract the user info object
            //             var user = JObject.Parse(await response.Content.ReadAsStringAsync());

            //             // Add the Name Identifier claim
            //             var userId = user.Value<string>("sub");
            //             if (!string.IsNullOrEmpty(userId))
            //             {
            //                 context.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId, ClaimValueTypes.String, context.Options.ClaimsIssuer));
            //             }

            //             // Add the Name claim
            //             var email = user.Value<string>("name");
            //             if (!string.IsNullOrEmpty(email))
            //             {
            //                 context.Identity.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, email, ClaimValueTypes.String, context.Options.ClaimsIssuer));
            //             }
            //         }
            //     };
            // });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["NordeaOB:Issuer"],
                        ValidAudience = Configuration["NordeaOB:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["NordeaOB:key"]))
                    };
                });

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

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

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
