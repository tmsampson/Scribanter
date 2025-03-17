using Newtonsoft.Json;

namespace Scribanter;

class Job
{
	[JsonIgnore]
	public Guid Id { get; } = Guid.NewGuid();
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
}