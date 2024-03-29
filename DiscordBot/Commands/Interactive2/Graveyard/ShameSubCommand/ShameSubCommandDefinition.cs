namespace DiscordBot.Commands.Interactive2.Graveyard.ShameSubCommand;

public class ShameSubCommandDefinition : SubCommandDefinitionBase<GraveyardRootDefinition> {
	public ShameSubCommandDefinition(IServiceProvider serviceProvider) : base(serviceProvider) { }
	public override string Name => "shame";
	public override string Description => "Shame somebody that died!";

	public static string ShamedOption => "shamed";
	public static string PictureOption => "picture";
	public static string LocationOption => "location";

	protected override Task<SlashCommandOptionBuilder> ExtendOptionCommandBuilder(SlashCommandOptionBuilder builder) {
		builder.AddOption(ShamedOption, ApplicationCommandOptionType.String, "Shame a user that died", true);
		builder.AddOption(LocationOption, ApplicationCommandOptionType.String, "Location of the shame", true, isAutocomplete: true);
		builder.AddOption(PictureOption, ApplicationCommandOptionType.String, "Picture url of the shame", false);
		return Task.FromResult(builder);
	}
}