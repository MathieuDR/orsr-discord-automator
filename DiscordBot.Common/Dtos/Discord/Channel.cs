namespace DiscordBot.Common.Dtos.Discord;

public class Channel {
	public DiscordChannelId Id { get; set; }
	public string Name { get; set; }

	public DiscordUserId RecipientId { get; set; }
	public bool IsTextChannel { get; set; }
	public bool IsVoiceChannel { get; set; }
	public bool IsCategoryChannel { get; set; }
	public bool IsDMChannel { get; set; }
	public bool IsGuildChannel => Guild is not null;
	public Guild Guild { get; set; }
	public DiscordGuildId DiscordGuildId => Guild.Id;
	public int Order { get; set; }
	public DiscordChannelId Category { get; set; }
}
