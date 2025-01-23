namespace Qhta.Unicode;

/// <summary>
/// An index of codePoints with specific UcdCategory.
/// </summary>
public class CategoryIndex
{
  private UnicodeData Ucd = null!;
  private readonly Dictionary<UcdCategory, List<CodePoint>> Index = new();

  /// <summary>
  /// Initializes DecompositionIndex from a UnicodeData object.
  /// </summary>
  /// <param name="ucd"></param>
  public void Initialize(UnicodeData ucd)
  {
    Ucd = ucd;
    foreach (CharInfo charInfo in ucd.Values)
    {
        Add(charInfo.Category, charInfo.CodePoint);
    }
  }

  /// <summary>
  /// Add a code point to the index.
  /// </summary>
  /// <param name="category"></param>
  /// <param name="codePoint"></param>
  private void Add(UcdCategory category, int codePoint)
  {
    if (Index.TryGetValue(category, out var value))
    {
      value.Add(codePoint);
    }
    else
    {
      Index.Add(category, [codePoint]);
    }
  }

  /// <summary>
  /// Number of code points in the index.
  /// </summary>
  public int Count => Index.Count;

  /// <summary>
  /// Get the code points for an UcdCategory.
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  public List<CodePoint> this[UcdCategory type] => Index.TryGetValue(type, out var value) ? value : new List<CodePoint>();

  /// <summary>
  /// Get the first count code points in the index.
  /// </summary>
  /// <param name="count"></param>
  /// <returns></returns>
  public IEnumerable<KeyValuePair<UcdCategory, List<CodePoint>>> Take(int count) => Index.Take(count);
}
