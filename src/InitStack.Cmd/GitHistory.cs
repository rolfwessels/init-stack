using System;
using System.IO;
using System.Linq;
using InitStack.Cmd.Utils;

namespace InitStack.Cmd;

public class GitHistory
{
  public string? OverrideUser { get; set; }
  public string? OverrideEmail { get; set; }

  public void GitInit(DirectoryInfo mainPath)
  {
    if (Directory.Exists(Path.Combine(mainPath.FullName, ".git")))
      Directory.Delete(Path.Combine(mainPath.FullName, ".git"), true);
    Directory.SetCurrentDirectory(mainPath.FullName);

    var message = RandomMessage();
    Util.Cmd("git", "init --initial-branch=main");
    Util.Cmd("git", "config core.autocrlf false");
    if (OverrideUser != null)
      Util.Cmd("git", $"config user.name \"{OverrideUser}\"");
    if (OverrideEmail != null)
      Util.Cmd("git", $"config user.email \"{OverrideEmail}\"");
    Util.Cmd("git", "add * ");
    Util.Cmd("git", $"commit -a -m \"{message}\" ");
  }

  public string RandomMessage()
  {
    var messages = Messages();
    var message = messages[new Random().Next(0, messages.Length - 1)];
    return message;
  }

  public static string[] Messages()
  {
    var commit = GlobalSettings.Default.CommitMessages
      .Select(x => x.Trim())
      .Where(x => !string.IsNullOrEmpty(x))
      .ToArray();
    return commit;
  }

  public string GetCurrentUser()
  {
    return Util.Cmd("git", "config user.name");
  }

  public string GetCurrentUserEmail()
  {
    return Util.Cmd("git", "config user.email");
  }
}
