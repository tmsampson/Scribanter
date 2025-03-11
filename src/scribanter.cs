using System.Dynamic;
using Newtonsoft.Json;
using Scriban;
using Scriban.Runtime;
using Scriban.Parsing;

namespace generator;

public class HtmlGenerator
{
	public static void GeneratePages(string jsonPath, string templatePath, string outputPath)
	{
		// Read and parse the JSON file
		string jsonText = File.ReadAllText(jsonPath);
		var json = JsonConvert.DeserializeObject<ExpandoObject>(jsonText) ?? throw new Exception("Failed to parse JSON file");

		// Read the template file
		string templateDir = Path.GetDirectoryName(templatePath) ?? "templates";
		string templateContent = File.ReadAllText(templatePath);
		var template = Template.Parse(templateContent);

		// Setup script model
		var scriptModel = new ScriptObject
		{
			{ "CURRENT_TEMPLATE_NAME", Path.GetFileName(templatePath) }
		};

		// Convert JSON into a Scriban script object
		foreach (var item in json)
		{
			scriptModel.Add(item.Key, item.Value);
		}

		// Setup template context
		var context = new TemplateContext
		{
			TemplateLoader = new DirectoryTemplateLoader(templateDir)
		};
		context.PushGlobal(scriptModel);

		// Ensure the output directory exists
		string outputDir = Path.GetDirectoryName(outputPath) ?? "output";
		Directory.CreateDirectory(outputDir);

		// Generate pages for each item in the JSON
		Console.WriteLine($"Generating {outputPath} from {templatePath} using {jsonPath}");
		var result = template.Render(context);

		// Write the rendered HTML to a file
		File.WriteAllText(outputPath, result);
	}
}

public class DirectoryTemplateLoader(string baseDirectory) : ITemplateLoader
{
	private readonly string _baseDirectory = baseDirectory;

	// Resolves the path of the template based on its name
	public string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
	{
		return Path.Combine(_baseDirectory, templateName);
	}

	// Loads the content of the template
	public string Load(TemplateContext context, SourceSpan callerSpan, string templatePath)
	{
		if (!File.Exists(templatePath))
			throw new FileNotFoundException($"Template '{templatePath}' not found.");
		return File.ReadAllText(templatePath);
	}

	// Asynchronous load method (required by ITemplateLoader)
	public async ValueTask<string> LoadAsync(TemplateContext context, SourceSpan span, string templateName)
	{
		var filePath = Path.Combine(_baseDirectory, templateName);
		if (File.Exists(filePath))
		{
			return await File.ReadAllTextAsync(filePath);
		}
		else
		{
			throw new FileNotFoundException($"Template file '{templateName}' not found at path '{_baseDirectory}'");
		}
	}
}

class Program
{
	static void Main(string[] args)
	{
		HtmlGenerator.GeneratePages("database.json", "templates/index.scriban", "site/index.html");
	}
}