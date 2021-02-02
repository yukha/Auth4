using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Auth4.AuthAzure
{
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