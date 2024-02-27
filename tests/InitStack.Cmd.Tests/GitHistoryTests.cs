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
  public void GetCurrentUser_WhenCalled_ShouldReturnGitUserName()
  {
    // arrange
    var gitHistory = new GitHistory();
    // action
    var value = gitHistory.GetCurrentUser();
    // assert
    value.Should().NotBeEmpty("");
  }

  [Test]
  public void GetCurrentEmail_GivenRequestForUserName_ShouldReturnUserName()
  {
    // arrange
    var gitHistory = new GitHistory();
    // action
    var value = gitHistory.GetCurrentUserEmail();
    // assert
    value.Should().Contain("@");
  }
}
