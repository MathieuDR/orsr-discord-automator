using DiscordBot.Common.Dtos.Discord;
using DiscordBot.Common.Identities;
using DiscordBot.Common.Models.Data.ClanFunds;
using DiscordBot.Data.Interfaces;
using DiscordBot.Data.Strategies;
using DiscordBot.Services.Interfaces;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Services.Services; 

public class ClanFundsService: BaseService, IClanFundsService {
	private readonly IRepositoryStrategy _repositoryStrategy;
	private readonly IDiscordService _discordService;


	public ClanFundsService(ILogger<ClanFundsService> logger, IRepositoryStrategy repositoryStrategy, IDiscordService discordService) : base(logger) {
		_repositoryStrategy = repositoryStrategy;
		_discordService = discordService;
	}
	public Task<Result<IEnumerable<ClanFundEvent>>> GetClanFundEvents(Guild guild) {
		var repo = _repositoryStrategy.GetOrCreateRepository<IClanFundsRepository>(guild.Id);

		var single = repo.GetSingle();
		if (single.IsFailed) {
			return Task.FromResult(Result.Fail<IEnumerable<ClanFundEvent>>("Could not get the clan funds from the repository! Perhaps it does not exist?"));
		}
		
		return Task.FromResult(Result.Ok(single.Value.Events.AsEnumerable()));
	}

	public Task<Result<ClanFunds>> GetClanFund(Guild guild) {
		var repo = _repositoryStrategy.GetOrCreateRepository<IClanFundsRepository>(guild.Id);

		var single = repo.GetSingle();
		if (single.IsFailed || single.Value is null) {
			return Task.FromResult(Result.Fail<ClanFunds>("Could not get the clan funds from the repository! Perhaps it does not exist?"));
		}
		
		return Task.FromResult(Result.Ok(single.Value));
	}

	public Task<Result> AddClanFund(Guild guild, ClanFundEvent clanFundEvent) {
		if(clanFundEvent is null) {
			return Task.FromResult(Result.Fail("The clan fund event is null!"));
		}
		
		if(clanFundEvent.CreatorId == DiscordUserId.Empty) {
			return Task.FromResult(Result.Fail("The creator must be set"));
		}
		
		if(clanFundEvent.Amount == 0) {
			return Task.FromResult(Result.Fail("The amount must be set"));
		}

		if(clanFundEvent.PlayerId == DiscordUserId.Empty) {
			return Task.FromResult(Result.Fail("The player must be set"));
		}
		
		if(string.IsNullOrWhiteSpace(clanFundEvent.PlayerName)) {
			return Task.FromResult(Result.Fail("The player name must be set"));
		}
		
		// save to clanfund
		var repo = _repositoryStrategy.GetOrCreateRepository<IClanFundsRepository>(guild.Id);
		var single = repo.GetSingle();
		if (single.IsFailed  || single.Value is null) {
			return Task.FromResult(Result.Fail("Could not get the clan funds from the repository! Perhaps it does not exist?"));
		}
		
		var clanFunds = single.Value;
		clanFunds.Events.Add(clanFundEvent);
		
		var result = repo.Update(clanFunds);
		if (result.IsFailed) {
			return Task.FromResult(Result.Fail("Could not update the clan funds in the repository!"));
		}
		
		// Track the event in discord
		return TrackEvent(guild.Id, clanFundEvent, clanFunds);
	}

	private async Task<Result> TrackEvent(DiscordGuildId guildId, ClanFundEvent clanFundEvent, ClanFunds clanFunds) {
		var messageResultTask = _discordService.TrackClanFundEvent(guildId, clanFundEvent, clanFunds.ChannelId, clanFunds.TotalFunds);
		
		// if it was donation
		if (clanFundEvent.EventType != ClanFundEventType.Donation) {
			return await messageResultTask;
		}

		var updateResult = await UpdateDonations(guildId, clanFunds);
		if(updateResult.IsFailed) {
			return updateResult.ToResult();
		}
			
		var repo = _repositoryStrategy.GetOrCreateRepository<IClanFundsRepository>(guildId);
		clanFunds = clanFunds with { DonationLeaderBoardMessage = updateResult.Value };
		var repoUpdate = repo.Update(clanFunds);
			
		if(repoUpdate.IsFailed) {
			return Result.Fail("Could not update repo with message update").WithErrors(repoUpdate.Errors);
		}

		return await messageResultTask;
	}

	private async Task<Result<DiscordMessageId>> UpdateDonations(DiscordGuildId guildId, ClanFunds clanFunds) {
		// Get top 10 users from donations
		var topDonations = clanFunds.Events
			.Where(x=> x.EventType == ClanFundEventType.Donation)
			.GroupBy(x=> x.PlayerId, x=>x)
			.Select(x=> (Player: x.Key, PlayerName: x.Last().PlayerName, Amount: x.Sum( y => y.Amount)))
			.OrderByDescending(x => x.Amount)
			.Take(50);
		
		return await _discordService.UpdateDonationMessage(guildId, clanFunds.DonationLeaderBoardChannel, clanFunds.DonationLeaderBoardMessage, topDonations);
	}

	public async Task<Result> Initialize(GuildUser user, Channel trackingChannel, Channel donationChannel, long? currentFunds = null) {
		var repo = _repositoryStrategy.GetOrCreateRepository<IClanFundsRepository>(user.GuildId);

		var single = repo.GetSingle();
		if (single.IsFailed) {
			return Result.Fail("Could not get the clan funds from the repository!");
		}

		var funds = single.Value ?? new ClanFunds();

		funds = funds with {  ChannelId = trackingChannel.Id, DonationLeaderBoardChannel = donationChannel.Id };

		if (currentFunds.HasValue) {
			if (funds.TotalFunds != currentFunds.Value) {
				var diff = currentFunds.Value - funds.TotalFunds;
				funds.Events.Add(new ClanFundEvent(DiscordUserId.Empty, user.Id, "Initialization / update", user.Username, diff, ClanFundEventType.System));

				await TrackEvent(user.GuildId, funds.Events.Last(), funds);
			}
		}
		
		var result = repo.UpdateOrInsert(funds);
		return result.IsFailed ? Result.Fail("Could not update the clan funds in the repository!") : Result.Ok();
	}
}
