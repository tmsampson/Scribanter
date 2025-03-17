using Xunit;

namespace Scribanter.Test.Tests;

public class ModelUsage : TestBase
{
	[Fact]
	public void PrintAnimals()
	{
		RunScriban(expectedExitCode: 0,
		[
			"--model", "Src/Scribanter.Test/Resources/ModelUsage/Animals.json",
			"--template", "Src/Scribanter.Test/Resources/ModelUsage/PrintAnimals.scriban",
			"--output", "Src/Scribanter.Test/Output/ModelUsage/PrintAnimals.txt",
		]);
	}
}