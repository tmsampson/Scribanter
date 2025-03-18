using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;

namespace Scribanter.Core;

internal class TemplateLoader() : ITemplateLoader
{
	public string BaseDirectory { get; set; } = string.Empty;

	public string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
	{
		return Path.Combine(BaseDirectory, templateName);
	}

	public string Load(TemplateContext context, SourceSpan callerSpan, string templatePath)
	{
		if (!File.Exists(templatePath))
		{
			throw new FileNotFoundException($"Template '{templatePath}' not found.");
		}
		return File.ReadAllText(templatePath);
	}

	public async ValueTask<string> LoadAsync(TemplateContext context, SourceSpan span, string templateName)
	{
		var filePath = GetPath(context, span, templateName);
		if (File.Exists(filePath))
		{
			return await File.ReadAllTextAsync(filePath);
		}
		else
		{
			throw new FileNotFoundException($"Template file '{templateName}' not found at path '{BaseDirectory}'");
		}
	}
}