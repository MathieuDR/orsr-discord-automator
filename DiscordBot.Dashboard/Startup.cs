using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Configuration;
using DiscordBot.Models.Configuration;
using DiscordBot.Repository;
using DiscordBot.Repository.Migrations;
using DiscordBot.Services;
using DiscordBot.Services.interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApp.Configuration;
using WebApp.Configuration.Options;
using WiseOldManConnector.Configuration;
using WiseOldManConnector.Interfaces;

namespace WebApp {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public ApiOptions ApiOptions { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {
            ApiOptions = Configuration.GetSection("WebApp").GetSection("Api").Get<ApiOptions>();
            // services.AddRazorPages();
            // services.AddServerSideBlazor();
            services.AddMvc();
            services.AddApiVersioning(options => {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(ApiOptions.VersionMajor, ApiOptions.VersionMinor);
                options.ReportApiVersions = true;
            });
            
            services.AddVersionedApiExplorer(
                options =>
                {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    options.GroupNameFormat = "'v'VVV";
            
                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                } );

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();
            services.AddSwaggerGen(options => {
                // add a custom operation filter which sets default values
                options.OperationFilter<SwaggerDefaultValues>();
                options.SwaggerDoc(ApiOptions.Version, new OpenApiInfo {Title = ApiOptions.Description, Version = ApiOptions.Version});
            });

            services.AddDiscordBot(Configuration);
        }
        

        public static void CreateLogger() {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel
                .Debug()
                .WriteTo.RollingFile(new JsonFormatter(), "logs/web.log")
                .WriteTo.Console(LogEventLevel.Information)
                .CreateLogger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider apiVersionDescriptionProvider) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            ConfigureSwagger(app, apiVersionDescriptionProvider);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                // endpoints.MapBlazorHub();
                // endpoints.MapFallbackToPage("/_Host");
            });
            
        }

        private void ConfigureSwagger(IApplicationBuilder app, IApiVersionDescriptionProvider provider ) {
            app.UseSwagger();
            // app.UseSwagger(options => {
            //    // options.RouteTemplate = ApiOptions.JsonRoute;
            // });
            app.UseSwaggerUI(options => {
                // build a swagger endpoint for each discovered API version
                foreach ( var description in provider.ApiVersionDescriptions )
                {
                    options.SwaggerEndpoint( $"{description.GroupName}/{ApiOptions.UIEndpointSuffix}", description.GroupName.ToUpperInvariant() );
                }
                // options.SwaggerEndpoint(ApiOptions.UIEndpoint, ApiOptions.Description);
            });
        }
    }
}
