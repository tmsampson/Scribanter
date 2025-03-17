using Xunit;

namespace Scribanter.Test.Tests;

public class JobFiles : TestBase
{
	[Fact]
	public void SingleTask()
	{
		RunScriban(expectedExitCode: 0,
		[
			"--job", "Src/Scribanter.Test/Resources/JobFiles/SingleTaskSingleItem.job",
		]);
	}

	[Fact]
	public void MultipleTasksSingleItem()
	{
		RunScriban(expectedExitCode: 0,
		[
			"--job", "Src/Scribanter.Test/Resources/JobFiles/MultipleTasksSingleItem.job",
		]);
	}

	[Fact]
	public void SingleTaskMultipleItems()
	{
		RunScriban(expectedExitCode: 0,
		[
				"--job", "Src/Scribanter.Test/Resources/JobFiles/SingleTaskMultipleItems.job",
		]);
	}
}