﻿using System;

namespace DiscordBotFanatic.Models.Enums {
    [Flags]
    public enum BotPermissions {
        None = 0,
        EventManager,
        CompetitionManager
    }
}