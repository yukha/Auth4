using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.StackExchangeRedis;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

namespace Auth4.WebApi
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
            services.AddCustomDataProtection(Configuration.GetSection("Redis"));


            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.Name = "ardp.Auth";
                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    };
                    options.Events.OnValidatePrincipal = context =>
                    {
                        var principal = context.Principal;
                        return Task.CompletedTask;
                    };
                });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
                options.HttpOnly = HttpOnlyPolicy.None;
                options.Secure = CookieSecurePolicy.Always;
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Auth4.WebApi", Version = "v1"});
            });
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth4.WebApi v1"));
            }

            app.Use(async (ctx, next) =>
            {
                var clientCookie = ctx.Request.Cookies["ardp.Auth"];

                await next();
            });

            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }


    public static class DataProtectionExtension
    {
        public static IServiceCollection AddCustomDataProtection(this IServiceCollection serviceCollection, 
            IConfiguration redisSection)
        {
            var builder = serviceCollection
                .AddDataProtection()
                .SetApplicationName(redisSection["ApplicationName"])
                .AddKeyManagementOptions(options =>
                {
                    options.NewKeyLifetime = new TimeSpan(365, 0, 0, 0);
                    options.AutoGenerateKeys = true;
                });

            serviceCollection
                .AddOptions<KeyManagementOptions>()
                .Configure(options =>
                {
                    options.XmlRepository =
                        new RedisXmlRepository(
                            () => ConnectionMultiplexer.Connect(redisSection["Configuration"]).GetDatabase(),
                            redisSection["DataProtectionKeyName"]);
                });
            return serviceCollection;
        }
    }
}