using DiscordBot.Common.Models.Data.PlayerManagement;
using DiscordBot.Data.Interfaces;
using FluentResults;
using LiteDB;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Data.Repository;

internal class PlayerRepository : BaseLiteDbRepository<Player>, IPlayerRepository {
    public PlayerRepository(ILogger<PlayerRepository> logger, LiteDatabase database) : base(logger, database) { }

    public override string CollectionName => "players";

    public Result<Player> GetByDiscordId(DiscordUserId id) {
        return Result.Ok(GetCollection()
            .FindOne(p => p.DiscordUserId == id));
    }

    public Result<Player> GetPlayerByOsrsAccount(int womId) {
        return Result.Ok(GetCollection()
            .FindOne(p => p.CoupledOsrsAccounts.Select(wom => wom.Id).Any(id => id == womId)));
    }

    public Result<Player> GetPlayerByOsrsAccount(string username) {
        return Result.Ok(GetCollection()
            .FindOne(p => p.CoupledOsrsAccounts.Select(wom => wom.Username).Any(name => name == username)));
    }
}
