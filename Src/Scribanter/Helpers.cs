using System.Dynamic;
using Newtonsoft.Json;
using Scriban.Runtime;

namespace Scribanter;

public class Helpers
{
	public static ScriptObject LoadModel(string modelPath)
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