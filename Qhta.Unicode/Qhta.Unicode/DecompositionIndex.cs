using System.Diagnostics;

using Qhta.TextUtils;


namespace Qhta.Unicode;

/// <summary>
/// An index of codePoints with specific DecompositionType
/// </summary>
public class DecompositionIndex
{
  private readonly UnicodeData Ucd;
  private readonly Dictionary<DecompositionType, List<int>> Index = new();

  /// <summary>
  /// This constructor creates a DecompositionIndex from a UnicodeData object.
  /// </summary>
  /// <param name="ucd"></param>
  public DecompositionIndex(UnicodeData ucd)
  {
    Ucd = ucd;
    foreach (CharInfo charInfo in ucd.Values)
    {
      if (charInfo.Decomposition is not null)
      {
        Add(charInfo.Decomposition.Type ?? DecompositionType.Unknown, charInfo.CodePoint);
      }
    }
  }

  private void Add(DecompositionType type, int codePoint)
  {
    if (Index.TryGetValue(type, out var value))
    {
      value.Add(codePoint);
    }
    else
    {
      Index.Add(type, [codePoint]);
    }
  }

  public int Count => Index.Count;

  public List<int> this[DecompositionType type] => Index.TryGetValue(type, out var value) ? value : new List<int>();

  public IEnumerable<KeyValuePair<DecompositionType, List<int>>> Take(int count) => Index.Take(count);
}
