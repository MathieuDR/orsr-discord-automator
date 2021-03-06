using DiscordBot.Common.Models.Data;
using DiscordBot.Common.Models.Enums;
using FluentResults;
using WiseOldManConnector.Models.WiseOldMan.Enums;

namespace DiscordBot.Data.Interfaces; 

public interface IGraveyardRepository : ISingleRecordRepository<Graveyard> {
	public Result<List<Shame>> GetShamesForUser(ulong userId);
	public Result<Dictionary<ulong, List<Shame>>> GetShamesPerLocation(ShameLocation location, MetricType? metricTypeLocation = null);
	public Result<List<Shame>> GetShamesForUserPerLocation(ulong userId, ShameLocation location, MetricType? metricTypeLocation = null);
	public Result AddShame(ulong userId, Shame shame);
	public Result RemoveShame(ulong userId, Guid shameId);
	public Result<Shame> GetShameById(ulong userId, Guid shameId);
	Result UpdateShame(ulong shamedId, Guid shameId, Shame shame);
}
