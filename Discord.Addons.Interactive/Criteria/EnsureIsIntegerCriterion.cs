﻿using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace Discord.Addons.Interactive.Criteria {
    public class EnsureIsIntegerCriterion : ICriterion<SocketMessage> {
        public Task<bool> JudgeAsync(SocketCommandContext sourceContext, SocketMessage parameter) {
            var ok = int.TryParse(parameter.Content, out _);
            return Task.FromResult(ok);
        }
    }
}
