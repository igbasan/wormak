using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PostService.Data.Interfaces;
using PostService.Data.Mongo;
using PostService.Data.Redis;
using PostService.Logic.Implementations;
using PostService.Logic.Interfaces;
using PostService.Models.Implementations;
using PostService.Models.Interfaces;
using PostService.Utility;

namespace PostService
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.Headers["Location"] = context.RedirectUri;
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    };
                });

            services.AddTransient<IAuthorizationHandler, TokenHandler>();
            services.AddScoped<IHttpHandler, Logic.Implementations.HttpClientHandler>();
            services.AddScoped<HttpClient, HttpClient>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //setup up token validator
            string authenticationUrl = Configuration.GetSection("ServiceUrls")["AuthenticationUrl"];
            string profileUrl = Configuration.GetSection("ServiceUrls")["ProfileUrl"];
            string interUserUrl = Configuration.GetSection("ServiceUrls")["InterUserUrl"];
            string appID = Configuration.GetSection("ApplicationSection")["AppID"];

            services.AddScoped<ITokenValidator>(sp => new TokenValidator(sp.GetRequiredService<IHttpHandler>(), authenticationUrl, profileUrl));

            //Mongo Db Config
            services.Configure<StoreDatabaseSettings>(
                Configuration.GetSection(nameof(StoreDatabaseSettings)));

            services.AddSingleton<IStoreDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<StoreDatabaseSettings>>().Value);

            services.AddSingleton<IRedisConnection>(new RedisConnection(Configuration.GetSection("RedisDatabaseSettings")["ConnectionString"]));

            //DAO
            services.AddTransient<IPostDAO>(sp => new PostRDAO(new PostDAO(sp.GetRequiredService<IStoreDatabaseSettings>()), sp.GetRequiredService<IRedisConnection>()));

            services.AddTransient<IPostLogic, PostLogic>();
            services.AddTransient<IFollowerLogic>(sp => new FollowerLogic(sp.GetRequiredService<IHttpHandler>(), sp.GetRequiredService<IAppTokenStore>(), interUserUrl));
            services.AddTransient<IProfileLogic>(sp => new ProfileLogic(sp.GetRequiredService<IHttpHandler>(), sp.GetRequiredService<IAppTokenStore>(), profileUrl));
            services.AddTransient<IAppTokenStore>(sp => new AppTokenStore(sp.GetRequiredService<IHttpHandler>(), authenticationUrl, appID));

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Token", policy =>
                    policy.Requirements.Add(new TokenRequirement()));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
