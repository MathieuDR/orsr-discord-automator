using DiscordBot.Common.Dtos.Discord;
using DiscordBot.Common.Identities;
using DiscordBot.Common.Models.Data;
using DiscordBot.Common.Models.Data.ClanFunds;
using DiscordBot.Common.Models.Data.Drops;
using DiscordBot.Common.Models.Enums;
using FluentResults;
using WiseOldManConnector.Models.Output;

namespace DiscordBot.Services.Interfaces;

public interface IDiscordService {
    Task<Result> SetUsername(GuildUser user, string nickname);
    Task<Result> PrintRunescapeDataDrop(RunescapeDropData data, DiscordGuildId guildId, DiscordChannelId channelId);
    Task<Result<IEnumerable<Guild>>> GetGuilds();
    Task<Result<IEnumerable<Channel>>> GetChannelsForGuild(DiscordGuildId guildId);
    Task<Result<Dictionary<Channel, IEnumerable<Channel>>>> GetNestedChannelsForGuild(DiscordGuildId guildId);
    Task<Result> SendFailedEmbed(DiscordChannelId channelId, string message, Guid traceId);
    Task<Result> SendSuccessEmbed(DiscordChannelId channelId, string message);
    Task<Result> SendWomGroupSuccessEmbed(DiscordChannelId channelId, string message, int groupId, string groupName);
    Task<Result> MessageLeaderboards<T>(DiscordChannelId channelId, IEnumerable<MetricTypeLeaderboard<T>> leaderboards) where T : ILeaderboardMember;
    Task<Result> TrackClanFundEvent(DiscordGuildId guildId, ClanFundEvent clanFundEvent, DiscordChannelId clanFundsChannelId, long clanFundsTotalFunds);
    Task<Result<DiscordMessageId>> UpdateDonationMessage(DiscordGuildId guildId, DiscordChannelId clanFundsDonationLeaderBoardChannel, DiscordMessageId clanFundsDonationLeaderBoardMessage, IEnumerable<(DiscordUserId Player, string PlayerName, long Amount)> topDonations);
    Task<Result<IEnumerable<GuildUser>>> GetUsers(DiscordGuildId guildId);
    Task<Result> AddRoles(DiscordGuildId guild, Dictionary<DiscordUserId, IEnumerable<DiscordRoleId>> userDicts);
    Task<Result> RemoveRoles(DiscordGuildId guild, Dictionary<DiscordUserId, IEnumerable<DiscordRoleId>> userDicts);
    Task<Result<DiscordMessageId>> SendConfirmationMessage(DiscordChannelId channelId, string title, string description, EmbedFieldDto[] fields, string thumbnailUrl = null);
}
