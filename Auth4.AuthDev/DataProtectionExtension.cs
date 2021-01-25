using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Auth4.AuthDev
{
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