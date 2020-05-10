﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBotFanatic.Models.Configuration;
using DiscordBotFanatic.Modules.Parameters;
using DiscordBotFanatic.TypeReaders;
using Microsoft.Extensions.Logging;

namespace DiscordBotFanatic.Services {
    public class CommandHandlingService {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private IServiceProvider _provider;
        private BotConfiguration _configuration;
        private readonly LogService _logger;

        public CommandHandlingService(IServiceProvider provider, DiscordSocketClient discord, CommandService commands,
            BotConfiguration configuration, LogService logger) {
            _discord = discord;
            _commands = commands;
            _provider = provider;
            _configuration = configuration;
            _logger = logger;

            _discord.MessageReceived += HandleCommandAsync;
            _commands.CommandExecuted += OnCommandExecutedAsync;
        }

        public async Task InitializeAsync(IServiceProvider provider) {
            _provider = provider;
            _commands.AddTypeReader<PeriodAndMetricOsrsArguments>(new PeriodAndMetricOsrsTypeReader());
            _commands.AddTypeReader<PeriodOsrsArguments>(new PeriodOsrsTypeReader());
            _commands.AddTypeReader<MetricOsrsArguments>(new MetricOsrsTypeReader());
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }

        public async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // We have access to the information of the command executed,
            // the context of the command, and the result returned from the
            // execution in this event.

            // We can tell the user what went wrong
            if (!string.IsNullOrEmpty(result?.ErrorReason))
            {
                CreateErrorMessage(context, result);
            }

            // ...or even log the result (the method used should fit into
            // your existing log handler)
            var commandName = command.IsSpecified ? command.Value.Name : "A command";
            _logger.LogDebug(new LogMessage(LogSeverity.Info, "CommandExecution", $"{commandName} was executed at {DateTime.UtcNow}."));
        }


        private async Task HandleCommandAsync(SocketMessage messageParam) {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasStringPrefix(_configuration.CustomPrefix, ref argPos) ||
                  message.HasMentionPrefix(_discord.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            SocketCommandContext context = new SocketCommandContext(_discord, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.

            // Keep in mind that result does not indicate a return value
            // rather an object stating if the command executed successfully.
            IResult result = await _commands.ExecuteAsync(context: context, argPos: argPos, services: _provider);

            // Optionally, we may inform the user if the command fails
            // to be executed; however, this may not always be desired,
            // as it may clog up the request queue should a user spam a
            // command.
            if (result.Error.HasValue) {
               
            }
        }

        public async Task CreateErrorMessage(ICommandContext context, IResult result) {
            var resultMessageTask = GetWaitMessage(context);

            EmbedBuilder builder = new EmbedBuilder() {Title = $"Error!"};

            builder.AddField(result.Error.Value.ToString(), result.ErrorReason);
            builder.AddField($"Get more help", $"Please use `{_configuration.CustomPrefix}help` for this bot's usage");

            var resultMessage = await resultMessageTask;
            if (resultMessage != null) {
                resultMessage.ModifyAsync(x => x.Embed = builder.Build());
            }
            else {
                context.User.SendMessageAsync(embed: builder.Build());
            }
        }

        private async Task<IUserMessage> GetWaitMessage(ICommandContext context) {
            string toCompareId = context.Message.Id.ToString();
            var channel = context.Message.Channel;
            var messagesList = channel.GetMessagesAsync(context.Message, Direction.After);
            IUserMessage resultMessage = null;
            var foundMessage = new CancellationTokenSource();

            await foreach (var messages in messagesList.WithCancellation(foundMessage.Token)) {
                foreach (IMessage message in messages) {
                    if (message.Embeds.Any()) {
                        foreach (IEmbed messageEmbed in message.Embeds) {
                            if (messageEmbed.Footer?.Text == toCompareId) {
                                resultMessage = (IUserMessage) message;
                                foundMessage.Cancel();
                                break;
                            }
                        }
                    }

                    if (foundMessage.Token.IsCancellationRequested) {
                        break;
                    }
                }
            }

            return resultMessage;
        }
    }
}