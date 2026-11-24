using AwesomeAssertions;
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

  [TestCase("SampleMovieApp", "Sample Movie App")]
  [TestCase("sample-movie-app", "sample movie app")]
  public void Build_RustWebTemplate_RewritesProjectFilesAndSkipsBuildArtifacts(string projectName, string expectedTitle)
  {
    var temp = Directory.CreateTempSubdirectory();
    var source = temp.CreateSubdirectory("template-rust-web-app");
    var output = temp.CreateSubdirectory("projects");
    try
    {
      var templateFiles = new Dictionary<string, (string Original, string Expected)>
      {
        ["Cargo.toml"] = ("template-rust-web-app", "sample-movie-app"),
        ["Cargo.lock"] = ("template-rust-web-app", "sample-movie-app"),
        ["main.rs"] = ("template_rust_web_app", "sample_movie_app"),
        ["client.ts"] = ("template-rust-web-app", "sample-movie-app"),
        ["App.tsx"] = ("Template Rust Web App", expectedTitle),
        ["compose.yaml"] = ("template-rust-web-app", "sample-movie-app"),
      };
      foreach (var (file, content) in templateFiles)
        File.WriteAllText(Path.Combine(source.FullName, file), content.Original);

      var target = source.CreateSubdirectory("target");
      var pnpmStore = source.CreateSubdirectory(".pnpm-store");
      var web = source.CreateSubdirectory("web");
      var dist = web.CreateSubdirectory("dist");
      File.WriteAllText(Path.Combine(target.FullName, "binary"), "generated");
      File.WriteAllText(Path.Combine(pnpmStore.FullName, "cache"), "generated");
      File.WriteAllText(Path.Combine(dist.FullName, "index.html"), "generated");

      var builder = new StackBuilder(new StackSourceFile(source.FullName), output.FullName, projectName);
      builder.Build();

      foreach (var (file, content) in templateFiles)
      {
        var actual = File.ReadAllText(Path.Combine(builder.OutputFolder, file));
        actual.Should().Be(content.Expected, "the generated {0} should use the project name", file);
      }

      Directory.Exists(Path.Combine(builder.OutputFolder, "target")).Should().BeFalse();
      Directory.Exists(Path.Combine(builder.OutputFolder, ".pnpm-store")).Should().BeFalse();
      Directory.Exists(Path.Combine(builder.OutputFolder, "web", "dist")).Should().BeFalse();
    }
    finally
    {
      temp.Delete(recursive: true);
    }
  }
}
