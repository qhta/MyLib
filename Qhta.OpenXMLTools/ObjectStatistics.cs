using System;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Represents the statistics of Objects.
/// Each entry in the dictionary represents an object and the number of times it is used.
/// </summary>
public class ObjectStatistics: Dictionary<Object, ulong>
{
  /// <summary>
  /// Adds an object count to the statistics.
  /// If the oject is already in the statistics, the given count is added to existing object count.
  /// Otherwise, the object is added to the statistics with the given count.
  /// </summary>
  /// <param name="str">Added Object</param>
  /// <param name="count">Added Object count</param>
  public void Add(Object str, int count)
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
  /// Adds an object to the statistics.
  /// If the object is already in the statistics, its count is incremented.
  /// Otherwise, the object is added to the statistics with the count of 1.  
  /// </summary>
  /// <param name="str"></param>
  public void Add(Object str)
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
  /// Returns the object that has the highest count.
  /// If two objects have the same count, the first one is returned.
  /// </summary>
  /// <returns></returns>
  public Object MostFrequent()
  {
    return this.OrderByDescending(x => x.Value).First().Key;
  }

  /// <summary>
  /// Adds the counts of another ObjectStatistics object to this object.
  /// </summary>
  /// <param name="other"></param>
  public void Add(ObjectStatistics other)
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
