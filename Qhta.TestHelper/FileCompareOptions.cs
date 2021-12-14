namespace Qhta.TestHelper
{
  public class FileCompareOptions
  {
    public bool IgnoreCase { get; set; } = false;

    public bool IgnoreAttributesOrder { get; set; } = false;

    public bool WriteContentIfEquals { get; set; }

    /// <summary>
    /// How many differences to show before return.
    /// </summary>
    public int DiffLimit { get; set; }

    /// <summary>
    /// What is the maximum distance to search for the same line when difference.
    /// </summary>
    public int SyncLimit { get; set; }

    ///// <summary>
    ///// What is the maximum number of same lines between two diffs to consider as a single diff.
    ///// </summary>
    //public int DiffGapLimit { get; set; }

    public string EqualityMsg { get; set; }

    public string InequalityMsg { get; set; }

    public string StartOfDiffOut { get; set; } = "--- out " + new string('-', 20);

    public string StartOfDiffExp { get; set; } = "--- exp " + new string('-', 20);

    public string EndOfDiffs { get; set; } = new string('=', 28);

  }
}
