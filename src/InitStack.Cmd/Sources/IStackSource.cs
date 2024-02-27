namespace InitStack.Cmd.Sources;

public interface IStackSource
{
  string Name { get; init; }
  StackSourceFile ToFileSystem();
}
