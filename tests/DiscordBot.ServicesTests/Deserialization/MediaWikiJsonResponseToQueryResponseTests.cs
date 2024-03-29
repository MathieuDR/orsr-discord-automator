using System.Text.Json;
using DiscordBot.Services.Models.MediaWikiApi;
using DiscordBot.ServicesTests.Resources.EmbedJsons;
using FluentAssertions;
using Xunit;

namespace DiscordBot.ServicesTests.Deserialization;

public class MediaWikiJsonResponseToQueryResponseTests {
    [Theory]
    [MemberData(nameof(WikiResponseFiles.AllFiles), MemberType = typeof(WikiResponseFiles))]
    public void CanDeserializeIntoDiscordEmbed(string pathToJson, int pages) {
        var json = File.ReadAllText(pathToJson);

        var response = JsonSerializer.Deserialize<QueryResponse>(json);

        response.Should().NotBeNull();
        response.Query.Pages.Should().HaveCount(pages);
        foreach (var page in response.Query.Pages) {
            page.Value.Revisions.Should().HaveCount(1);
            page.Value.Revisions.First().Slots.Main.Should().NotBeNull();
            page.Value.Revisions.First().Slots.Main.Content.Should().NotBeNullOrEmpty();
        }
    }
}
