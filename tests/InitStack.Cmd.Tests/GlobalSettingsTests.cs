using AwesomeAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace InitStack.Cmd.Tests;

public class GlobalSettingsTests
{
  [Test]
  public void GitSources_GivenDefaultSettings_ShouldIncludeAllTemplates()
  {
    var settings = new GlobalSettings(new ConfigurationBuilder().Build());

    settings.GitSources.Should().BeEquivalentTo(new[]
    {
      "https://github.com/rolfwessels/template-dotnet-core-console-app.git",
      "https://github.com/rolfwessels/template-dotnet-core-graphql-app.git",
      "https://github.com/rolfwessels/template-go-agent.git",
      "https://github.com/rolfwessels/template-go-console-app.git",
      "https://github.com/rolfwessels/template-react-spa-site.git"
    });
  }
}
