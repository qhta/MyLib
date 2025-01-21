namespace Qhta.Unicode;

/// <summary>
/// An index of codePoints with specific DecompositionType
/// </summary>
public class DecompositionIndex
{
  private UnicodeData Ucd = null!;
  private readonly Dictionary<DecompositionType, List<CodePoint>> Index = new();

  /// <summary>
  /// Initializes DecompositionIndex from a UnicodeData object.
  /// </summary>
  /// <param name="ucd"></param>
  public void Initialize(UnicodeData ucd)
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

  /// <summary>
  /// Add a code point to the index.
  /// </summary>
  /// <param name="type"></param>
  /// <param name="codePoint"></param>
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

  /// <summary>
  /// Number of code points in the index.
  /// </summary>
  public int Count => Index.Count;

  /// <summary>
  /// Get the code points for a DecompositionType.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  public List<CodePoint> this[DecompositionType type] => Index.TryGetValue(type, out var value) ? value : new List<CodePoint>();

  /// <summary>
  /// Get the first count code points in the index.
  /// </summary>
  /// <param name="count"></param>
  /// <returns></returns>
  public IEnumerable<KeyValuePair<DecompositionType, List<CodePoint>>> Take(int count) => Index.Take(count);
}
