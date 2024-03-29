using DiscordBot.Data.Factories;
using DiscordBot.Data.Repository.Migrations;
using DiscordBot.Data.Strategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Data.Configuration;

public static class ConfigurationExtensions {
    public static IServiceCollection UseLiteDbRepositories(this IServiceCollection serviceCollection,
        IConfiguration configuration) {
        serviceCollection
            .AddSingleton<MigrationManager>()
            .AddSingleton<LiteDbManager>()
            .AddTransient<GuildConfigLiteDbRepositoryFactory>()
            .AddTransient<PlayerLiteDbRepositoryFactory>()
            .AddTransient<UserCountInfoLiteDbRepositoryFactory>()
            .AddTransient<AutomatedJobStateLiteDbRepositoryFactory>()
            .AddTransient<RunescapeDropDataRepositoryFactory>()
            .AddTransient<CommandInfoRepositoryFactory>()
            .AddTransient(x => x.GetRequiredService<CommandInfoRepositoryFactory>().Create())
            .AddTransient(x => x.GetRequiredService<RunescapeDropDataRepositoryFactory>().Create())
            .AddTransient<GraveyardLiteDbRepositoryFactory>()
            .AddTransient<ClanFundsLiteDbRepositoryFactory>()
            .AddTransient<ClanFundsLiteDbRepositoryFactory>()
            .AddTransient<ItemsLiteDbRepositoryFactory>()
            .AddTransient<ConfirmationLiteDbRepositoryFactory>()
            .AddTransient<ConfirmationConfigurationLiteDbRepositoryFactory>()
            .AddTransient<SelfCountConfigurationLiteDbRepositoryFactory>()
            .AddTransient<RunescapeDropperGuildConfigurationRepositoryFactory>()
            .AddSingleton<IRepositoryStrategy>(x =>
                new RepositoryStrategy(new IRepositoryFactory[] {
                    x.GetRequiredService<PlayerLiteDbRepositoryFactory>(),
                    x.GetRequiredService<GuildConfigLiteDbRepositoryFactory>(),
                    x.GetRequiredService<UserCountInfoLiteDbRepositoryFactory>(),
                    x.GetRequiredService<AutomatedJobStateLiteDbRepositoryFactory>(),
                    x.GetRequiredService<RunescapeDropDataRepositoryFactory>(),
                    x.GetRequiredService<CommandInfoRepositoryFactory>(),
                    x.GetRequiredService<GraveyardLiteDbRepositoryFactory>(),
                    x.GetRequiredService<ClanFundsLiteDbRepositoryFactory>(),
                    x.GetRequiredService<RunescapeDropperGuildConfigurationRepositoryFactory>(),
                    x.GetRequiredService<ItemsLiteDbRepositoryFactory>(),
                    x.GetRequiredService<ConfirmationLiteDbRepositoryFactory>(),
                    x.GetRequiredService<ConfirmationConfigurationLiteDbRepositoryFactory>(),
                    x.GetRequiredService<SelfCountConfigurationLiteDbRepositoryFactory>(),
                }))
            .AddOptions<LiteDbOptions>()
            .Configure<IConfiguration>((options, configuration1) => configuration.GetSection(LiteDbOptions.SectionName).Bind(options));

        return serviceCollection;
    }
}
