namespace Qhta.Erratum
{
  public class ErrataLines: ErrataEntry
  {
    /// <summary>
    /// Number of the first line in a file when to start operation.
    /// </summary>
    public int? From { get; set; }

    /// <summary>
    /// Number of the last line in a file when to start operation.
    /// Should be greater than <see cref="From"/>
    /// </summary>
    public int? To { get; set; }

  }
}
