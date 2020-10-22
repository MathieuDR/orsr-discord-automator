﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBotFanatic.Models.Exceptions;
using DiscordBotFanatic.Models.ResponseModels;
using DiscordBotFanatic.Paginator;
using DiscordBotFanatic.Services.interfaces;

namespace DiscordBotFanatic.Modules {
    [DontAutoLoad]
    public abstract class BaseEmbeddedResponseModule : InteractiveBase<SocketCommandContext> {
        //protected object ResponseObject;
        //protected TimeSpan DeleteTimeOut = new TimeSpan(0,1,0);


        protected BaseEmbeddedResponseModule(Mapper mapper, ILogService logger) {
            Mapper = mapper;
            Logger = logger;
        }

        public Mapper Mapper { get; }
        public ILogService Logger { get; }

        protected override void BeforeExecute(CommandInfo command) {
            if (Context == null) {
                return;
            }

            base.BeforeExecute(command);
        }


        public async Task<IUserMessage> SendPaginatedMessageAsync(PaginatedMessage pager, 
            ICriterion<SocketReaction> criterion = null)
        {
            var callback = new ImagePaginatedMessageCallback(Interactive, Context, pager, criterion);
            await callback.DisplayAsync().ConfigureAwait(false);
            return callback.Message;
        }

       
        protected PaginatedMessage ConvertToPaginatedMessage(IPageableResponse responseObject) {
            return new PaginatedMessage() {
                AlternateDescription = responseObject.AlternatedDescription,
                Pages = Mapper.Map<IEnumerable<Embed>>(responseObject.Pages),
                Options = new PaginatedAppearanceOptions() {
                    JumpDisplayOptions = JumpDisplayOptions.Never,
                    DisplayInformationIcon = false,
                },
                Author = BuildUserAsAuthor(),
                Content = "TEST",
                Color = Color.Red,
                Title = "TITLE"
            };
        }

        protected EmbedAuthorBuilder BuildUserAsAuthor() {
            return new EmbedAuthorBuilder() {
                IconUrl = Context.User.GetAvatarUrl(),
                Name = Context.User.Username
            };
        }

        protected EmbedFooterBuilder BuildUserForFooter() {
            return new EmbedFooterBuilder() {
                IconUrl = Context.User.GetAvatarUrl(),
                Text = Context.User.Username
            };
        }

        protected IGuildUser GetGuildUser() {
            return Context.User as IGuildUser ?? throw new Exception($"User is not in a server.");
        }
    }
}