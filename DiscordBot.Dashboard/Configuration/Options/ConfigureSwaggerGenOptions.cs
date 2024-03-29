using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DiscordBot.Dashboard.Configuration.Options;

/// <summary>
///     Configures the Swagger generation options.
/// </summary>
/// <remarks>
///     This allows API versioning to define a Swagger document per API version after the
///     <see cref="IApiVersionDescriptionProvider" /> service has been resolved from the service container.
/// </remarks>
// REF: https://github.com/dotnet/aspnet-api-versioning/blob/master/samples/aspnetcore/SwaggerSample/Startup.cs
public class ConfigureSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions> {
	private readonly IApiVersionDescriptionProvider provider;

	/// <summary>
	///     Initializes a new instance of the <see cref="ConfigureSwaggerGenOptions" /> class.
	/// </summary>
	/// <param name="provider">
	///     The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger
	///     documents.
	/// </param>
	public ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider provider) => this.provider = provider;

	/// <inheritdoc />
	public void Configure(SwaggerGenOptions options) {
		// add a swagger document for each discovered API version
		// note: you might choose to skip or document deprecated API versions differently
		foreach (var description in provider.ApiVersionDescriptions) {
			options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
		}
	}

	private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description) {
		var info = new OpenApiInfo {
			Title = "Sample API",
			Version = description.ApiVersion.ToString(),
			Description = "A sample application with Swagger, Swashbuckle, and API versioning.",
			Contact = new OpenApiContact { Name = "Bill Mei", Email = "bill.mei@somewhere.com" },
			License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
		};

		if (description.IsDeprecated) {
			info.Description += " This API version has been deprecated.";
		}

		return info;
	}
}
