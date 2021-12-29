namespace Qhta.Erratum
{
  public class ErrataLine: ErrataEntry
  {
    /// <summary>
    /// Number of the line in a file
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// How many lines are changed in one operation.
    /// Usually 1.
    /// </summary>
    public int Count { get; set; } = 1;

  }
}
