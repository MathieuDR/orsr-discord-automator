using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using DiscordBot.Common.Dtos.Runescape;
using DiscordBot.Configuration;
using DiscordBot.Dashboard.Binders.RouteConstraints;
using DiscordBot.Dashboard.Configuration;
using DiscordBot.Dashboard.Configuration.Options;
using DiscordBot.Dashboard.InputFormatters;
using DiscordBot.Dashboard.Models.ApiRequests.DiscordEmbed;
using DiscordBot.Dashboard.Services;
using DiscordBot.Dashboard.Transformers;
using DiscordBot.Data.Configuration;
using DiscordBot.Services.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WiseOldManConnector.Configuration;

namespace DiscordBot.Dashboard;

public static class StartupHelper {
    private static ApiOptions GetApiOptions(IConfiguration configuration) {
        return configuration.GetSection("WebApp").GetSection("Api").Get<ApiOptions>();
    }

    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration) {
        var apiOptions = GetApiOptions(configuration);
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddBlazorise(options => { });
        services.AddFontAwesomeIcons();
        services.AddBootstrap5Providers();
        services.AddBootstrap5Components();
        services.AddMvc(options => { options.InputFormatters.Add(new BypassFormDataInputFormatter()); });
        services.Configure<RouteOptions>(options =>
        {
            options.ConstraintMap.Add(UlongRouteConstraint.UlongRouteConstraintName, typeof(UlongRouteConstraint));
        });

        services.AddApiVersioning(options => {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(apiOptions.VersionMajor, apiOptions.VersionMinor);
            options.ReportApiVersions = true;
        }).AddApiExplorer(options => {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
        
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();
        services.AddSwaggerGen(options => {
            // add a custom operation filter which sets default values
            options.OperationFilter<SwaggerDefaultValues>();
            options.SwaggerDoc(apiOptions.Version, new OpenApiInfo { Title = apiOptions.Description, Version = apiOptions.Version });
        });

        services
            .AddDiscordBot(configuration, typeof(global::DiscordBot.Bot))
            .UseLiteDbRepositories(configuration)
            .AddWiseOldManApi()
            .AddDiscordBotServices()
            .ConfigureQuartz(configuration);

        services.AddTransient<IMapper<Embed, RunescapeDrop>, EmbedToRunescapeDropMapper>();
        services.AddSingleton<ICachedDiscordService, CachedCachedDiscordService>();
    }

    public static void ConfigurePipeline(IApplicationBuilder app, IWebHostEnvironment env,
        IApiVersionDescriptionProvider apiVersionDescriptionProvider, IConfiguration configuration) {
        if (env.IsDevelopment()) {
            app.UseDeveloperExceptionPage();
        } else {
            app.UseExceptionHandler("/Error");
        }

        ConfigureSwagger(app, apiVersionDescriptionProvider, GetApiOptions(configuration));

        app.UseStaticFiles();

        app.UseRouting();

        app.UseEndpoints(endpoints => {
            endpoints.MapControllers();
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
        });
    }

    private static void ConfigureSwagger(IApplicationBuilder app, IApiVersionDescriptionProvider provider, ApiOptions options) {
        app.UseSwagger();
        app.UseSwaggerUI(swaggerUiOptions => {
            // build a swagger endpoint for each discovered API version
            foreach (var description in provider.ApiVersionDescriptions) {
                swaggerUiOptions.SwaggerEndpoint($"{description.GroupName}/{options.UIEndpointSuffix}", description.GroupName.ToUpperInvariant());
            }
        });
    }
}
