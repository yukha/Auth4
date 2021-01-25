using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                        IsPersistent = true,
                        IssuedUtc = DateTimeOffset.UtcNow
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
}