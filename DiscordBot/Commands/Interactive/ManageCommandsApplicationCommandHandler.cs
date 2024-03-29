using DiscordBot.Common.Identities;
using DiscordBot.Common.Models.Data.Configuration;
using DiscordBot.Common.Models.Enums;
using DiscordBot.Data.Interfaces;
using DiscordBot.Data.Strategies;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Commands.Interactive;

public class ManageCommandsApplicationCommandHandler : ApplicationCommandHandler {
    private readonly IApplicationCommandInfoRepository _applicationCommandInfoRepository;
    private readonly IServiceProvider _serviceProvider;
    private readonly ICommandDefinitionProvider _commandDefinitionProvider;

    public ManageCommandsApplicationCommandHandler(ILogger<ManageCommandsApplicationCommandHandler> logger, IServiceProvider serviceProvider,
        IRepositoryStrategy repositoryStrategy, ICommandDefinitionProvider commandDefinitionProvider) : base("commands",
        "Manage commands", logger) {
        _serviceProvider = serviceProvider;
        _commandDefinitionProvider = commandDefinitionProvider;
        _applicationCommandInfoRepository = repositoryStrategy.GetOrCreateRepository<IApplicationCommandInfoRepository>();
    }

    public override AuthorizationRoles MinimumAuthorizationRole => AuthorizationRoles.BotAdmin;

    public override async Task<Result> HandleCommandAsync(ApplicationCommandContext context) {
        var embed = context.CreateEmbedBuilder("Select a command.");
        var commandMenu = GetCommandsSelectMenu().WithButton("Cancel", SubCommand("cancel"), ButtonStyle.Danger);

        await context.RespondAsync(embeds: new[] { embed.Build() }, component: commandMenu.Build(), ephemeral: true);
        return Result.Ok();
    }

    public override async Task<Result> HandleComponentAsync(MessageComponentContext context) {
        var subCommand = context.CustomSubCommandId;
        var result = subCommand switch {
            "cancel" => await HandleCancellation(context),
            "reset" => await HandleReset(context),
            "command" => await HandleCommandSubCommand(context),
            "global" => await HandleGlobalSubCommand(context),
            "guild" => await HandleGuildSubCommand(context),
            _ => Result.Fail("Could not find subcommand handler")
        };

        if (result.IsFailed) {
            // Empty message
            await context.UpdateAsync(null, null, null, null, null);
        }

        return result;
    }

    /// <summary>
    ///     Creates a guild command
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private async Task<Result> HandleGuildSubCommand(MessageComponentContext context) {
        var command = context.EmbedFields.First(x => x.Name == "Command").Value;
        var guild = GetGuild(context);

        var commandInfo = (await _applicationCommandInfoRepository.GetByCommandName(command)).Value;
        if (commandInfo is null) {
            try {
                var commandStrategy = _serviceProvider.GetRequiredService<ICommandStrategy>();
                var commandHash = await commandStrategy.GetCommandHash(command);
                commandInfo = new ApplicationCommandInfo(command) { Hash = commandHash };
            } catch {
                // ignored
            }

            // do definition
            if (commandInfo is null) {
                var definition = _commandDefinitionProvider.GetRootCommandByName(command).Value;
                var hash = await definition.GetCommandBuilderHash();
                commandInfo = new ApplicationCommandInfo(command) { Hash = hash };
            }
        }

        var list = commandInfo.RegisteredGuilds;
        string embedDescription;
        if (!commandInfo.RegisteredGuilds.Contains(guild.Id)) {
            list.Add(guild.Id);
            embedDescription = $"Creating command: {command} for guild {guild.Name} ({guild.Id})";
        } else {
            list.Remove(guild.Id);
            embedDescription = $"Removed command: {command} for guild {guild.Name} ({guild.Id})";
        }

        commandInfo = commandInfo with { RegisteredGuilds = list };
        var registrationService = _serviceProvider.GetRequiredService<ICommandRegistrationService>();
        await registrationService.UpdateCommand(commandInfo);
        _applicationCommandInfoRepository.UpdateOrInsert(commandInfo);

        var embed = context.CreateEmbedBuilder("Success!", embedDescription);

        await context.UpdateAsync(embed: embed.Build(), component: null, content: null);
        return Result.Ok();
    }

    private (string Name, DiscordGuildId Id) GetGuild(MessageComponentContext context) {
        var guilds = _serviceProvider.GetRequiredService<DiscordSocketClient>().Guilds
            .Where(x => x.Id == ulong.Parse(context.SelectedMenuOptions.First()))
            .Select(x => (x.Name, x.GetGuildId())).ToList();

        if (!guilds.Any()) {
            throw new ArgumentException("Could not find guild");
        }

        if (guilds.Count() > 1) {
            throw new ArgumentException("Found multiple guilds");
        }

        var guild = guilds.First();
        return guild;
    }

