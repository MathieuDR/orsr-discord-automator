using MathieuDR.Common.Extensions;

namespace DiscordBot.Helpers.Extensions;

public static class InteractiveExtensions {
    public static SelectMenuBuilder AddOptions(this SelectMenuBuilder menu,
        IEnumerable<SelectMenuOptionBuilder> options) {
        options.ForEach(opt => menu.AddOption(opt));
        return menu;
    }

    public static Dictionary<string, object> GetOptionsWithValues(this SocketSlashCommandDataOption dataOpt) {
        return dataOpt.GetOptions().ToDictionary(x => x.Key, x => x.Value.Value);
    }

    public static T GetOptionOfValue<T>(this SocketSlashCommandDataOption dataOpt, string name) where T : class {
        return dataOpt.GetOptionsWithValues()[name].Cast<T>();
    }

    public static Dictionary<string, SocketSlashCommandDataOption> GetOptions(
        this SocketSlashCommandDataOption dataOpt) {
        return dataOpt.Options?.ToDictionary(x => x.Name) ??
               new Dictionary<string, SocketSlashCommandDataOption>();
    }

    public static ActionRowBuilder AsActionRow(this IEnumerable<IMessageComponent> components) {
        return new ActionRowBuilder().AddComponents(components);
    }

    public static ActionRowBuilder AddComponents(this ActionRowBuilder builder,
        IEnumerable<IMessageComponent> components) {
        return builder.Apply(x => components.ForEach(c => x.AddComponent(c)));
    }

    public static ComponentBuilder AddActionRow(this ComponentBuilder builder,
        Action<ActionRowBuilder> initializer) {
        return builder.AddActionRows(new ActionRowBuilder().Apply(initializer));
    }

    public static ComponentBuilder AddActionRows(this ComponentBuilder builder,
        IEnumerable<ActionRowBuilder> actionRows) {
        return builder.AddActionRows(actionRows.ToArray());
    }

    public static ComponentBuilder AddActionRows(this ComponentBuilder builder,
        params ActionRowBuilder[] actionRows) {
        builder.ActionRows ??= new List<ActionRowBuilder>();
        builder.ActionRows.AddRange(actionRows);
        return builder;
    }

    public static EmbedBuilder CreateEmbedBuilder(this SocketSlashCommand command, string content = null) {
        return new EmbedBuilder()
            .WithColor(command.User.Cast<SocketGuildUser>()?.GetHighestRole()?.Color ?? 0x7000FB)
            .WithDescription(content ?? string.Empty);
    }

    public static SocketSlashCommandDataOption GetOption(this SocketSlashCommand command, string name) {
        return command.Data.Options?.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
    }

    public static T GetValueOr<T>(this SocketSlashCommandDataOption option, object @default) where T : class {
        return (option?.Value ?? @default).Cast<T>();
    }
}
