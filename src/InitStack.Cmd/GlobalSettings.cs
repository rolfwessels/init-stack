using System;
using System.IO;
using Bumbershoot.Utilities;
using Microsoft.Extensions.Configuration;

namespace InitStack.Cmd;

public class GlobalSettings(IConfiguration configuration) : BaseSettings(configuration, "")
{
  private static readonly Lazy<GlobalSettings> _instance = new(() => new GlobalSettings(
    new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", true, false)
      .Build()
  ));


  public static GlobalSettings Default => _instance.Value;

  public string[] TemplateCollectionFolders => ReadConfigValue("TemplateCollectionFolders", new[] { "c:\\templates" });
  public string DefaultOutputFolder => ReadConfigValue("DefaultOutputFolder", "./output");
  public bool InitGitByDefault => ReadConfigValue("InitGitByDefault", true);

  public string[] IgnoreFiles => ReadConfigValue("IgnoreFiles", _ignore);
  public string[] ValidSourceFiles => ReadConfigValue("ValidSourceFiles", _replaceContent);


  public string[] GitSources => ReadConfigValue("GitSources",
    new[]
    {
      "https://github.com/rolfwessels/template-dotnet-core-graphql-app.git",
      "https://github.com/rolfwessels/template-dotnet-core-console-app.git",
      "https://github.com/rolfwessels/template-react-spa-site.git",
    });

  public string[] CommitMessages => ReadConfigValue("CommitMessages",
  [
    "This is where it all begins...",
    "Commit committed",
    "Version control is awful",
    "The same thing we do every night, Pinky - try to take over the world!",
    "Lock S-foils in attack position",
    "This commit is a lie",
    "I'll explain when you're older!",
    "Here be Dragons",
    "Reinventing the wheel.Again.",
    "This is not the commit message you are looking for",
    "Batman!(this commit has no parents)"
  ]);

  private readonly string[] _ignore =
  {
    Path.DirectorySeparatorChar + "build",
    Path.DirectorySeparatorChar + "bin",
    Path.DirectorySeparatorChar + "Bin",
    Path.DirectorySeparatorChar + "obj",
    Path.DirectorySeparatorChar + "_ReSharper.",
    Path.DirectorySeparatorChar + "_UpgradeReport_Files",
    Path.DirectorySeparatorChar + "Backup",
    Path.DirectorySeparatorChar + "node_modules",
    ".cache",
    ".user",
    ".suo",
    "UpgradeLog.XML",
    ".hg",
    @".git"
  };

  private readonly string[] _replaceContent =
  {
    @".bat",
    @".cmd",
    @".build",
    @".config",
    @".ps1",
    @".sln",
    @".json",
    @".cs",
    @".js",
    @"Makefile",
    @"Dockerfile",
    @".yml",
    @".md",
    @".sh",
    @".sh",
    @"Dockerfile",
    @".html",
    @".config",
    @".xml",
    @".xaml",
    @".aspx",
    @".csproj",
    @".asax",
    @".yml",
    @".settings",
    @"Site.Master"
  };

  private string[] ReadConfigValue(string ignorefiles, string[] defaultValue)
  {
    var configValue = ReadConfigValue(ignorefiles, "");
    if (string.IsNullOrWhiteSpace(configValue)) return defaultValue;
    return configValue.Split(",");
  }
};
