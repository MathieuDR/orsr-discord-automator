﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.Common.Configuration;
using DiscordBot.Helpers;

namespace DiscordBot.Modules {
    [Name("Help")]
    public class HelpModule : ModuleBase<SocketCommandContext> {
        private const string HelpSummary = "Help function";
        private const string HelpCommand = "help";

        private readonly CommandService _commandService;
        private readonly BotConfiguration _configuration;

        private readonly Dictionary<string, Type> _parametersToOutput;
        private readonly IServiceProvider _serviceProvider;

        public HelpModule(CommandService commandService, IServiceProvider serviceProvider, BotConfiguration configuration) {
            _commandService = commandService;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _parametersToOutput = new Dictionary<string, Type>();
        }

        [Command(HelpCommand)]
        [Summary(HelpSummary)]
        [Alias("?", "info")]
        public async Task Help([Remainder] string module = "") {
            var output = new EmbedBuilder();
            if (string.IsNullOrEmpty(module)) {
                output.Title = "Help";
                output.AddField("**Module help**",
                    $"Use `{_configuration.CustomPrefix} help <module>` to get help with a module.{Environment.NewLine}For example: `{_configuration.CustomPrefix} help \"player stats\"`");
                foreach (var mod in _commandService.Modules.Where(m => m.Parent == null)) {
                    AddHelp(mod, ref output);
                }

                AddPrefixHelp(output);
            } else {
                var mod = _commandService.Modules.FirstOrDefault(m => m.Name.ToLower() == module.ToLower());
                if (mod == null) {
                    await ReplyAsync("No module could be found with that name.");
                    return;
                }

                output.Title = mod.Name.Replace("Module", "");
                output.Description = $"{mod.Summary}\n" + (!string.IsNullOrEmpty(mod.Remarks) ? $"({mod.Remarks})\n" : "") +
                                     (mod.Aliases.Any(x => !string.IsNullOrEmpty(x))
                                         ? $"Prefix(es): {string.Join(", ", mod.Aliases)}\n"
                                         : "") + (mod.Submodules.Any()
                                         ? $"Submodules: {string.Join(", ", mod.Submodules.Select(m => m.Name).ToList())}\n"
                                         : "") + " ";
                AddCommands(mod, ref output);
                if (_parametersToOutput.Count > 0) {
                    AddParameterInfo(ref output);
                }
            }

            await ReplyAsync("", embed: output.Build());
        }

        public static void AddStandardParameterInfo(EmbedBuilder output, string prefix) {
            output.AddField("Parameter parsing",
                $"When you have a text parameter with a space, you need to encase the parameter with quotes.{Environment.NewLine}Example: `{prefix} get \"iron man\"`");
        }

        public void AddHelp(ModuleInfo module, ref EmbedBuilder builder) {
            foreach (var sub in module.Submodules) {
                AddHelp(sub, ref builder);
            }

            builder.AddField(f => {
                f.Name = $"Module: **{module.Name}**";
                f.Value = $"Commands: {string.Join(", ", module.Commands.Select(x => $"`{x.Name}`"))}";
            });
        }

        public void AddCommands(ModuleInfo module, ref EmbedBuilder builder) {
            foreach (var command in module.Commands) {
                var t = command.CheckPreconditionsAsync(Context, _serviceProvider).GetAwaiter().GetResult();
                if (t.IsSuccess) {
                    AddCommand(command, ref builder);
                }
            }
        }

        public void AddCommand(CommandInfo command, ref EmbedBuilder builder) {
            builder.AddField(f => {
                f.Name = $"**{command.Name}**";
                f.Value = $"{command.Summary}\n" + (!string.IsNullOrEmpty(command.Remarks) ? $"({command.Remarks})\n" : "") +
                          (command.Aliases.Any()
                              ? $"Chat commands: {string.Join(", ", command.Aliases.Distinct().Select(x => $"`{x}`"))}\n"
                              : "") +
                          $"Usage: `{_configuration.CustomPrefix} {GetPrefix(command).Trim()} {GetParameters(command)}`";
            });
        }

        public string GetParameters(CommandInfo command) {
            var output = new StringBuilder();
            if (!command.Parameters.Any()) {
                return output.ToString();
            }

            foreach (var param in command.Parameters) {
                if (!_parametersToOutput.ContainsKey(param.Type.FullName ?? string.Empty)) {
                    _parametersToOutput.Add(param.Type.FullName ?? string.Empty, param.Type);
                }

                if (param.IsOptional) {
                    output.Append($"[{param.Name}:{param.Type.ToFriendlyName()}");
                    if (param.DefaultValue != null && param.Type == typeof(string) && !string.IsNullOrEmpty(param.DefaultValue.ToString())) {
                        output.Append($" = {param.DefaultValue}");
                    }

                    output.Append("] ");
                } else if (param.IsMultiple) {
                    output.Append($"|{param.Name}:{param.Type.ToFriendlyName()}| ");
                } else if (param.IsRemainder) {
                    output.Append($"...{param.Name}:{param.Type.ToFriendlyName()} ");
                } else {
                    output.Append($"<{param.Name}:{param.Type.ToFriendlyName()}> ");
                }
            }

            return output.ToString();
        }


        public string GetPrefix(CommandInfo command) {
            //var output = GetPrefix(command.Module);
            //output += $"{command.Aliases.FirstOrDefault()} ";
            return $"{command.Aliases.FirstOrDefault()} ";
        }

        public string GetPrefix(ModuleInfo module) {
            var output = "";
            if (module.Parent != null) {
                output = $"{GetPrefix(module.Parent)}{output}";
            }

            if (module.Aliases.Any()) {
                output += string.Concat(module.Aliases.FirstOrDefault(), " ");
            }

            return output;
        }

