using System.Linq;
using Discord;

namespace DiscordBot.Helpers.Builders {
    public static class ComponentBuilderExtensions {
        public static ComponentBuilder WithOriginalMenu(this ComponentBuilder builder, SelectMenu originalMenu, string selected = null) {
            return builder
                .WithSelectMenu(originalMenu.Placeholder, originalMenu.CustomId,
                originalMenu.Options.Select(x => 
                        new SelectMenuOptionBuilder(x.Label, x.Value, x.Description, x.Emote, selected == x.Value)).ToList());

        }
    }
}