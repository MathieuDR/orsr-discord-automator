﻿using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBotFanatic.Models.Configuration;
using DiscordBotFanatic.Repository;
using DiscordBotFanatic.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace DiscordBotFanatic
{
    class Program
    {
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync() {
            IConfiguration config = BuildConfig();
            var services = ConfigureServices(config); // No using statement?

            DiscordSocketClient client = services.GetRequiredService<DiscordSocketClient>();

            services.GetRequiredService<LogService>();

            await ((CommandHandlingService) services.GetRequiredService(typeof(CommandHandlingService)))
                .InitializeAsync(services);

            var botConfig = config.GetSection("Bot").Get<BotConfiguration>();
            await client.LoginAsync(TokenType.Bot, botConfig.Token);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices(IConfiguration config) {
            StartupConfiguration configuration = config.GetSection("Startup").Get<StartupConfiguration>();
            BotConfiguration botConfiguration = config.GetSection("Bot").Get<BotConfiguration>();

            return new ServiceCollection()
                // Base
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                // Logging
                .AddLogging(builder => builder.AddConsole(x=>new ConsoleLoggerOptions(){LogToStandardErrorThreshold = LogLevel.Debug}))
                .AddSingleton<LogService>()
                // Extra
                .AddSingleton(config)
                .AddSingleton(botConfiguration)
                .AddSingleton(botConfiguration.Messages)
                .AddTransient<WiseOldManConsumer>()
                .AddTransient<IDiscordBotRepository>(x=> new LiteDbRepository(configuration.DatabaseFile))
                // Add additional services here...
                .BuildServiceProvider();
        }

        private IConfiguration BuildConfig()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }
    }
}
