using DiscordBot.Common.Dtos.Discord;
using DiscordBot.Common.Models.Data;
using FluentResults;
using WiseOldManConnector.Models.Output;

namespace DiscordBot.Services.Interfaces;

public interface IDiscordService {
    Task<Result> SetUsername(GuildUser user, string nickname);
    Task<Result> PrintRunescapeDataDrop(RunescapeDropData data, ulong guildId, ulong channelId);
    Task<Result<IEnumerable<Guild>>> GetAllGuilds();
    Task<Result> SendFailedEmbed(ulong channelId, string message, Guid traceId);
    Task<Result> SendWomGroupSuccessEmbed(ulong channelId, string message, int groupId, string groupName);
    Task<Result> MessageLeaderboards<T>(ulong channelId, IEnumerable<MetricTypeLeaderboard<T>> leaderboards) where T : ILeaderboardMember;
    Task<Result> TrackClanFundEvent(ulong guildId, ClanFundEvent clanFundEvent, ulong clanFundsChannelId, long clanFundsTotalFunds);
    Task<Result<ulong>> UpdateDonationMessage(ulong guildId, ulong clanFundsDonationLeaderBoardChannel, ulong clanFundsDonationLeaderBoardMessage, IEnumerable<(ulong Player, string PlayerName, long Amount)> topDonations);
}
