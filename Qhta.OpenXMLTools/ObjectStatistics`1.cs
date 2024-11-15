namespace Qhta.OpenXmlTools;

/// <summary>
/// Represents the statistics of object of a specific type.
/// Each entry in the dictionary represents an object and the number of times it is used.
/// </summary>
public class ObjectStatistics<T>: Dictionary<T, ulong>
{
  /// <summary>
  /// Adds an object count to the statistics.
  /// If the object is already in the statistics, the given count is added to existing object count.
  /// Otherwise, the object is added to the statistics with the given count.
  /// </summary>
  /// <param name="item">Added object</param>
  /// <param name="count">Added object count</param>
  public void Add(T item, int count)
  {
    if (ContainsKey(item))
    {
      this[item]++;
    }
    else
    {
      Add(item, (ulong)count);
    }
  }

  /// <summary>
  /// Adds an object to the statistics.
  /// If the object is already in the statistics, its count is incremented.
  /// Otherwise, the object is added to the statistics with the count of 1.  
  /// </summary>
  /// <param name="item"></param>
  public void Add(T item)
  {
    if (ContainsKey(item))
    {
      this[item]++;
    }
    else
    {
      Add(item, 1);
    }
  }

  /// <summary>
  /// Returns the object that has the highest count.
  /// If two objects have the same count, the first one is returned.
  /// </summary>
  /// <returns></returns>
  public T? MostFrequent()
  {
    if (Count == 0)
      return default;
    return this.OrderByDescending(x => x.Value).First().Key;
  }

  /// <summary>
  /// Adds the counts of another ObjectStatistics object to this object.
  /// </summary>
  /// <param name="other"></param>
  public void Add(ObjectStatistics<T> other)
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
