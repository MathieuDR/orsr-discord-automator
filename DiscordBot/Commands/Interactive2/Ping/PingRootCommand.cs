﻿using DiscordBot.Commands.Interactive2.Base.Definition;

namespace DiscordBot.Commands.Interactive2.Ping.Definition;


public class PingRootCommandDefinition : RootCommandDefinitionBase {
    public override string Name => "ping2";
    public override string Description => "Ping command through mediatr";

    public override Guid Id => Guid.Parse("341A00F5-AB4A-451F-8FA4-639D54EE658C");

    protected override Task<SlashCommandBuilder> ExtendBaseSlashCommandBuilder(SlashCommandBuilder builder) {
        return Task.FromResult<SlashCommandBuilder>(builder);
    }
}

public class InsultSubCommandDefinition : SubCommandDefinitionBase<PingRootCommandDefinition> {
    public override string Name => "insult";
    public  override string Description => "Receive an insult";
    public string VariantOtion => "Variant";
    
    protected override Task<SlashCommandOptionBuilder> ExtendOptionCommandBuilder(SlashCommandOptionBuilder builder) {
        builder.AddOption(VariantOtion, ApplicationCommandOptionType.Boolean,"Variant of insult", false);
        return Task.FromResult(builder);
    }
}
