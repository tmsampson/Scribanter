using Xunit;

namespace Scribanter.Test.Tests;

public class TemplateIncludes : TestBase
{
	[Fact]
	public void HeaderAndFooter()
	{
		RunScriban(expectedExitCode: 0,
		[
			"--template", "Src/Scribanter.Test/Resources/TemplateIncludes/Content.scriban",
			"--output", "Src/Scribanter.Test/Output/TemplateIncludes/Content.txt",
		]);
	}
}