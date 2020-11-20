﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using DiscordBotFanatic.Models.Decorators;
using WiseOldManConnector.Models.Output;
using WiseOldManConnector.Models.WiseOldMan.Enums;

namespace DiscordBotFanatic.Services.interfaces {
    public interface IGroupService {
        public Task<ItemDecorator<Group>> SetGroupForGuild(IGuildUser guildUser, int womGroupId, string verificationCode);
        Task SetAutoAdd(IGuildUser guildUser, bool autoAdd);
        Task<Dictionary<string, string>> GetSettingsDictionary(IGuild guild);
        Task<ItemDecorator<Leaderboard>> GetGroupLeaderboard(IGuildUser guildUser);
        Task<ItemDecorator<Leaderboard>> GetGroupLeaderboard(IGuildUser guildUser, MetricType metric, Period period);
    }
}