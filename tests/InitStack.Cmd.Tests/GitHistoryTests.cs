using FluentAssertions;
using NUnit.Framework;

namespace InitStack.Cmd.Tests;

public class GitHistoryTests
{
  [Test]
  public void Message_GivenCalledShouldReturnRandomGitMessage_ShouldReturnMessage()
  {
    // arrange
    var gitHistory = new GitHistory();
    // action
    var message = gitHistory.RandomMessage();
    // assert
    GitHistory.Messages().Should().Contain(message);
  }

  [Test]
  [Category("Integration")]
  public void GetCurrentUser_WhenCalled_ShouldReturnGitUserName()
  {
    // arrange
    var path = Path.Combine(Path.GetTempPath(), "StackInit",
      "GitHistoryTests" + Guid.NewGuid().ToString("n").Substring(0, 4));
    Directory.CreateDirectory(path);
    File.WriteAllText(Path.Combine(path, "test.txt"), "test");
    var gitHistory = new GitHistory
    {
      OverrideEmail = "ssamepl@asdf.com",
      OverrideUser = "ssamepl"
    };
    gitHistory.GitInit(new DirectoryInfo(path));

    // action
    var name = gitHistory.GetCurrentUser();
    var email = gitHistory.GetCurrentUserEmail();
    // assert
    name.Should().NotBeEmpty("ssamepl");
    email.Should().NotBeEmpty("ssamepl@asdf.com");
  }
}
