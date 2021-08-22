﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Common.Configuration;
using DiscordBot.Helpers;
using DiscordBot.Services.interfaces;
using Serilog.Events;

namespace DiscordBot.Modules {
    [DontAutoLoad]
    public abstract class BaseWaitMessageEmbeddedResponseModule : BaseEmbeddedResponseModule {
        private bool _waitMessageHandled;
        private Task<IUserMessage> _waitMessageTask;


        protected BaseWaitMessageEmbeddedResponseModule(Mapper mapper, ILogService logger,
            MessageConfiguration messageConfiguration) : base(mapper, logger) {
            MessageConfiguration = messageConfiguration;
        }

        public MessageConfiguration MessageConfiguration { get; }

        [DontInject]
        protected string MessageUserDisplay { get; set; }

        [DontInject]
        public string Name { get; set; }

        private IDisposable _typingState = null;

        protected override void AfterExecute(CommandInfo command) {
            _typingState?.Dispose();
            
            if (!_waitMessageHandled) {
                DeleteWaitMessageAsync();
            }

            base.AfterExecute(command);
        }


        protected override void BeforeExecute(CommandInfo command) {
            if (Context == null) {
                return;
            }
            
            _typingState = Context.Channel.EnterTypingState();

            MessageUserDisplay = Context.User.Username;
            SocketGuildUser user = Context.Guild?.Users.SingleOrDefault(x => x.Id == Context.User.Id);
            if (user != null) {
                MessageUserDisplay = user.Nickname;
            }

            _waitMessageTask = ReplyAsync(embed: PleaseWaitEmbed());
            base.BeforeExecute(command);
        }

        private Embed PleaseWaitEmbed() {
            var builder = new EmbedBuilder() {
                Title = $"Please hang tight {MessageUserDisplay}, we're executing your command {new Emoji("\u2699")}",
                Description = $"{MessageHelper.GetRandomDescription(MessageConfiguration)}"
            };
            builder.WithFooter(Context.Message.Id.ToString());
            return builder.Build();
        }

        protected Task ModifyWaitMessageAsync(Embed embed) {
            _waitMessageHandled = true;
            return _waitMessageTask.ContinueWith((waitMessageTask) => {
                Logger.Log(
                    $"{nameof(BaseWaitMessageEmbeddedResponseModule)}, {nameof(ModifyWaitMessageAsync)}, continue at of waitmessage",
                    LogEventLevel.Debug);
                waitMessageTask.Result.ModifyAsync(x => x.Embed = new Optional<Embed>(embed));
            });
        }

        protected Task DeleteWaitMessageAsync() {
            return DeleteWaitMessageAsync(new TimeSpan(0, 0, 0));
        }

        protected Task DeleteWaitMessageAsync(TimeSpan timeout) {
            _waitMessageHandled = true;
            return _waitMessageTask.ContinueWith((waitMessageTask) => {
                Logger.Log(
                    $"{nameof(BaseWaitMessageEmbeddedResponseModule)}, {nameof(ModifyWaitMessageAsync)}, continue at of waitmessage",
                    LogEventLevel.Debug);
                if (timeout.TotalSeconds > 1) {
                    Task.Delay(timeout)
                        .ContinueWith(_ => _waitMessageTask.Result.DeleteAsync().ConfigureAwait(false))
                        .ConfigureAwait(false);
                } else {
                    _waitMessageTask.Result.DeleteAsync();
                }
            });
        }

        protected Task SendNoResultMessage(string title = "No Results!", string description = "Seems there is nothing there") {
            var builder = new EmbedBuilder()
                .AddCommonProperties()
                .WithMessageAuthorFooter(Context)
                .WithTitle(title)
                .WithDescription(description);

            return ModifyWaitMessageAsync(builder.Build());
        }
    }
}