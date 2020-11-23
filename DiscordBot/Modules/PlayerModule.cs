﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Discord;
using Discord.Commands;
using DiscordBotFanatic.Helpers;
using DiscordBotFanatic.Models.Configuration;
using DiscordBotFanatic.Models.ResponseModels;
using DiscordBotFanatic.Paginator;
using DiscordBotFanatic.Services.interfaces;

namespace DiscordBotFanatic.Modules {
    [Name("Player module")]
    public class PlayerModule : BaseWaitMessageEmbeddedResponseModule {
        private readonly IPlayerService _playerService;

        public PlayerModule(Mapper mapper, ILogService logger, MessageConfiguration messageConfiguration,
            IPlayerService playerService) : base(mapper, logger, messageConfiguration) {
            _playerService = playerService;
        }

        //[Name("Set Default OSRS username")]
        //[Command("setosrs", RunMode = RunMode.Async)]
        //[Summary("Set your default OSRS name for commands, and competitions")]
        //public Task SetOsrsName(string name) {
        //    throw new NotImplementedException();
        //    //var player = await _playerService.CoupleDiscordGuildUserToOsrsAccount(GetGuildUser(), name);
        //    //var b = Context.CreateCommonWiseOldManEmbedBuilder();
        //    //b.Description = $"Successfully set the default player to {player.DisplayName}";
        //    //await ModifyWaitMessageAsync(b.Build());
        //}

        [Name("Add an OSRS account")]
        [Command("addosrs", RunMode = RunMode.Async)]
        [Summary("Add an OSRS name.")]
        [RequireContext(ContextType.Guild)]
        public async Task AddOsrsName(string name) {
            var playerDecorater = await _playerService.CoupleDiscordGuildUserToOsrsAccount(GetGuildUser(), name);

            var builder = new EmbedBuilder()
                .AddWiseOldMan(playerDecorater)
                .WithMessageAuthorFooter(Context)
                .WithDescription(
                    $"Coupled {playerDecorater.Item.DisplayName} to your discord account in the server {Context.Guild.Name}");

            await ModifyWaitMessageAsync(builder.Build());
        }

        [Name("Account cycle")]
        [Command("accounts", RunMode = RunMode.Async)]
        [Summary("Cycle through accounts ")]
        [RequireContext(ContextType.Guild)]
        public async Task CycleThroughNames() {
            var accountDecorators = (await _playerService.GetAllOsrsAccounts(GetGuildUser())).ToList();
            

            if (!accountDecorators.Any()) {
                // we want to update actually.
                _ = SendNoResultMessage(description:"No accounts coupled");
                return;
            }

            var pages = accountDecorators.Select(x => x.Item.DisplayName).ToList();

            var message = new CustomPaginatedMessage(new EmbedBuilder().AddCommonProperties().WithMessageAuthorFooter(Context)) {
                Pages = pages,
                Options = new CustomActionsPaginatedAppearanceOptions() {
                    Delete = async (toDelete, i) => await _playerService.DeleteCoupleOsrsAccountAtIndex(GetGuildUser(), i)
                }
            };
            await SendPaginatedMessageAsync(message);
        }
    }
}