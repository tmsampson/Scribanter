using Scribanter.Core;

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

		// Prepare and run job
		try
		{
			Job? job = (options.JobPath != null) ? Job.LoadFromFile(options.JobPath) : new Job(options.TemplatePath, options.OutputPath, options.ModelPath);
			if(job == null)
			{
				Console.Error.WriteLine("Error: Unable to load job.");
				return -1;
			}
			JobRunner.Run(job);
			return 0;
		}
		catch(Exception e)
		{
			Console.Error.WriteLine($"Error: {e.Message}");
			return -1;
		}
	}
}