        private void AddPrefixHelp(EmbedBuilder output) {
            output.AddField("Prefixes",
                $"`{_configuration.CustomPrefix}` or a mention towards me!{Environment.NewLine}For example `{_configuration.CustomPrefix} help`");
            //output.AddField($"Parameters", $"`{_configuration.CustomPrefix}` or a mention towards me!{Environment.NewLine}For example `{_configuration.CustomPrefix} help`");
        }

        private void AddParameterInfo(ref EmbedBuilder output) {
            var builders = TypesToFriendlyDescription(_parametersToOutput).OrderBy(x => x.Length).ToList();
            var chunkSize = 1024;
            var fieldBuilder = new StringBuilder();
            var continueString = "";

            // what a nice function..
            foreach (var builder in builders) {
                // It's too big, lets output all
                if (fieldBuilder.Length + builder.Length > chunkSize) {
                    output.AddField($"Parameters {continueString}", fieldBuilder.ToString());
                    fieldBuilder = new StringBuilder();
                    continueString = "(continued)";
                }


                if (builder.Length > chunkSize) {
                    // Too big for one field, lets split it up in multiple.. god
                    var description = builder.ToString();
                    while (description.Length > chunkSize) {
                        var lastIndex = description.Substring(0, chunkSize).LastIndexOf(",", StringComparison.Ordinal);
                        if (lastIndex == -1) {
                            return; // TODO??? throw error?
                        }

                        output.AddField($"Parameters {continueString}", description.Substring(0, lastIndex));
                        continueString = "(continued)";
                        description = description.Substring(lastIndex + 2);
                    }

                    output.AddField($"Parameters {continueString}", description);
                } else {
                    // just append it, or start new one! eyy
                    fieldBuilder.Append(builder);
                    if (fieldBuilder.Length + 2 <= chunkSize) {
                        fieldBuilder.Append(Environment.NewLine + Environment.NewLine);
                    }
                }
            }

            // output the leftovers!
            if (fieldBuilder.Length > 0) {
                output.AddField($"Parameters {continueString}", fieldBuilder.ToString());
            }
        }

        private List<StringBuilder> TypesToFriendlyDescription(Dictionary<string, Type> types) {
            var result = new List<StringBuilder>();
            var extraTypes = new Dictionary<string, Type>();
            foreach (var keyValuePair in types) {
                var builder = new StringBuilder();
                var type = keyValuePair.Value;

                if (type == null) {
                    continue;
                }

                if (type == typeof(string)) {
                    continue;
                }

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                    type = Nullable.GetUnderlyingType(type);
                }

                Debug.Assert(type != null, nameof(type) + " != null");
                if (type.IsValueType && !type.IsEnum) {
                    continue;
                }

                var properties = type.GetProperties().ToList();

                builder.Append($"**{type.ToFriendlyName(true)}** {Environment.NewLine}");

                if (type.IsEnum) {
                    var values = new List<string>();
                    foreach (var x in type.GetFields()) {
                        if (!x.Name.Equals("value__")) {
                            values.Add(Enum.Parse(type, x.Name).ToString());
                        }
                    }

                    builder.Append(string.Join(", ", values));
                } else if (TypeHelper.WhiteListedTypesToOutput().Contains(type)) {
                    builder.Append(string.Join(", ", properties.Select(x => $"**{x.Name}** {x.PropertyType.ToFriendlyName()}")));


                    var list = properties.Where(x =>
                        !types.ContainsKey(x.PropertyType.FullName ?? string.Empty) &&
                        !extraTypes.ContainsKey(x.PropertyType.FullName ?? string.Empty)).ToList();
                    var dictionary = new Dictionary<string, Type>();
                    foreach (var item in list) {
                        if (!dictionary.ContainsKey(item.PropertyType.FullName ?? string.Empty)) {
                            dictionary.Add(item.PropertyType.FullName ?? string.Empty, item.PropertyType);
                        }
                    }

                    if (dictionary.Any()) {
                        extraTypes = extraTypes.Concat(dictionary).ToDictionary(x => x.Key, x => x.Value);
                    }
                } else {
                    builder.Append(type.ToFriendlyExplenation());
                }

                result.Add(builder);
            }

            if (extraTypes.Any()) {
                result.AddRange(TypesToFriendlyDescription(extraTypes));
            }

            return result;
        }


        //private Embed CreateEmbedForCommandInfos(List<CommandInfo> commands) {
        //    EmbedBuilder embedBuilder = new EmbedBuilder();

        //    foreach (CommandInfo command in commands)
        //    {
        //        // Get the command Summary attribute information
        //        string embedFieldText = command.Summary ?? "No description available\n";
        //        embedBuilder.AddField(command.Name, embedFieldText);
        //    }

        //    return embedBuilder.Build();

        //}


        //private string BuildHelperText() {
        //    string message = "";

        //    foreach (CommandInfo command in _commandService.Commands) {
        //        message += $"{command.Name} - {command.Summary} \n";
        //        foreach (ParameterInfo commandParameter in command.Parameters) {
        //            message += $"   P: {commandParameter.Name} ({commandParameter.Summary} - {commandParameter.Type})";
        //            if (commandParameter.IsOptional) {
        //                message += " (optional)";
        //            }

        //            if (commandParameter.DefaultValue != null) {
        //                message += $" (default: {commandParameter.DefaultValue})";
        //            }

        //            message += $".\n";
        //        }

        //        message += $"\n";
        //    }

        //    return message;
        //}
    }
}