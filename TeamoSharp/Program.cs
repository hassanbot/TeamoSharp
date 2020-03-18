﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System.Globalization;
using TeamoSharp.Services;

namespace TeamoSharp
{
    class Program
    {
        public static void Main(string[] args)
        {
            var culture = new CultureInfo("en-SE");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext<Models.PlayContext>();

                    services.AddLogging(ConfigureLogging);

                    services.AddScoped<ITeamoService, TeamoService>();
                    services.AddSingleton<ITimerService, TimerService>();

                    services.AddTransient<IPlayDiscordService, PlayDiscordService>();
                    services.AddTransient<IPlayDbService, PlayDbService>();
                    services.AddSingleton<IPlayService, PlayService>();

                    var serviceProvider = services.BuildServiceProvider();
                    var bot = new Bot(serviceProvider);
                    services.AddSingleton(bot);
                });
        }

        private static void ConfigureLogging(ILoggingBuilder logging)
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            logging.AddConsole(ConfigureConsole);
            //logging.AddProvider(new Logging.DiscordProvider());
        }

        private static void ConfigureConsole(ConsoleLoggerOptions console)
        {
            console.IncludeScopes = true;
            console.TimestampFormat = "yyyy-MM-dd HH:mm ";
        }
    }
}
