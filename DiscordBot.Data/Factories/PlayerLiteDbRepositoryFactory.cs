using DiscordBot.Data.Interfaces;
using DiscordBot.Data.Repository;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Data.Factories;

internal class PlayerLiteDbRepositoryFactory : BaseLiteDbRepositoryFactory<IPlayerRepository, PlayerRepository> {
    public PlayerLiteDbRepositoryFactory(ILoggerFactory loggerFactory, LiteDbManager liteDbManager) : base(loggerFactory, liteDbManager) { }

    public override bool RequiresGuildId => true;

    public override IPlayerRepository Create(DiscordGuildId guildId) {
        return new PlayerRepository(GetLogger(), LiteDbManager.GetDatabase(guildId));
    }

    public override IRepository Create() {
        throw new NotImplementedException();
    }
}
