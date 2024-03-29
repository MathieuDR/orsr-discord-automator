using DiscordBot.Data.Interfaces;
using DiscordBot.Data.Repository;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Data.Factories;

internal class UserCountInfoLiteDbRepositoryFactory : BaseLiteDbRepositoryFactory<IUserCountInfoRepository, UserCountInfoRepository> {
    public UserCountInfoLiteDbRepositoryFactory(ILoggerFactory loggerFactory, LiteDbManager liteDbManager) :
        base(loggerFactory, liteDbManager) { }

    public override bool RequiresGuildId => true;

    public override IUserCountInfoRepository Create(DiscordGuildId guildId) {
        return new UserCountInfoRepository(GetLogger(), LiteDbManager.GetDatabase(guildId));
    }

    public override IRepository Create() {
        throw new NotImplementedException();
    }
}