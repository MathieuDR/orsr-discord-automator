using DiscordBot.Common.Models.Data.Base;

namespace DiscordBot.Common.Models.Data.ClanFunds;

public record ClanFunds : BaseGuildRecord {
	// channel with clan fund tracking
	public DiscordChannelId ChannelId { get; init; }

	// channel with donation tracking
	public DiscordChannelId DonationLeaderBoardChannel { get; init; }

	// message Id for donation leaderboard
	public DiscordMessageId DonationLeaderBoardMessage { get; init; }

	// list of fund events
	public List<ClanFundEvent> Events { get; init; } = new();

	// Calculated total funds
	public long TotalFunds => Events.Sum(x => x.Amount);

	// Calculated total donations
	public long TotalDonations => Events.Where(x => x.EventType == ClanFundEventType.Donation).Sum(x => x.Amount);

	// select ClanFundEvent with the highest donation
	public ClanFundEvent HighestDonation =>
		Events.Where(x => x.EventType == ClanFundEventType.Donation).OrderByDescending(x => x.Amount).FirstOrDefault();

	// calculate total refunds
	public long TotalRefunds => Events.Where(x => x.EventType == ClanFundEventType.Refund).Sum(x => x.Amount);

	// calculate total donation of player
	public long TotalDonationOfPlayer(DiscordUserId playerId) =>
		Events.Where(x => x.EventType == ClanFundEventType.Donation && x.PlayerId == playerId).Sum(x => x.Amount);

	// calculate total donation of multiple players
	public long TotalDonationOfPlayers(List<DiscordUserId> playerIds) =>
		Events.Where(x => x.EventType == ClanFundEventType.Donation && playerIds.Contains(x.PlayerId)).Sum(x => x.Amount);
}
