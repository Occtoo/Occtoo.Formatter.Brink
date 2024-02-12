using AutoMapper;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Occtoo.Formatter.Brink;
using Occtoo.Formatter.Brink.Mappers;
using Occtoo.Formatter.Brink.Models;
using Occtoo.Formatter.Brink.Services;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Occtoo.Formatter.Brink
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging(logger => { logger.SetMinimumLevel(LogLevel.Trace); });
            ConfigureServices(builder.Services);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(ParseSettingsFromEnvironmentVariables());
            services.AddScoped<IBrinkApiService, BrinkApiService>();
            services.AddTransient(typeof(IOcctooApiService<>), typeof(OcctooApiService<>));

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddHttpClient<IOcctooApiService<OcctooProductResponse>, OcctooApiService<OcctooProductResponse>>()
                .AddPolicyHandler(GetRetryPolicy());

            services.AddHttpClient<IBrinkApiService, BrinkApiService>()
                .AddPolicyHandler(GetRetryPolicy());
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public static AppSettings ParseSettingsFromEnvironmentVariables()
        {
            var settings = new AppSettings
            {
                AzureWebJobsStorage = Environment.GetEnvironmentVariable(nameof(AppSettings.AzureWebJobsStorage)),
                ApiUrl = Environment.GetEnvironmentVariable(nameof(AppSettings.ApiUrl)),
                StockApiUrl = Environment.GetEnvironmentVariable(nameof(AppSettings.StockApiUrl)),
                PriceApiUrl = Environment.GetEnvironmentVariable(nameof(AppSettings.PriceApiUrl)),
                OcctooTokenAuthUrl = Environment.GetEnvironmentVariable(nameof(AppSettings.OcctooTokenAuthUrl)),
                BrinkApiUrl = Environment.GetEnvironmentVariable(nameof(AppSettings.BrinkApiUrl)),
                BrinkAccessTokenClientId = Environment.GetEnvironmentVariable(nameof(AppSettings.BrinkAccessTokenClientId)),
                BrinkAccessTokenSecret = Environment.GetEnvironmentVariable(nameof(AppSettings.BrinkAccessTokenSecret)),
                BrinkTokenAuthUrl = Environment.GetEnvironmentVariable(nameof(AppSettings.BrinkTokenAuthUrl)),
                OcctooClientId = Environment.GetEnvironmentVariable(nameof(AppSettings.OcctooClientId)),
                OcctooClientSecret = Environment.GetEnvironmentVariable(nameof(AppSettings.OcctooClientSecret)),
            };
            return settings;
        }
    }
}
