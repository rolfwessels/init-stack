using System.Diagnostics;

namespace InitStack.Cmd.Utils;

internal class Util
{
  public static string Cmd(string command, string? arguments = null, bool b = false)
  {
    var startInfo = new ProcessStartInfo()
    {
      FileName = command,
      Arguments = arguments,
      RedirectStandardOutput = true,
      RedirectStandardError = true,
      UseShellExecute = false,
      CreateNoWindow = true,
    };

    using var process = Process.Start(startInfo);
    if (process == null)
    {
      return $"Process [{command} {arguments}] start failed.";
    }

    process.WaitForExit();

    var output = process.StandardOutput.ReadToEnd();
    var error = process.StandardError.ReadToEnd();

    if (!string.IsNullOrEmpty(error))
    {
      return $"Error: {error}";
    }

    return output;
  }
}
