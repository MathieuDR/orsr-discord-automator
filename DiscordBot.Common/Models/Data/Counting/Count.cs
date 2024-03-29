namespace DiscordBot.Common.Models.Data.Counting;

public class Count {
	public Count(DiscordUserId requesterId, string requester, int additive, string reason) {
		Additive = additive;
		RequestedBy = requesterId;
		RequestedDiscordTag = requester;
		Reason = reason;
		RequestedOn = DateTimeOffset.Now;
	}

	public Count() { }
	public int Additive { get; set; }
	public DiscordUserId RequestedBy { get; set; }
	public string RequestedDiscordTag { get; set; }
	public DateTimeOffset RequestedOn { get; set; }
	public string Reason { get; set; }
}
