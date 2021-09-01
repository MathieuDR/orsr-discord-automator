using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using DiscordBot.Helpers.Extensions;
using DiscordBot.Models.Contexts;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands.Interactive {
    public class PingApplicationCommand : ApplicationCommand {

        public PingApplicationCommand(ILogger<PingApplicationCommand> logger) : base("ping", "Lets see if the bot is awake", logger) { }
        
        protected override Task<SlashCommandBuilder> ExtendSlashCommandBuilder(SlashCommandBuilder builder) {
            builder.AddOption("info", ApplicationCommandOptionType.String, "Some extra information", false);
            builder.AddOption("time", ApplicationCommandOptionType.Boolean, "Print the ping time in ms", false);
            builder.AddOption("hash", ApplicationCommandOptionType.Boolean, "Show the hash of this command", false);
            return Task.FromResult(builder);
        }

        public override async Task<Result> HandleCommandAsync(ApplicationCommandContext context) {
            Logger.LogInformation("Received command!");

            var guildUser = context.GuildUser;
            var extraInfo = context.GetOptionValue<string>("info");
            var printTime = context.GetOptionValue<bool>("time");
            var printHash = context.GetOptionValue<bool>("hash");

            var builder = new StringBuilder();
            builder.AppendLine($"Hello {guildUser.DisplayName()}.");
            if (!string.IsNullOrWhiteSpace(extraInfo)) {
                builder.AppendLine(extraInfo);
            }

            if (printHash) {
                var hash = await GetCommandBuilderHash();
                builder.AppendLine($"My hash is: {hash}");
            }

            if (printTime) {
                var timeDifference = DateTimeOffset.Now - context.InnerContext.CreatedAt;
                builder.AppendLine($"Difference is: {timeDifference.TotalMilliseconds}ms");
            }
            
            await context.RespondAsync(builder.ToString());
            return Result.Ok();
        }

        public override Task<Result> HandleComponentAsync(MessageComponentContext context) {
            throw new NotImplementedException();
        }

        public override Guid Id => Guid.Parse("912DFB5E-4837-40C5-8E66-CDA3779FE823");
        public override bool GlobalRegister => true;
    }
}
