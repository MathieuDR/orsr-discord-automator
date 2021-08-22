﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.Common.Models.Decorators;
using DiscordBot.Common.Models.DiscordDtos;
using DiscordBot.Data.Repository;
using DiscordBot.Services.Helpers;
using DiscordBot.Services.Services.interfaces;
using Microsoft.Extensions.Logging;
using WiseOldManConnector.Models.Output;

namespace DiscordBot.Services.Services {
    public class PlayerService : BaseService, IPlayerService {
        private readonly IDiscordService _discordService;
        private readonly IOsrsHighscoreService _osrsHighscoreService;
        private readonly IDiscordBotRepository _repository;

        public PlayerService(ILogger<PlayerService> logger, IDiscordService discordService, IOsrsHighscoreService osrsHighscoreService,
            IDiscordBotRepository repository) :
            base(logger) {
            _discordService = discordService;
            _osrsHighscoreService = osrsHighscoreService;
            _repository = repository;
        }

        public async Task<ItemDecorator<Player>> CoupleDiscordGuildUserToOsrsAccount(GuildUser user,
            string proposedOsrsName) {
            proposedOsrsName = proposedOsrsName.ToLowerInvariant();

            var discordUserPlayer = _repository.GetPlayerById(user.GuildId, user.Id) ?? new Common.Models.Data.Player(user.GuildId, user.Id);
            CheckIfPlayerIsAlreadyCoupled(user, proposedOsrsName, discordUserPlayer);

            var osrsPlayer = await _osrsHighscoreService.GetPlayersForUsername(proposedOsrsName);
            //  Check if coupled with other user by ID
            if (IsPresentById(discordUserPlayer.CoupledOsrsAccounts, osrsPlayer.Id)) {
                // Already coupled to this account
                UpdateExistingPlayerOsrsAccount(user.GuildId, discordUserPlayer, osrsPlayer);
                return osrsPlayer.Decorate();
            }

            if (IsIdCoupledInServer(user.GuildId, osrsPlayer.Id)) {
                throw new ValidationException($"User {proposedOsrsName} is already registered on this server.");
            }

            AddNewOsrsAccount(user, discordUserPlayer, osrsPlayer);
            return osrsPlayer.Decorate();
        }

        public Task<IEnumerable<ItemDecorator<Player>>> GetAllOsrsAccounts(GuildUser user) {
            var player = _repository.GetPlayerById(user.GuildId, user.Id);

            if (player == null) {
                return Task.FromResult(new List<ItemDecorator<Player>>().AsEnumerable());
            }

            var accounts = player.CoupledOsrsAccounts;
            var tasks = new List<Task>();

            for (var i = 0; i < accounts.Count; i++) {
                var index = i;
                var account = accounts[i];

                // Account is 7 days old, or doesn't have a snapshot.
                var task = _osrsHighscoreService.GetPlayerById(account.Id).ContinueWith(antecedent => {
                    var p = antecedent.Result;
                    if (p != null) {
                        accounts[index] = p;
                    }
                });

                tasks.Add(task);
            }

            if (tasks.Any()) {
                Task.WaitAll(tasks.ToArray());

                player.CoupledOsrsAccounts = accounts.ToList();
                player.DefaultPlayerUsername =
                    accounts.Where(x => x.Id == player.WiseOldManDefaultPlayerId).Select(x => x.DisplayName)
                        .FirstOrDefault() ?? player.DefaultPlayerUsername;
                _repository.UpdateOrInsertPlayerForGuild(user.GuildId, player);
            }

            return Task.FromResult(accounts.Decorate());
        }

        public Task<NameChange> RequestNameChange(int womAccountId, string newName) {
            throw new NotImplementedException("Pls implement!");
        }

        public Task<NameChange> RequestNameChange(string oldName, string newName) {
            return _osrsHighscoreService.RequestNameChange(oldName, newName);
        }

        public Task DeleteCoupledOsrsAccount(GuildUser user, int id) {
            var player = GetPlayerConfigurationOrThrowException(user);

            if (player.CoupledOsrsAccounts.Count == 1) {
                throw new Exception("Cannot delete last coupled account. Please add a new one first.");
            }

            var index = player.CoupledOsrsAccounts.FindIndex(x => x.Id == id);
            player.CoupledOsrsAccounts.RemoveAt(index);

            if (player.WiseOldManDefaultPlayerId == id) {
                SetDefaultAccount(user, player.CoupledOsrsAccounts.FirstOrDefault(), player);
            }

            _repository.UpdateOrInsertPlayerForGuild(user.GuildId, player);
            return Task.CompletedTask;
        }

        public Task<string> SetDefaultAccount(GuildUser user, Player player) {
            return SetDefaultAccount(user, player, null);
        }

