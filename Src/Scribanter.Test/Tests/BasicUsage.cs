using Xunit;

namespace Scribanter.Test.Tests;

public class BasicUsage : TestBase
{
	[Fact]
	public void BasicLoop()
	{
		RunScriban(expectedExitCode: 0,
		[
			"--template", "Src/Scribanter.Test/Resources/BasicUsage/BasicLoop.scriban",
			"--output", "Src/Scribanter.Test/Output/BasicUsage/BasicLoop.txt",
		]);
	}

	[Fact]
	public void PrintTemplateParams()
	{
		RunScriban(expectedExitCode: 0,
		[
				"--template", "Src/Scribanter.Test/Resources/BasicUsage/PrintTemplateParams.scriban",
				"--output", "Src/Scribanter.Test/Output/BasicUsage/PrintTemplateParams.txt",
		]);
	}
}