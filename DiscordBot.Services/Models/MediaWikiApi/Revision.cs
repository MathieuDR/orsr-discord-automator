using System.Text.Json.Serialization;

namespace DiscordBot.Services.Models.MediaWikiApi;

public class Revision {
	[JsonPropertyName("slots")]
	public Slots Slots { get; set; }
}