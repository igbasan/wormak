using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ProfileService.Data.Interfaces;
using ProfileService.Data.Mongo;
using ProfileService.Data.Redis;
using ProfileService.Logic.Implementations;
using ProfileService.Logic.Interfaces;
using ProfileService.Models.Implementations;
using ProfileService.Models.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProfileService
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
            services.AddScoped<ITokenValidator>(sp => new TokenValidator(sp.GetRequiredService<IHttpHandler>(), Configuration.GetSection("AuthenticationSection")["AuthenticationUrl"]));

            //Mongo Db Config
            services.Configure<StoreDatabaseSettings>(
                Configuration.GetSection(nameof(StoreDatabaseSettings)));

            services.AddSingleton<IStoreDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<StoreDatabaseSettings>>().Value);

            services.AddSingleton<IRedisConnection>(new RedisConnection(Configuration.GetSection("RedisDatabaseSettings")["ConnectionString"]));

            //DAO
            services.AddTransient<IGeneralUserDAO>(sp => new GeneralUserRDAO(new GeneralUserDAO(sp.GetRequiredService<IStoreDatabaseSettings>()), sp.GetRequiredService<IRedisConnection>()));
            services.AddTransient<ICompanyDAO>(sp => new CompanyRDAO(new CompanyDAO(sp.GetRequiredService<IStoreDatabaseSettings>()), sp.GetRequiredService<IRedisConnection>()));
            services.AddTransient<IProfessionalDAO>(sp => new ProfessionalRDAO(new ProfessionalDAO(sp.GetRequiredService<IStoreDatabaseSettings>()), sp.GetRequiredService<IRedisConnection>()));
            services.AddTransient<ICurrentProfileDAO>(sp => new CurrentProfileRDAO(new CurrentProfileDAO(sp.GetRequiredService<IStoreDatabaseSettings>()), sp.GetRequiredService<IRedisConnection>()));

            services.AddTransient<IGeneralUserLogic, GeneralUserLogic>();
            services.AddTransient<ICompanyLogic, CompanyLogic>();
            services.AddTransient<IProfessionalLogic, ProfessionalLogic>();
            services.AddTransient<ICurrentProfileLogic, CurrentProfileLogic>();

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

            //app.UseHttpsRedirection();

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
