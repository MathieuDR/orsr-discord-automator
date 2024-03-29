using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace DiscordBot.Dashboard.InputFormatters;

/// <summary>
///     This input formatter bypasses the <see cref="BodyModelBinder" /> by returning a null result, when the request has a
///     form content type.
///     When registered, both <see cref="FromBodyAttribute" /> and <see cref="FromFormAttribute" /> can be used in the same
///     method.
/// </summary>
public class BypassFormDataInputFormatter : IInputFormatter {
	public bool CanRead(InputFormatterContext context) => context.HttpContext.Request.HasFormContentType;

	public Task<InputFormatterResult> ReadAsync(InputFormatterContext context) => InputFormatterResult.SuccessAsync(null);
}
