using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace Auth4.AuthDev
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomDataProtection();
            
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
                });
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.Use(async (ctx, next) =>
            {
                if (ctx.Request.Path == "/auth/Login")
                {
                    var claims = new List<Claim>
                    {
                        new Claim("tid", "00000000-0000-0000-0000-000000000001"),
                        new Claim("sub", "00000000-0000-0000-0000-000000000002"),
                        new Claim("username", "Local Developer"),
                        new Claim("roles", "Login User Developer"),
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        // Refreshing the authentication session should be allowed.

                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                        // The time at which the authentication ticket expires. A 
                        // value set here overrides the ExpireTimeSpan option of 
                        // CookieAuthenticationOptions set with AddCookie.

                        IsPersistent = true,
                        // Whether the authentication session is persisted across 
                        // multiple requests. When used with cookies, controls
                        // whether the cookie's lifetime is absolute (matching the
                        // lifetime of the authentication ticket) or session-based.

                        IssuedUtc = DateTimeOffset.UtcNow
                        // The time at which the authentication ticket was issued.

                        //RedirectUri = <string>
                        // The full path or absolute URI to be used as an http 
                        // redirect response value.
                    };

                    await ctx.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    ctx.Response.Redirect("/sign-in");
                    return;
                }

                if (ctx.Request.Path == "/auth/Logout")
                {
                    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    ctx.Response.Redirect("/sign-out");
                    return;
                }

                ctx.Response.StatusCode = 404;
            });
        }
        
    }

    public static class DataProtectionExtension
    {
        public static IServiceCollection AddCustomDataProtection(this IServiceCollection serviceCollection)
        {
            var builder = serviceCollection
                .AddDataProtection()
                .SetApplicationName("MyApp")
                .AddKeyManagementOptions(options =>
                {
                    options.NewKeyLifetime = new TimeSpan(365, 0, 0, 0);
                    options.AutoGenerateKeys = true;
                });

            serviceCollection
                .AddOptions<KeyManagementOptions>()
                .Configure((options) =>
                {
                    options.XmlRepository =
                        new Microsoft.AspNetCore.DataProtection.StackExchangeRedis.RedisXmlRepository(
                            () => ConnectionMultiplexer.Connect("127.0.0.1:6379,password=Password1").GetDatabase(),
                            "DataProtection-Keys");
                });
            return serviceCollection;
        }
    }
}