using Common.Extensions;
using Discord.Net;
using DiscordBot.Commands.Interactive;
using DiscordBot.Configuration;
using DiscordBot.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DiscordBot.Services; 

public class InteractiveCommandHandlerService {
    private readonly DiscordSocketClient _client;
    private readonly IApplicationCommandInfoRepository _commandInfoRepository;
    private readonly ILogger<InteractiveCommandHandlerService> _logger;
    private readonly IServiceProvider _provider;
    private readonly ICommandRegistrationService _registrationService;
    private readonly IOptions<BotTeamConfiguration> _botTeamConfiguration;
    private readonly ICommandStrategy _strategy;

    public InteractiveCommandHandlerService(ILogger<InteractiveCommandHandlerService> logger,
        DiscordSocketClient client,
        ICommandStrategy strategy,
        IServiceProvider provider,
        IApplicationCommandInfoRepository commandInfoRepository,
        ICommandRegistrationService registrationService,
        IOptions<BotTeamConfiguration> botTeamConfiguration) {
        _logger = logger;
        _client = client;
        _strategy = strategy;
        _provider = provider;
        _commandInfoRepository = commandInfoRepository;
        _registrationService = registrationService;
        _botTeamConfiguration = botTeamConfiguration;

        client.InteractionCreated += OnInteraction;
    }

    private ulong GuildId => _botTeamConfiguration.Value.GuildId;

  public async Task SetupAsync() {
        if (_client.ConnectionState != ConnectionState.Connected) {
            _client.Connected += ClientOnConnected;
            return;
        }

        await Initialize();
    }

    private async Task ClientOnConnected() {
        _client.Connected -= ClientOnConnected;
        await Initialize();
    }

    public async Task Initialize() {
        await InitializeCommands();
    }

    private async Task OnInteraction(SocketInteraction arg) {
        BaseInteractiveContext ctx = arg switch {
            SocketSlashCommand socketSlashCommand => new ApplicationCommandContext(socketSlashCommand, _provider),
            SocketMessageComponent socketMessageComponent => new MessageComponentContext(socketMessageComponent, _provider),
            _ => null
        };

        _logger.LogInformation("[{ctx}] Command created", ctx);
        var result = await _strategy.HandleInteractiveCommand(ctx).ConfigureAwait(false);

        if (result.IsFailed) {
            var msg = result.CombineMessage();
            
            if (string.IsNullOrWhiteSpace(msg)) {
                msg = "Unknown error";
            }
            
            _logger.LogWarning("[{ctx}] failed: {msg}", ctx, msg);
            if (ctx is null || ctx.IsDeferred) {
                await arg.FollowupAsync(msg);
            } else {
                await arg.RespondAsync(msg);
            }
        }

        _logger.LogInformation("[{ctx}] done", ctx);
    }

    private async Task InitializeCommands() {
        var manageCommand = _provider.GetRequiredService<ManageCommandsApplicationCommandHandler>();
        await RegisterCommandForOwnersGuild(manageCommand);

        var commandInfos = _commandInfoRepository.GetAll().Value;
        await _registrationService.UpdateAllCommands(commandInfos);
    }

    private async Task RegisterCommandForOwnersGuild(IApplicationCommandHandler handler) {
        var propertiesTask = handler.GetCommandProperties();
        var commands = await _client.GetGuild(GuildId).GetApplicationCommandsAsync();

        try {
            var existing = commands.FirstOrDefault(x => x.Name == handler.Name && x.IsGlobalCommand == false);
            if (existing is not null) {
                await existing.DeleteAsync();
            }

            await _client.Rest.CreateGuildCommand(await propertiesTask, GuildId);
        } catch (ApplicationCommandException e) {
            _logger.LogWarning(e, "Cannot register command {name} in the owners guild", handler.Name);
        } catch (Exception e) {
            _logger.LogError(e, "Error when creating command");
        }
    }
}