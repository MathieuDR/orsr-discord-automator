using System.Drawing;

namespace DiscordBot.Common.Dtos.Discord;

public class Role : GuildEntity {
	public string Name { get; set; }
	public DiscordRoleId Id { get; set; }
	public Color Color { get; set; }
}

public class Message {
	public DiscordUserId AuthorId;
	public DiscordMessageId Id { get; set; }
	public DiscordChannelId ChannelId => Channel.Id;
	public Channel Channel { get; set; }

	public Guild Guild => Channel.Guild;
}
