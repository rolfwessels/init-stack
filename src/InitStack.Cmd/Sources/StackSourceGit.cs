using System;
using System.IO;
using System.Linq;
using System.Reflection;
using InitStack.Cmd.Utils;
using Serilog;

namespace InitStack.Cmd.Sources;

internal record StackSourceGit(string Name, string Url) : IStackSource
{
  private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod()!.DeclaringType!);

  public StackSourceGit(string url) : this(Path.GetFileNameWithoutExtension(url), url)
  {
  }

  public override string ToString()
  {
    return "\ud83c\udf10" + Name;
  }

  public StackSourceFile ToFileSystem()
  {
    _log.Information("Cloning {Name} from {Url}", Name, Url);
    var folder = Path.Combine(Path.GetTempPath(), "StackInit", Name);
    try
    {
      if (Directory.Exists(folder))
      {
        Directory.Delete(folder, true);
      }
    }
    catch (Exception)
    {
      folder += Guid.NewGuid().ToString().Substring(0, 4);
    }

    var output = Util.Cmd("git", $"clone {Url} {folder}");
    _log.Debug("Cloning {Name} from {Url} to {Folder}: {Output}", Name, Url, folder, output);

    // Hack to assume the sln file name is the real name
    var sln = Directory.GetFiles(folder, "*.sln").FirstOrDefault();
    if (sln != null)
    {
      var slnName = Path.GetFileNameWithoutExtension(sln);
      if (slnName != null)
      {
        return new StackSourceFile(slnName, folder);
      }
    }

    return new StackSourceFile(Name, folder);
  }
};
