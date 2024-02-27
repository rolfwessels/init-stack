using FluentAssertions;
using NUnit.Framework;

namespace InitStack.Cmd.Tests;

public class NewCommandTests
{
  [Test]
  [Category("Integration")]
  [Explicit]
  public async Task ExecuteAsync_GivenDefaultSettings_ShouldReturnReplaceFiles()
  {
    var outputFolder = Path.Combine(Path.GetTempPath(), "DefaultCommandTests");
    var newSolutionName = "YarYar";
    var newStackFolder = Path.Combine(outputFolder, newSolutionName);
    try
    {
      // arrange
      var defaultCommandTests = new NewCommand();
      // action

      var executeAsync = await defaultCommandTests.ExecuteAsync(null!, new NewCommand.Settings()
      {
        OutputFolder = outputFolder,
        TemplateSelected = "template-dotnet-core-console-app",
        NewSolutionName = newSolutionName,
        InitGit = false
      });
      // assert
      executeAsync.Should().Be(0);
      Directory.Exists(newStackFolder).Should().BeTrue();
      Directory.Exists(Path.Combine(newStackFolder, ".git")).Should().BeFalse();
      var readAllTextAsync = await File.ReadAllTextAsync(Path.Combine(newStackFolder, "readme.md"));
      readAllTextAsync.Should().Contain("[Yar yar]");
    }
    finally
    {
      if (Directory.Exists(outputFolder))
        Directory.Delete(outputFolder, true);
    }
  }
}
