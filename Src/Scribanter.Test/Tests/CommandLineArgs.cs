using Xunit;

namespace Scribanter.Test.Tests;

public class CommandLineArgs : TestBase
{
	[Fact]
	public void FailEmpty()
	{
		RunScriban(expectedExitCode: -1, []);
	}

	[Fact]
	public void FailMissingTemplate()
	{
		RunScriban(expectedExitCode: -1,
		[
			"--output", "output",
		]);
	}

	[Fact]
	public void FailMissingOutput()
	{
		RunScriban(expectedExitCode: -1,
		[
			"--template", "output",
		]);
	}
}