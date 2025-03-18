using System.Text;
using Xunit;

namespace Scribanter.Test;

public class TestBase
{
	public TestBase()
	{
		Environment.CurrentDirectory = Path.Combine(AppContext.BaseDirectory, "../../../../");
	}

	public class NullTextWriter : TextWriter
	{
		public override Encoding Encoding => Encoding.UTF8;
		public override void Write(char value) { }
	}

	class OutputSupressor : IDisposable
	{
		private readonly TextWriter _originalConsoleOut;
		private readonly NullTextWriter _nullWriter = new();

		public OutputSupressor()
		{
			_originalConsoleOut = Console.Out;
			Console.SetOut(_nullWriter);
			Console.SetError(_nullWriter);
		}

		public void Dispose()
		{
			Console.SetOut(_originalConsoleOut);
			GC.SuppressFinalize(this);
		}
	}

	public void RunScriban(int expectedExitCode, string[] commandLineArgs)
	{
		//using (SuppressOutput())
		{
			int exitCode = Program.Main(commandLineArgs);
			Assert.Equal(expectedExitCode, exitCode);
		}
	}

	protected IDisposable SuppressOutput()
	{
		return new OutputSupressor();
	}
}