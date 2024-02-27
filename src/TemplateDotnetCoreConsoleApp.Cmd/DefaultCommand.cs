using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Cli;

namespace TemplateDotnetCoreConsoleApp.Cmd;

public sealed class DefaultCommand : AsyncCommand<DefaultCommand.Settings>
{
  public const string Name = "default";
  public static string Desc = "The default command doing default-ish things!";


  public sealed class Settings : CommandSettings
  {
    [CommandArgument(0, "[process]")]
    [Description("The example to run.\nIf none is specified, all examples will be listed")]
    public string? Name { get; set; }

    [CommandOption("-t|--table")]
    [Description("Show table")]
    public bool Table { get; set; }
  }


  public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
  {
    if (settings.Name == null)
    {
      AnsiConsole.MarkupLine("Type [blue]--help[/] for help");
      return 1;
    }

    if (settings.Name == "service")
    {
      await RunService();
      return 0;
    }

    AnsiConsole.MarkupLine($"Processing [green]{settings.Name}[/]..");
    await AnsiConsole.Status().StartAsync("Thinking...", async ctx => { await Task.Delay(1000); });
    if (settings.Table)
    {
      var examples = new[] { new { Test = "test" }, new { Test = "test2" } }.ToList();
      var table = new Table { Border = TableBorder.Rounded }.Expand();
      table.AddColumn(new TableColumn("[yellow]Example[/]") { NoWrap = true });
      table.AddColumn(new TableColumn("[grey]Description[/]"));

      foreach (var group in examples.GroupBy(ex => ex.Test))
      {
        table.AddRow(group.Key, group.Count().ToString());
        table.AddEmptyRow();
      }

      AnsiConsole.Write(table);
    }

    return 0;
  }

  private async Task RunService()
  {
    Console.WriteLine("Application started. Press Ctrl+C to exit.");
    var cancellationTokenSource = new CancellationTokenSource();
    AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
    {
      Console.WriteLine("Application is exiting...");
      cancellationTokenSource.Cancel();
    };
    while (!cancellationTokenSource.IsCancellationRequested)
    {
      await Task.Delay(1000, cancellationTokenSource.Token);
    }
  }
}
