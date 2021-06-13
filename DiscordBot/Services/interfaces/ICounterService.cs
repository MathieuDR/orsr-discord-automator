using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBotFanatic.Models;
using DiscordBotFanatic.Models.Data;

namespace DiscordBotFanatic.Services.interfaces {
    public interface ICounterService {
        int TotalCount(IGuildUser user);
        int Count(IGuildUser user, IGuildUser requester, int additive, string reason);
        UserCountInfo GetCountInfo(IGuildUser user);
        List<UserCountInfo> TopCounts(SocketGuild contextGuild, int quantity);
        
        Task<bool> SetChannelForCounts(IGuildUser creator, IChannel outputChannel);
        Task<bool> CreateTreshold(IGuildUser creator, int count, string name, IRole role = null);        
        Task<bool> RemoveCount(ulong guildId, int index);
        Task<IReadOnlyList<CountTreshold>> GetTresholds(ulong guildId);

        Task<ulong> GetChannelForGuild(ulong guildId);
    }
}
