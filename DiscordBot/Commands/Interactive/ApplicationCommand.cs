using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Models.Contexts;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Commands.Interactive {
    public abstract class ApplicationCommand : IApplicationCommand {
        public ApplicationCommand(string name, string description, ILogger logger) {
            Name = name.ToLowerInvariant().Trim().Replace(" ", "-");
            Description = description;
            Logger = logger;
        }

        public string Name { get; }
        public string Description { get; }
        public virtual bool GlobalRegister => true;
        public ILogger Logger { get; }

        public async Task<SlashCommandBuilder> GetCommandBuilder() {
            Logger.LogInformation("Creating SlashCommandBuilder for {command}: {description}", Name, Description);
            var builder = new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription(Description);

            builder = await ExtendSlashCommandBuilder(builder);

            return builder;
        }

        /// <summary>
        /// Extend the builder. The Name and description is already set
        /// </summary>
        /// <param name="builder">Builder with name and description set</param>
        /// <returns>Fully build slash command builder</returns>
        protected abstract Task<SlashCommandBuilder> ExtendSlashCommandBuilder(SlashCommandBuilder builder);

        public abstract Task<Result> HandleCommandAsync(ApplicationCommandContext context);

        public abstract Task<Result> HandleComponentAsync(MessageComponentContext context);

        public virtual bool CanHandle(ApplicationCommandContext context) => string.Equals(context.InnerContext.Data.Name, Name, StringComparison.InvariantCultureIgnoreCase);
    }
}
