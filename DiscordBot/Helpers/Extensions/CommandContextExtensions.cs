using MathieuDR.Common;

namespace DiscordBot.Helpers.Extensions;

public static class CommandContextExtensions {
    public static SocketSlashCommandDataOption GetOption(this DefaultDictionary<string, SocketSlashCommandDataOption> valueDictionary,
        string optionName) {
        return valueDictionary[optionName];
    }

    public static object GetOptionValue(this DefaultDictionary<string, SocketSlashCommandDataOption> valueDictionary, string optionName) {
        return valueDictionary.GetOption(optionName)?.Value;
    }

    public static T GetOptionValueOrDefault<T>(this DefaultDictionary<string, SocketSlashCommandDataOption> valueDictionary, string optionName,
        T @default) {
        var opt = valueDictionary[optionName];
        return opt != null ? (T)opt.Value : @default;
    }

    public static T GetOptionValue<T>(this DefaultDictionary<string, SocketSlashCommandDataOption> valueDictionary, string optionName) {
        return (T)(valueDictionary.GetOptionValue(optionName) ?? default(T));
    }
    
    public static T GetOptionValueAsEnum<T>(this DefaultDictionary<string, SocketSlashCommandDataOption> valueDictionary, string optionName) where T: Enum {
        var value = (long)valueDictionary.GetOptionValue(optionName);
        return (T)Enum.ToObject(typeof(T), value);
    }
    

    public static DefaultDictionary<string, T> ToDefaultDictionary<T>(this IReadOnlyCollection<T> options)
        where T : IApplicationCommandInteractionDataOption {
        return new DefaultDictionary<string, T>(options?.ToDictionary(x => x.Name) ?? new Dictionary<string, T>());
    }

    public static async Task<(IEnumerable<IUser> users, string[] remainingArguments)> GetUsersFromString(this string usersString,
        ApplicationCommandContext context) {
        var stringParams = usersString.ToCollectionOfParameters()
            .ToArray();

        return await stringParams.GetUsersListFromStringWithRoles(context);
    }
}
