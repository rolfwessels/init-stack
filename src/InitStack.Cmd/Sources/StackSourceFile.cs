using System.IO;

namespace InitStack.Cmd.Sources;

public record StackSourceFile(string Name, string Folder) : IStackSource
{
  public StackSourceFile(string folder) : this(Path.GetFileNameWithoutExtension(folder),
    folder)
  {
  }

  public override string ToString()
  {
    return "\ud83d\udcc1" + Name;
  }

  public StackSourceFile ToFileSystem()
  {
    return this;
  }
}
