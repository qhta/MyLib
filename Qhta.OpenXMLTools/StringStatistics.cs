namespace Qhta.OpenXmlTools;

/// <summary>
/// Represents the statistics of strings.
/// Each entry in the dictionary represents a string and the number of times it is used.
/// </summary>
public class StringStatistics: Dictionary<string, ulong>
{
  /// <summary>
  /// Adds a string count to the statistics.
  /// If the string is already in the statistics, the given count is added to existing string count.
  /// Otherwise, the string is added to the statistics with the given count.
  /// </summary>
  /// <param name="str">Added string</param>
  /// <param name="count">Added string count</param>
  public void Add(string str, int count)
  {
    if (ContainsKey(str))
    {
      this[str]++;
    }
    else
    {
      Add(str, (ulong)count);
    }
  }

  /// <summary>
  /// Adds a string to the statistics.
  /// If the string is already in the statistics, its count is incremented.
  /// Otherwise, the string is added to the statistics with the count of 1.  
  /// </summary>
  /// <param name="str"></param>
  public void Add(string str)
  {
    if (ContainsKey(str))
    {
      this[str]++;
    }
    else
    {
      Add(str, 1);
    }
  }

  /// <summary>
  /// Returns the string that has the highest count.
  /// If two strings have the same count, the first one is returned.
  /// </summary>
  /// <returns></returns>
  public string MostFrequent()
  {
    return this.OrderByDescending(x => x.Value).First().Key;
  }

  /// <summary>
  /// Adds the counts of another StringStatistics object to this object.
  /// </summary>
  /// <param name="other"></param>
  public void Add(StringStatistics other)
  {
    foreach (var item in other)
    {
      if (ContainsKey(item.Key))
      {
        this[item.Key] += item.Value;
      }
      else
      {
        Add(item.Key, item.Value);
      }
    }
  }
}
