﻿using DiscordBot.Common.Models.Data.Base;
using WiseOldManConnector.Models.Output;

namespace DiscordBot.Common.Models.Data.Configuration;

public class AutomatedJobState : BaseGuildModel {
	public Achievement LastPrintedAchievement { get; set; }
}
