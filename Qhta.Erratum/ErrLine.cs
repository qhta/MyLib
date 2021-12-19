namespace Qhta.Erratum
{
  public class ErrLine
  {
    /// <summary>
    /// Number of the line in a file
    /// </summary>
    public int Number { get; init; }

    /// <summary>
    /// How many lines are changed in one operation.
    /// Usually 1.
    /// </summary>
    public int Count { get; init; } = 1;

    /// <summary>
    /// A text to search.
    /// </summary>
    public string Text { get; init; }

    /// <summary>
    /// A text to replace
    /// </summary>
    public string Repl { get; init; }

    /// <summary>
    /// Move distance.
    /// </summary>
    public int Move { get; init; }

    public ErrLine() { }

    public ErrLine(int lineNo, string text, string repl)
    {
      Number = lineNo;
      Text = text;
      Repl = repl;
    }

    public ErrLine(int from, int count, string text, string repl)
    {
      Number = from;
      Count = count;
      Text = text;
      Repl = repl;
    }

    public ErrLine(int lineNo, string text, int move)
    {
      Number = lineNo;
      Text = text;
      Move = move;
    }

    public ErrLine(int from, int count, string text, int move)
    {
      Number = from;
      Count = count;
      Text = text;
      Move = move;
    }
  }
}
