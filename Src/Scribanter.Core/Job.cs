using Newtonsoft.Json;

namespace Scribanter.Core;

public class Job
{
	[JsonIgnore]
	public Guid Id { get; } = Guid.NewGuid();
	[JsonIgnore]
	public string? LoadDirectory { get; private set; } = null;
	public class Task
	{
		[JsonProperty("model")]
		public string? ModelPath { get; set; } = null;
		public class Item
		{
			[JsonProperty("template"), JsonRequired]
			public string TemplatePath { get; set; } = string.Empty;
			[JsonProperty("output"), JsonRequired]
			public string OutputPath { get; set; } = string.Empty;
			[JsonProperty("model")]
			public string? ModelPath { get; set; } = null;
		}
		[JsonProperty("items"), JsonRequired]
		public List<Item> Items { get; set; } = [];
	}
	[JsonProperty("tasks"), JsonRequired]
	public List<Task> Tasks { get; set; } = [];

	public Job(string templatePath, string outputPath, string? modelPath = null)
	{
		Tasks.Add(new()
		{
			ModelPath = modelPath,
			Items =
			[
				new() { TemplatePath = templatePath, OutputPath = outputPath }
			]
		});
	}

	// Factory methods
	public static Job? LoadFromFile(string jobFilePath)
	{
		// Load job from job file
		string jobJsonString = File.ReadAllText(jobFilePath);
		Job? job = JsonConvert.DeserializeObject<Job>(jobJsonString);
		if(job != null)
		{
			// Set root directory based on where the job file was loaded from
			string? rootDirectory = Path.GetDirectoryName(jobFilePath);
			if(rootDirectory != null)
			{
				job.LoadDirectory = rootDirectory;
			}
		}
		return job;
	}
}