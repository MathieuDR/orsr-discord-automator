namespace DiscordBot.Common.Models.DiscordDtos {
    public class Channel {

        public Channel() { }

        public Channel(ulong thirdParty) {
            ThirdPartyId = thirdParty;
        }
        public ulong Id { get; set; }
        public string Name { get; set; }
        private ulong? ThirdPartyId { get; set; }

        public ulong? GuildId => IsGuildChannel ? ThirdPartyId : null;
        public ulong? RecipientId => IsDMChannel ? ThirdPartyId : null;
        public bool IsTextChannel { get; set; }
        public bool IsDMChannel { get; set; }
        public bool IsGuildChannel { get; set; }
    }
}
