using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Askmethat.Aspnet.JsonLocalizer.Extensions;
using lonefire.Data;
using lonefire.Hubs;
using lonefire.Models;
using lonefire.Services;
using lonefire.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace lonefire
{
    public class Startup
    {
        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("db_string.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
            Environment = env;
        }

        public static IConfigurationRoot Configuration { get; set; }
        public static IHostEnvironment Environment { get; set; }
        private static readonly ILoggerFactory ConsoleLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddFilter("Microsoft", LogLevel.Warning)
                   .AddFilter("System", LogLevel.Warning)
                   .AddFilter("lonefire", LogLevel.Debug)
                   .AddConsole();
        });

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // DB connection
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"))
                       .UseLoggerFactory(ConsoleLoggerFactory));

            services.AddSignalR();

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IFileIoHelper, FileIoHelper>();
            services.AddTransient<INotifier, Notifier>();
            services.AddTransient<SeedData>();

            services.AddTransient<IUserValidator<ApplicationUser>, LfUsernameValidator<ApplicationUser>>();
            services.AddScoped<IPasswordHasher<ApplicationUser>, LfPasswordHasher>();

            services.Configure<LfPasswordHasherOptions>(options => {
                options.HashFunction = LfHashFunction.Argon2;
            });

            // Add Identity
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 2;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.RequireUniqueEmail = false;
            });

            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddLogging();

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true; //Prevent JS from accessing cookies
                options.ExpireTimeSpan = TimeSpan.FromDays(30);

                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddJsonLocalization(options =>
            {
                options.ResourcesPath = "Resources";
                options.UseBaseName = false;
                options.FileEncoding = Encoding.GetEncoding("ISO-8859-1");
                options.CacheDuration = TimeSpan.FromMinutes(15);
                options.SupportedCultureInfos = new HashSet<CultureInfo>()
                {
                  new CultureInfo("en-US"),
                  new CultureInfo("zh-CN")
                };
            });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("zh-CN"),
                };

                options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddControllers().AddControllersAsServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env, SeedData seed, ApplicationDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var _ = seed.SeedAdminUser();

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCookiePolicy();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRequestLocalization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("/hub");
            });
        }
    }
}
