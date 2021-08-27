﻿using System.Threading.Tasks;
using Discord.Commands;

namespace Discord.Addons.Interactive.Criteria {
    public class EmptyCriterion<T> : ICriterion<T> {
        public Task<bool> JudgeAsync(SocketCommandContext sourceContext, T parameter) {
            return Task.FromResult(true);
        }
    }
}
