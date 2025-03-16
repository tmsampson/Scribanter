using System.Dynamic;
using Newtonsoft.Json;
using Scriban;
using Scriban.Runtime;
using Scriban.Parsing;

namespace Scribanter;

public class Generator
{
	private readonly MyTemplateLoader _templateLoader = new();
	private readonly TemplateContext _context = new();

	public Generator()
	{
		_context.TemplateLoader = _templateLoader;
	}

	public void AddDataSource(string jsonPath)
	{
		// Read and parse the JSON file
		string jsonText = File.ReadAllText(jsonPath);
		var json = JsonConvert.DeserializeObject<ExpandoObject>(jsonText) ?? throw new Exception("Failed to parse JSON file");

		// Push JSON data into global context
		ScriptObject _scriptModel = [];
		foreach (var item in json)
		{
			_scriptModel.Add(item.Key, item.Value);
		}
		_context.PushGlobal(_scriptModel);
	}

	public void Render(string templatePath, string outputPath)
	{
		// Setup template loader
		// NOTE: This allows other templates to be loaded relative template being processed
		_templateLoader.BaseDirectory = Path.GetDirectoryName(templatePath) ?? "templates";

		// Read the template file
		string templateContent = File.ReadAllText(templatePath);
		var template = Template.Parse(templateContent);

		// Push current template name into the model
		var templateModel = new ScriptObject
		{
			{ "CURRENT_TEMPLATE_NAME", Path.GetFileName(templatePath) }
		};
		_context.PushGlobal(templateModel);

		// Ensure the output directory exists
		string outputDir = Path.GetDirectoryName(outputPath) ?? "output";
		Directory.CreateDirectory(outputDir);

		// Generate pages for each item in the JSON
		Console.WriteLine($"Generating {outputPath} from {templatePath}");
		var result = template.Render(_context);

		// Pop current template name from the model
		_context.PopGlobal();

		// Write results to file
		File.WriteAllText(outputPath, result);
	}
}

public class MyTemplateLoader() : ITemplateLoader
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

class Program
{
	static void Main(string[] args)
	{
		Generator generator = new();
		generator.AddDataSource("database.json");
		generator.Render("templates/index.scriban", "site/index.html");
	}
}