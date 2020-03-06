using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using AuthenticationService.WebAPI.Data.Interfaces;
using AuthenticationService.WebAPI.Data.Mongo;
using AuthenticationService.WebAPI.Logic.Implementations;
using AuthenticationService.WebAPI.Logic.Interfaces;
using AuthenticationService.WebAPI.Models.Implementations;
using AuthenticationService.WebAPI.Models.Interfaces;
using System.Net.Http;
using AuthenticationService.WebAPI.Data.Redis;
using Microsoft.Extensions.Hosting;

namespace AuthenticationService
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
            services.AddControllers();
            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                })
                .AddCookie();

            //Mongo Db Config
            services.Configure<StoreDatabaseSettings>(
                Configuration.GetSection(nameof(StoreDatabaseSettings)));

            services.AddSingleton<IStoreDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<StoreDatabaseSettings>>().Value);

            //Sevice setting
            services.Configure<InternalServiceKeys>(
               Configuration.GetSection(nameof(InternalServiceKeys)));

            services.AddSingleton<IInternalServiceKeys>(sp =>
                sp.GetRequiredService<IOptions<InternalServiceKeys>>().Value);

            services.AddSingleton<IConfiguration>(sp => new EnvConfiguration(Configuration, Configuration.GetSection("DockerSecretConfig")["Path"]));
            services.AddSingleton<IRedisConnection>(new RedisConnection(Configuration.GetSection("RedisDatabaseSettings")["ConnectionString"]));
            services.AddTransient<IUserDAO>(sp => new UserRDAO(new UserDAO(sp.GetRequiredService<IStoreDatabaseSettings>()), sp.GetRequiredService<IRedisConnection>()));
            services.AddTransient<IUserSessionDAO>(sp => new UserSessionRDAO(new UserSessionDAO(sp.GetRequiredService<IStoreDatabaseSettings>()), sp.GetRequiredService<IRedisConnection>()));
            services.AddTransient<IInternalServiceSessionDAO>(sp => new InternalServiceSessionRDAO(new InternalServiceSessionDAO(sp.GetRequiredService<IStoreDatabaseSettings>()), sp.GetRequiredService<IRedisConnection>()));
            services.AddTransient<ILoginAttemptDAO, LoginAttemptDAO>();

            services.AddTransient<IUserLogic, UserLogic>();
            services.AddTransient<IUserSessionLogic, UserSessionLogic>();
            services.AddTransient<IInternalServiceSessionLogic, InternalServiceSessionLogic>();

            services.AddScoped<HttpClient, HttpClient>();
            services.AddScoped<IHttpHandler, WebAPI.Logic.Implementations.HttpClientHandler>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //else
            //{
            //    app.UseExceptionHandler("/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}

            app.UseRouting();
            app.UseAuthentication();
            //app.UseHttpsRedirection();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


    }
}
