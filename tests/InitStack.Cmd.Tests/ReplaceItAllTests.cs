using FluentAssertions;
using NUnit.Framework;

namespace InitStack.Cmd.Tests;

public class ReplaceItAllTests
{
  [Test]
  [TestCase("__NAME__", "SnakyPacky")]
  [TestCase("WonderFullPackageName", "SnakyPacky")]
  [TestCase("WONDERFULLPACKAGENAME", "SNAKYPACKY")]
  [TestCase("wonderfullpackagename", "snakypacky")]
  [TestCase("Wonder Full Package Name", "Snaky Packy")]
  [TestCase("Wonder full package name", "Snaky packy")]
  [TestCase("wonder full package name", "snaky packy")]
  [TestCase("wonder-full-package-name", "snaky-packy")]
  [TestCase("wonder_full_package_name", "snaky_packy")]
  [TestCase("wonderFullPackageName", "snakyPacky")]
  [TestCase("WonderFullPackageNames", "SnakyPackies")]
  [TestCase("class WonderFullPackageName { prop WonderFullPackageNames { return wonder_full_package_name}}",
    "class SnakyPacky { prop SnakyPackies { return snaky_packy}}")]
  public void ReplaceAll_GivenString_ShouldReplaceParts(string given, string expected)
  {
    // arrange
    var replaceItAll = new ReplaceItAll("WonderFullPackageName", "SnakyPacky");
    // action
    var replaceIt = replaceItAll.ReplaceIt(given);
    // assert
    replaceIt.Should().Be(expected, $"Send {given}");
  }

  [TestCase("""assembly: Guid("7fbabd76-b95e-4449-a05e-8fe55b076ff4")""",
    "assembly: Guid(")]
  [TestCase("ProductID=\"{e5833fa8-70c0-42a6-b37a-2025157272a5}\"", "ProductID=")]
  [TestCase("AppId = \"9b503732-c2d8-4bc4-bffa-f6da9a9a9e00\"", "AppId = \"")]
  [TestCase("AppId = \"9b503732-c2d8-4bc4-bffa-f6da9a9a9e00\"", "AppId = \"")]
  [TestCase("<ProjectGuid>{66c8fa02-e2ff-4d00-95ab-ed2744bafa42}</ProjectGuid>", "<ProjectGuid>{")]
  public void ReplaceAll_GivenDynamicStrings_ShouldReplaceParts(string given, string expected)
  {
    // arrange
    var replaceItAll = new ReplaceItAll("WonderFullPackageName", "SnakyPacky");
    // action
    var replaceIt = replaceItAll.ReplaceIt(given);
    // assert
    replaceIt.Should().StartWith(expected);
    replaceIt.Should().NotBe(given);
  }
}
