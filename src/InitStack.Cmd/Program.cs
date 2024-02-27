using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Spectre.Console;
using Spectre.Console.Cli;

namespace InitStack.Cmd;

internal class Program
{
  public static async Task<int> Main(string[] args)
  {
    Log.Logger = CreateLogger();
    var app = new CommandApp();
    Console.OutputEncoding = Encoding.UTF8;

    app.Configure(config =>
    {
      config.SetApplicationName("Init-Stack");
      config.SetExceptionHandler(e =>
      {
        Log.Logger.Error(e, e.Message);
        AnsiConsole.MarkupLine($"[red]Error:[/]{Markup.Escape(e.Message)}");
      });

      config.AddCommand<NewCommand>(NewCommand.Name)
        .WithAlias("d")
        .WithDescription(NewCommand.Description);
      // .WithExample(DefaultCommand.Name, "-t true", "DarthPedro");
      config.ValidateExamples();
    });

    return await app.RunAsync(args);
  }

  private static Logger CreateLogger()
  {
    return new LoggerConfiguration()
      .MinimumLevel.Information()
      .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
      // .WriteTo.Console()
      .WriteTo.RollingFile(Path.Combine(Path.GetTempPath(), "init-stack.log"))
      .CreateLogger();
  }
}
