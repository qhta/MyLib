namespace Qhta.Erratum
{
  public class ErrLine
  {
    public int Number { get; internal set; }

    public int Count { get; internal set; } = 1;

    public string Text { get; internal set; }

    public string Repl { get; internal set; }

    public int Move { get; internal set; }

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