        public Task<string> GetDefaultOsrsDisplayName(GuildUser user) {
            var player = _repository.GetPlayerById(user.GuildId, user.Id);
            return Task.FromResult(player?.DefaultPlayerUsername);
        }

        public Task<string> GetUserNickname(GuildUser user, out bool isOsrsAccount) {
            var player = GetPlayerConfigurationOrThrowException(user);
            isOsrsAccount = string.IsNullOrEmpty(player.Nickname);
            return isOsrsAccount ? Task.FromResult(player.DefaultPlayerUsername) : Task.FromResult(player.Nickname);
        }

        public async Task<string> SetUserName(GuildUser user, string name) {
            if (string.IsNullOrWhiteSpace(name)) {
                throw new Exception("Name must not be empty!");
            }

            var player = GetPlayerConfigurationOrThrowException(user);
            player.Nickname = name;

            await EnforceUsername(user, player);

            _repository.UpdateOrInsertPlayerForGuild(user.GuildId, player);
            return player.Nickname;
        }

        private bool IsPresentById(List<Player> players, int id) {
            return players.Any(p => p.Id == id);
        }

        private bool IsIdCoupledInServer(ulong guildId, int id) {
            return _repository.GetPlayerByOsrsAccount(guildId, id) != null;
        }

        public async Task<string> SetDefaultAccount(GuildUser discordUser, Player osrsPlayer, Common.Models.Data.Player player) {
            if (player == null) {
                player = GetPlayerConfigurationOrThrowException(discordUser);
            }

            if (player.CoupledOsrsAccounts.All(x => x.Id != osrsPlayer.Id)) {
                // Should never really happen though..
                AddNewOsrsAccount(discordUser, player, osrsPlayer);
            }

            player.DefaultPlayerUsername = osrsPlayer.DisplayName;
            player.WiseOldManDefaultPlayerId = osrsPlayer.Id;

            await EnforceUsername(discordUser, player);

            _repository.UpdateOrInsertPlayerForGuild(discordUser.GuildId, player);
            return player.DefaultPlayerUsername;
        }

        private async Task EnforceUsername(GuildUser user, Common.Models.Data.Player playerConfig) {
            if (playerConfig.EnforceNameTemplate && !string.IsNullOrWhiteSpace(playerConfig.DefaultPlayerUsername) &&
                !string.IsNullOrWhiteSpace(playerConfig.Nickname)) {
                var result = await _discordService.SetUsername(user, $"{playerConfig.DefaultPlayerUsername} ({playerConfig.Nickname})");
            }
        }

        private Common.Models.Data.Player GetPlayerConfigurationOrThrowException(GuildUser user) {
            var config = _repository.GetPlayerById(user.GuildId, user.Id);

            if (config == null) {
                throw new Exception("No configuration found for player!");
            }

            return config;
        }

        private void AddNewOsrsAccount(GuildUser discordUser, Common.Models.Data.Player player, Player osrsPlayer) {
            // New account
            player.CoupledOsrsAccounts.Add(osrsPlayer);

            if (player.CoupledOsrsAccounts.Count == 1) {
                // RISKY LOOP!
                SetDefaultAccount(discordUser, osrsPlayer, player);
            }

            _repository.UpdateOrInsertPlayerForGuild(discordUser.GuildId, player);

            var config = _repository.GetGroupConfig(discordUser.GuildId);
            if (config is not null && config.AutoAddNewAccounts) {
                _osrsHighscoreService.AddOsrsAccountToToGroup(config.WomGroupId, config.WomVerificationCode, osrsPlayer.Username);
            }
        }

        private void CheckIfPlayerIsAlreadyCoupled(GuildUser discordUser, string proposedOsrsName, Common.Models.Data.Player player) {
            if (player.CoupledOsrsAccounts.Any(p => p.Username == proposedOsrsName)) {
                throw new ValidationException($"User {proposedOsrsName} is already coupled to you.");
            }

            if (_repository.GetPlayerByOsrsAccount(discordUser.GuildId, proposedOsrsName) != null) {
                throw new ValidationException($"User {proposedOsrsName} is already registered on this server.");
            }
        }

        private void UpdateExistingPlayerOsrsAccount(ulong guildId, Common.Models.Data.Player toUpdate, Player osrsPlayer) {
            var old = toUpdate.CoupledOsrsAccounts.Find(p => p.Id == osrsPlayer.Id);
            toUpdate.CoupledOsrsAccounts.Remove(old);
            toUpdate.CoupledOsrsAccounts.Add(osrsPlayer);

            _repository.UpdateOrInsertPlayerForGuild(guildId, toUpdate);
        }
    }
}
