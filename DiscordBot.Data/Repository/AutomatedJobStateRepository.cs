using DiscordBot.Common.Models.Data.Configuration;
using DiscordBot.Data.Interfaces;
using LiteDB;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Data.Repository;

internal class AutomatedJobStateRepository : BaseLiteDbRepository<AutomatedJobState>, IAutomatedJobStateRepository {
    public AutomatedJobStateRepository(ILogger<AutomatedJobStateRepository> logger, LiteDatabase database) : base(logger, database) { }
    public override string CollectionName => "guildJobState";
}
