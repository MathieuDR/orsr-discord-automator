﻿using System;

namespace DiscordBot.Paginator {
    public class PaginatedStringWithContext<T> {
        public String StringValue { get; set; }
        public T Reference { get; set; }

        public override string ToString() {
            return StringValue;
        }
    }
}