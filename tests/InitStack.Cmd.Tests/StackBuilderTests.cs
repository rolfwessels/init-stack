using FluentAssertions;
using InitStack.Cmd.Sources;
using NUnit.Framework;

namespace InitStack.Cmd.Tests;

public class StackBuilderTests
{
  [Test]
  public void OutputFolder_GivenFolders_ShouldSelectCorrectOutputFolder()
  {
    // arrange
    var stackSourceFile = new StackSourceFile("TemplateDotnetCoreConsoleApp",
      "C:\\template\\dotnet-core-console-app");
    var stackBuilder = new StackBuilder(stackSourceFile, Path.GetTempPath(), "YarYar");
    // action
    var stackBuilderOutputFolder = stackBuilder.OutputFolder;
    // assert
    stackBuilderOutputFolder.Should().Be(Path.GetTempPath() + "YarYar");
  }
}