    /// <summary>
    ///     Creates a global command
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private async Task<Result> HandleGlobalSubCommand(MessageComponentContext context) {
        var command = context.EmbedFields.First(x => x.Name == "Command").Value;

        var commandInfo = (await _applicationCommandInfoRepository.GetByCommandName(command)).Value;

        if (commandInfo is null) {
            var commandStrategy = _serviceProvider.GetRequiredService<ICommandStrategy>();
            var commandHash = await commandStrategy.GetCommandHash(command);
            commandInfo = new ApplicationCommandInfo(command) { Hash = commandHash };
        }

        commandInfo = commandInfo with { IsGlobal = !commandInfo.IsGlobal };
        _applicationCommandInfoRepository.UpdateOrInsert(commandInfo);
        var registrationService = _serviceProvider.GetRequiredService<ICommandRegistrationService>();
        await registrationService.UpdateCommand(commandInfo);

        var embed = context.CreateEmbedBuilder("Success!", $"Creating global command: {command}");

        await context.UpdateAsync(embed: embed.Build(), component: null, content: null);
        return Result.Ok();
    }

    /// <summary>
    ///     Creates a guild select menu and buttons to register globally
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private async Task<Result> HandleCommandSubCommand(MessageComponentContext context) {
        var command = context.SelectedMenuOptions.First();
        var commandInfo = (await _applicationCommandInfoRepository.GetByCommandName(command)).Value ?? new ApplicationCommandInfo(command);

        var guildSelector = GetGuildsSelectMenu(commandInfo.RegisteredGuilds)
            .WithButton("Back", SubCommand("reset"), ButtonStyle.Secondary)
            .WithButton("Cancel", SubCommand("cancel"), ButtonStyle.Danger);

        if (commandInfo.IsGlobal) {
            guildSelector.WithButton("Remove global", SubCommand("global"), ButtonStyle.Danger);
        } else {
            guildSelector.WithButton("Register globally", SubCommand("global"));
        }

        var embed = context
            .CreateEmbedBuilder("Select a guild to register or unregister.")
            .AddField("Command", command)
            .WithMessageAuthorFooter(context.User);
        await context.UpdateAsync(embeds: new[] { embed.Build() }, component: guildSelector.Build());

        return Result.Ok();
    }


    /// <summary>
    ///     Resets the command to the start
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private async Task<Result> HandleReset(MessageComponentContext context) {
        var s2 = context
            .CreateEmbedBuilder("Select a command.")
            .WithMessageAuthorFooter(context.User);

        var commandMenu = GetCommandsSelectMenu()
            .WithButton("Cancel", SubCommand("cancel"), ButtonStyle.Danger);
        await context.UpdateAsync(embeds: new[] { s2.Build() }, component: commandMenu.Build());

        return Result.Ok();
    }

    /// <summary>
    ///     Cancels the command
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private async Task<Result> HandleCancellation(MessageComponentContext context) {
        await context.UpdateAsync("Command cancelled", null, null, null, null);
        return Result.Ok();
    }

    /// <summary>
    ///     Put the commands in a select list
    /// </summary>
    /// <returns></returns>
    private ComponentBuilder GetCommandsSelectMenu() {
        var strategy = _serviceProvider.GetRequiredService<ICommandStrategy>();
        var commands = strategy.GetCommandDescriptions().ToList();
        commands.AddRange(_commandDefinitionProvider.GetRootDefinitionDescriptions().Value);
        
        
        
        return new ComponentBuilder()
            .WithSelectMenu(new SelectMenuBuilder()
                .WithCustomId(SubCommand("command"))
                .WithOptions(commands.Select(c => new SelectMenuOptionBuilder()
                    .WithLabel($"{c.Name}: {c.Description}".Truncate(100))
                    .WithValue(c.Name)).ToList())
                .WithPlaceholder("Choose a command"));
    }

    /// <summary>
    ///     Gets the select menu for the guilds
    /// </summary>
    /// <param name="registeredCommands">Guildids where the command has been registered</param>
    /// <returns></returns>
    private ComponentBuilder GetGuildsSelectMenu(IEnumerable<DiscordGuildId> registeredCommands) {
        registeredCommands ??= Array.Empty<DiscordGuildId>();
        var guilds = _serviceProvider.GetRequiredService<DiscordSocketClient>().Guilds
            .Select(x => (x.Name, Id: x.GetGuildId()));

        return new ComponentBuilder()
            .WithSelectMenu(new SelectMenuBuilder()
                .WithCustomId(SubCommand("guild"))
                .WithOptions(guilds.Select(c => {
                    var label = $"{c.Name}";
                    if (registeredCommands.Contains(c.Id)) {
                        label += " (Deregister)";
                    }

                    return new SelectMenuOptionBuilder()
                        .WithLabel(label.Truncate(100))
                        .WithValue(c.Id.ToString());
                }).ToList())
                .WithPlaceholder("Choose a guild"));
    }

    protected override Task<SlashCommandBuilder> ExtendSlashCommandBuilder(SlashCommandBuilder builder) {
        return Task.FromResult(builder);
    }
}
