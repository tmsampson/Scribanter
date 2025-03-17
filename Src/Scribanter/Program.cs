using Newtonsoft.Json;
using Scriban;
using Scriban.Runtime;

namespace Scribanter;

class CommandLineOptions
{
	[Clap.Option("template", HelpText = "Scriban template to render", RequiredUnless = "job")]
	public string TemplatePath { get; set; } = string.Empty;
	[Clap.Option("output", HelpText = "File to output rendered template", RequiredUnless = "job")]
	public string OutputPath { get; set; } = string.Empty;
	[Clap.Option("model", HelpText = "JSON data model injected into template when rendering")]
	public string? ModelPath { get; set; } = null;
	[Clap.Option("job", HelpText = "A job file including one or more rendering tasks (overrides all other options)")]
	public string? JobPath { get; set; } = null;
}

public class Program
{
	public static int Main(string[] args)
	{
		// Parse command line arguments
		CommandLineOptions options = new();
		Clap.Parser parser = new(options);
		if (!parser.Parse(args))
		{
			parser.PrintUsage();
			return -1;
		}

		// Prepare job
		Job? job;
		if (options.JobPath != null)
		{
			// Load job from job file
			string jobJsonString = File.ReadAllText(options.JobPath);
			job = JsonConvert.DeserializeObject<Job>(jobJsonString);
		}
		else
		{
			// Setup job from options
			job = new();
			job.Tasks.Add(new()
			{
				ModelPath = options.ModelPath,
				Items =
				[
					new () { TemplatePath = options.TemplatePath, OutputPath = options.OutputPath }
				]
			});
		}
		if (job == null)
		{
			return -1;
		}

		// Setup path resolution
		string jobRootDirectory = Path.GetDirectoryName(options.JobPath) ?? Environment.CurrentDirectory;
		string ResolvePath(string path) => Path.IsPathRooted(path)? path : Path.Combine(jobRootDirectory, path);

		// Create the job
		TemplateContext jobContext = new();
		TemplateLoader templateLoader = new();
		jobContext.TemplateLoader = templateLoader;

		// Process each task...
		foreach (Job.Task task in job.Tasks)
		{
			// Push task model
			if(task.ModelPath != null)
			{
				string taskModelPath = ResolvePath(task.ModelPath);
				ScriptObject taskModel = Helpers.LoadModel(taskModelPath);
				jobContext.PushGlobal(taskModel);
			}

			// Render each item in the task
			foreach (Job.Task.Item item in task.Items)
			{
				// Load template
				string itemTemplatePath = ResolvePath(item.TemplatePath);
				templateLoader.BaseDirectory = Path.GetDirectoryName(itemTemplatePath) ?? "templates";
				string templateContent = File.ReadAllText(itemTemplatePath);
				Template template = Template.Parse(templateContent);

				// Push item model
				ScriptObject itemModel = [];
				itemModel.Add("TEMPLATE_PATH", itemTemplatePath);
				itemModel.Add("TEMPLATE_NAME", Path.GetFileName(itemTemplatePath));
				jobContext.PushGlobal(itemModel);

				// Render template
				Console.WriteLine($"Rendering {itemTemplatePath}");
				string result = template.Render(jobContext);

				// Ensure the output directory exists
				string itemOutputPath = ResolvePath(item.OutputPath);
				string outputDir = Path.GetDirectoryName(itemOutputPath) ?? "output";
				Directory.CreateDirectory(outputDir);

				// Write output
				Console.WriteLine($"Outputting {itemOutputPath}");
				File.WriteAllText(itemOutputPath, result);

				// Pop task model
				jobContext.PopGlobal();
			}

			// Pop task model
			if(task.ModelPath != null)
			{
				jobContext.PopGlobal();
			}
		}

		return 0;
	}
}