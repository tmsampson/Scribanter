using System.Dynamic;
using Newtonsoft.Json;
using Scriban;
using Scriban.Runtime;

namespace Scribanter.Core;

public class JobRunner
{
	public static void Run(Job job)
	{
		// Create the job
		TemplateContext jobContext = new();
		TemplateLoader templateLoader = new();
		jobContext.TemplateLoader = templateLoader;

		// Determine root directory
		string rootDirectory = job.LoadDirectory ?? Directory.GetCurrentDirectory();

		// Process each task...
		foreach (Job.Task task in job.Tasks)
		{
			// Push task model
			if(task.ModelPath != null)
			{
				string taskModelPath = ResolvePath(task.ModelPath, rootDirectory);
				ScriptObject taskModel = LoadModel(taskModelPath);
				jobContext.PushGlobal(taskModel);
			}

			// Establish template and output directories
			string templateDirectory = string.IsNullOrEmpty(task.TemplateDirectory)? rootDirectory : ResolvePath(task.TemplateDirectory, rootDirectory);
			string outputDirectory = string.IsNullOrEmpty(task.OutputDirectory)? rootDirectory : ResolvePath(task.OutputDirectory, rootDirectory);

			// Render each item in the task
			foreach (Job.Task.Item item in task.Items)
			{
				// Load template
				string itemTemplatePath = ResolvePath(item.TemplatePath, templateDirectory);
				templateLoader.BaseDirectory = Path.GetDirectoryName(itemTemplatePath) ?? "templates";
				string templateContent = File.ReadAllText(itemTemplatePath);
				Template template = Template.Parse(templateContent);

				// Push item model
				ScriptObject itemModel = [];
				itemModel.Add("TEMPLATE_PATH", itemTemplatePath);
				itemModel.Add("TEMPLATE_FILENAME", Path.GetFileName(itemTemplatePath));
				itemModel.Add("TEMPLATE_NAME", Path.GetFileNameWithoutExtension(itemTemplatePath));
				jobContext.PushGlobal(itemModel);

				// Render template
				Console.WriteLine($"Rendering {itemTemplatePath}");
				string result = template.Render(jobContext);

				// Ensure the output directory exists
				string itemOutputPath = ResolvePath(item.OutputPath, outputDirectory);
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
	}

	private static string ResolvePath(string path, string rootDirectory)
	{
		return Path.IsPathRooted(path)? path : Path.Combine(rootDirectory, path);
	}

	private static ScriptObject LoadModel(string modelPath)
	{
		// Read and parse the JSON file
		string modelJsonText = File.ReadAllText(modelPath);
		var modelJson = JsonConvert.DeserializeObject<ExpandoObject>(modelJsonText) ?? throw new Exception("Failed to parse JSON model");

		// Load JSON data into script object
		ScriptObject model = [];
		foreach (var item in modelJson)
		{
			model.Add(item.Key, item.Value);
		}

		// Make accessible via model. prefix
		return new ScriptObject { ["model"] = model };
	}
}