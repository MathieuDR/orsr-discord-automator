using DiscordBot.Common.Models.Data.Configuration;
using DiscordBot.Common.Models.Enums;
using FluentResults;

namespace DiscordBot.Services.Interfaces;

public interface IJobService {
    Task<Result<ChannelJobConfiguration>> GetConfigurationForJobType(JobType job);
}
