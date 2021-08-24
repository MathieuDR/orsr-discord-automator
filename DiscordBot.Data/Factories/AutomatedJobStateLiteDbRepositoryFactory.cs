using DiscordBot.Data.Interfaces;
using DiscordBot.Data.Repository;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Data.Factories {
    public class AutomatedJobStateLiteDbRepositoryFactory : BaseLiteDbRepositoryFactory<IAutomatedJobStateRepository, AutomatedJobStateRepository> {
        public AutomatedJobStateLiteDbRepositoryFactory(ILoggerFactory loggerFactory, LiteDbManager liteDbManager) : base(loggerFactory, liteDbManager) { }

        public override IAutomatedJobStateRepository Create(ulong guildId) {
            return new AutomatedJobStateRepository(GetLogger(), LiteDbManager.GetDatabase(guildId));
        }
    }
}