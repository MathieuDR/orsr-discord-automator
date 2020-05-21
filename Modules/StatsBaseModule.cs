﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBotFanatic.Helpers;
using DiscordBotFanatic.Models.Configuration;
using DiscordBotFanatic.Models.Enums;
using DiscordBotFanatic.Modules.DiscordCommandArguments;
using DiscordBotFanatic.Services;

namespace DiscordBotFanatic.Modules
{
    [DontAutoLoad]
    public abstract class StatsBaseModule : ModuleBase<SocketCommandContext>
    {
        protected readonly WiseOldManConsumer Client;
        protected readonly MessageConfiguration MessageConfiguration;

        private IUserMessage _waitMessage;
        private Task<IUserMessage> _waitMessageTask;

        public StatsBaseModule(WiseOldManConsumer wiseOldManClient, MessageConfiguration messageConfiguration)
        {
            Client = wiseOldManClient;
            MessageConfiguration = messageConfiguration;
        }

        [DontInject]
        protected string MessageUserDisplay { get; set; }
        
        [DontInject]
        public MetricType? CommandMetricType { get; set; }
        
        [DontInject]
        public Period? CommandPeriod { get; set; }
        
        [DontInject]
        public string OsrsUsername { get; set; }
        
        [DontInject]
        public int WiseOldManId { get; set; }

        protected IUserMessage WaitMessage
        {
            get
            {
                try
                {
                    return _waitMessage ??= _waitMessageTask.Result;
                }
                catch
                {
                    return null;
                }
            }
        }

        protected override void BeforeExecute(CommandInfo command)
        {
            if (Context == null)
            {
                return;
            }

            MessageUserDisplay = Context.User.Username;
            OsrsUsername = Client.GetUserNameFromUser(Context.User);

            SocketGuildUser user = Context.Guild?.Users.SingleOrDefault(x => x.Id == Context.User.Id);
            if (user != null)
            {
                MessageUserDisplay = user.Nickname;
            }

            _waitMessageTask = ReplyAsync(embed: PleaseWaitEmbed());
            base.BeforeExecute(command);
        }

        protected override void AfterExecute(CommandInfo command)
        {
            Embed embedResponse = GetEmbedResponse();
            if (embedResponse != null)
            {
                //throw new NullReferenceException($"No embed response found.");
                if (WaitMessage != null)
                {
                    WaitMessage.ModifyAsync(x => x.Embed = new Optional<Embed>(embedResponse));
                }
                else
                {
                    ReplyAsync(embed: embedResponse);
                }
            }

            base.AfterExecute(command);
        }


        #region helper functions

        protected void ExtractPeriodAndMetricOsrsArguments(PeriodAndMetricOsrsArguments arguments)
        {
            if (arguments == null)
            {
                return;
            }

            ExtractBaseOsrsArguments(arguments);

            CommandMetricType = arguments.MetricType ?? CommandMetricType;
            CommandPeriod = arguments.Period ?? CommandPeriod;
        }

        protected void ExtractMetricOsrsArguments(MetricOsrsArguments arguments)
        {
            if (arguments == null)
            {
                return;
            }

            ExtractBaseOsrsArguments(arguments);

            CommandMetricType = arguments.MetricType ?? CommandMetricType;
        }

        protected void ExtractBaseOsrsArguments(BaseOsrsArguments baseOsrsArguments)
        {
            if (baseOsrsArguments == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(baseOsrsArguments.Username))
            {
                OsrsUsername = baseOsrsArguments.Username;
            }
        }

        #endregion

        #region formatting

        protected abstract Embed GetEmbedResponse();

        private Embed PleaseWaitEmbed()
        {
            var builder = new EmbedBuilder()
            {
                Title = $"Please hang tight {MessageUserDisplay}, we're executing your command {new Emoji("\u2699")}",
                Description = $"{FanaticHelper.GetRandomDescription(MessageConfiguration)}{Environment.NewLine}This can actually take a while!"
            };
            builder.WithFooter(Context.Message.Id.ToString());
            return builder.Build();
        }

        protected virtual EmbedBuilder GetCommonEmbedBuilder(string area, string title, string description = null)
        {
            EmbedBuilder builder = new EmbedBuilder();
            string authorName = string.IsNullOrEmpty(area) ? "WiseOldMan" : $"WiseOldMan - {area}";
            builder.Author = new EmbedAuthorBuilder() { Name = authorName, Url = $"{GetUrl(WiseOldManId)}", IconUrl = "https://wiseoldman.net/img/logo.png" };
            builder.Title = title;
            builder.Color = Color.Purple;
            builder.Description = description;
            builder.Timestamp = DateTimeOffset.UtcNow;
            builder.Footer = new EmbedFooterBuilder() { Text = $"Requested by {MessageUserDisplay}", IconUrl = Context.User.GetAvatarUrl() };

            return builder;
        }

        protected virtual string GetUrl(int id)
        {
            return "https://wiseoldman.net";
        }

        #endregion
    }
}