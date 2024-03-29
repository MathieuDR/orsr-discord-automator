﻿using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Commands.Interactive2.Base.Definitions;

public  abstract class CommandDefinitionBase : ICommandDefinition {
	public CommandDefinitionBase(IServiceProvider serviceProvider) {
		ServiceProvider = serviceProvider;
		var loggerFactory = ServiceProvider.GetRequiredService<ILoggerFactory>();
		Logger = loggerFactory.CreateLogger(GetType());
	}

	protected ILogger Logger { get; }
	protected IServiceProvider ServiceProvider { get; }
	public abstract string Name { get; }
	public abstract string Description { get; }
	public IEnumerable<(string optionName, Type optionType)> Options { get; } = new List<(string optionName, Type optionType)>();
}