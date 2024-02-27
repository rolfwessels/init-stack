using System;
using System.Linq;
using System.Text.RegularExpressions;
using Humanizer;

namespace InitStack.Cmd;

public class ReplaceItAll
{
  private readonly IReplacements[] _regexReplace;

  public ReplaceItAll(string from, string to)
  {
    _regexReplace = GetReplace(from, to);
  }

  private IReplacements[] GetReplace(string solutionName, string newSolutionName)
  {
    if (solutionName.Contains("-") || solutionName.Contains("_"))
      solutionName = solutionName.Replace("_", " ").Replace("-", " ").Pascalize();
    var solutionNameSplit = SplitCamelCase(solutionName);

    var newSolutionNameSplit = SplitCamelCase(newSolutionName).Replace(".", "");
    var simpleOnes = new[]
      {
        new StringReplacements("__NAME__", newSolutionName),
        new StringReplacements(solutionName, newSolutionName),
        new StringReplacements(solutionName.Pluralize(), newSolutionName.Pluralize()),
        new StringReplacements(solutionName.ToUpper(), newSolutionName.ToUpper()),
        new StringReplacements(solutionName.ToLower(), newSolutionName.ToLower()),

        new StringReplacements(solutionNameSplit, newSolutionNameSplit),
        new StringReplacements(InitCapital(solutionNameSplit), InitCapital(newSolutionNameSplit)),
        new StringReplacements(solutionNameSplit.ToLower(), newSolutionNameSplit.ToLower()),

        new StringReplacements(solutionNameSplit.Replace(" ", "-").ToLower(),
          newSolutionNameSplit.Replace(" ", "-").ToLower()),
        new StringReplacements(solutionNameSplit.Replace(" ", "_").ToLower(),
          newSolutionNameSplit.Replace(" ", "_").ToLower()),
        new StringReplacements(solutionName.Camelize(), newSolutionName.Camelize())
      }.OrderByDescending(x => x.Length)
      .OfType<IReplacements>();

    var regexOnes = new RegexUpdates[]
    {
      new("assembly\\: Guid\\(\\\"[a-z0-9\\-]+\\\"\\)", () => $"assembly: Guid(\"{Guid.NewGuid()}\")"),
      new("ProductID=\\\"\\{[a-z0-9\\-]+\\}\\\"", () => $"ProductID=\"{{{Guid.NewGuid()}}}\" "),
      new("AppId = \\\"[a-z0-9\\-]+\\\"", () => $"AppId = \"{Guid.NewGuid()}\""),
      new(@"\<ProjectGuid\>\{[A-z0-9\\-]+\}\<\/ProjectGuid\>",
        () => $@"<ProjectGuid>{{{Guid.NewGuid()}}}</ProjectGuid>")
    };
    return simpleOnes.Concat(regexOnes).ToArray();
  }

  private string InitCapital(string solutionNameSplit)
  {
    return solutionNameSplit.Substring(0, 1).ToUpper() + solutionNameSplit.Substring(1).ToLower();
  }

  public static string SplitCamelCase(string input)
  {
    return Regex.Replace(input, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
  }

  public string ReplaceIt(string inputText)
  {
    foreach (var replace in _regexReplace)
    {
      var (newValue, found) = replace.Replace(inputText);
      inputText = newValue;
    }

    return inputText;
  }

  private interface IReplacements
  {
    (string, bool) Replace(string inputText);
  }

  private class RegexUpdates(string input, Func<string> replaceFunction) : IReplacements
  {
    public (string, bool) Replace(string inputText)
    {
      var replace = Regex.Replace(inputText, input, replaceFunction());
      return (replace, replace != inputText);
    }
  }

  internal class StringReplacements(string input, string replaceWith) : IReplacements
  {
    public int Length => replaceWith.Length;

    public (string, bool) Replace(string inputText)
    {
      var replace = inputText.Replace(input, replaceWith);
      return (replace, replace != inputText);
    }
  }
}
