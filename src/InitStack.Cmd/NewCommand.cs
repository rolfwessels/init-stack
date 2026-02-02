using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InitStack.Cmd.Sources;
using Spectre.Console;
using Spectre.Console.Cli;

namespace InitStack.Cmd;

public sealed class NewCommand : AsyncCommand<NewCommand.Settings>
{
  public sealed class Settings : CommandSettings
  {
    [CommandArgument(0, "[template]")]
    [Description("The folder where the template is located")]
    public string? TemplateSelected { get; set; }

    [CommandArgument(1, "[new solution names]")]
    [Description("The name of the new solution")]
    public string? NewSolutionName { get; set; }

    [CommandOption("-o|--output")]
    [Description("The folder where the new stack will be written")]
    public string? OutputFolder { get; set; }

    [CommandOption("--init-git")]
    [Description("Initialize git repo")]
    public bool? InitGit { get; set; }

    [CommandOption("--git-name")]
    [Description("Set git username at init")]
    public string? GitUser { get; set; }

    [CommandOption("--git-email")]
    [Description("Set git email at init")]
    public string? GitEmail { get; set; }
  }


  public static string Name = "new";
  public static string Description = "Initializes a new stack";


  public override Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken)
  {
    var source = GetStackSource(settings.TemplateSelected);
    var toFolder = Path.GetFullPath(settings.OutputFolder ?? GlobalSettings.Default.DefaultOutputFolder);
    AnsiConsole.MarkupLine($"You picked: [green]'{source}'[/]");

    var solutionName = settings.NewSolutionName ??
                       AnsiConsole.Prompt(new TextPrompt<string>("What is the name of the new solution?"));
    var stackBuilder = new StackBuilder(source.ToFileSystem(), toFolder, solutionName);


    var files = stackBuilder.Build();
    AnsiConsole.MarkupLine($"Created [green]{files.Length}[/] files in [green]{stackBuilder.OutputFolder}[/]");
    if (settings.InitGit ?? GlobalSettings.Default.InitGitByDefault)
    {
      var gitHistory = new GitHistory();
      if (settings.GitUser != null)
      {
        gitHistory.OverrideUser = settings.GitUser;
      }

      if (settings.GitEmail != null)
      {
        gitHistory.OverrideEmail = settings.GitEmail;
      }

      if (settings.GitUser == null && gitHistory.GetCurrentUser() == "")
      {
        var user = AnsiConsole.Prompt(new TextPrompt<string>("What is your git user name?"));
        gitHistory.OverrideUser = user;
      }

      if (settings.GitEmail == null && gitHistory.GetCurrentUserEmail() == "")
      {
        var email = AnsiConsole.Prompt(new TextPrompt<string>("What is your git email?"));
        gitHistory.OverrideEmail = email;
      }

      AnsiConsole.MarkupLine($"Initializing git repo at [green]{stackBuilder.OutputFolder}[/]");
      gitHistory.GitInit(new DirectoryInfo(stackBuilder.OutputFolder));
    }

    AnsiConsole.MarkupLine($"Done, Your file is locate at [green]{stackBuilder.OutputFolder}[/]");

    return Task.FromResult(0);
  }

  private static IStackSource GetStackSource(string? settingsTemplateFolder)
  {
    if (settingsTemplateFolder != null && Directory.Exists(settingsTemplateFolder))
    {
      return new StackSourceFile(Path.GetFullPath(settingsTemplateFolder).Trim('\\'));
    }

    var sources =
      GlobalSettings.Default.TemplateCollectionFolders
        .Where(Directory.Exists)
        .SelectMany(Directory.GetDirectories)
        .Select(x => new StackSourceFile(Path.GetFileNameWithoutExtension(x), x))
        .Concat(GlobalSettings.Default.GitSources.Select(url => (IStackSource)new StackSourceGit(url)))
        .OrderBy(x => x.Name)
        .ToDictionary(x => x.ToString() ?? "");

    if (settingsTemplateFolder != null)
    {
      var found = sources.Values.FirstOrDefault(x =>
        x.Name.Equals(settingsTemplateFolder, StringComparison.InvariantCultureIgnoreCase));
      if (found != null)
      {
        return found;
      }

      AnsiConsole.MarkupLine($"Could not find the template [red]{settingsTemplateFolder}[/]!");
    }

    var selectionPrompt = new SelectionPrompt<string>()
      .Title("Please pick a template")
      .PageSize(10)
      .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
      .AddChoices(sources.Keys);
    var fromTemplate = AnsiConsole.Prompt(selectionPrompt);
    var source = sources[fromTemplate];
    return source;
  }
}
