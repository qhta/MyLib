namespace Qhta.Unicode;

/// <summary>
/// An index of codePoints with specific UcdCategory.
/// </summary>
public class CategoryIndex: Dictionary<UcdCategory, List<CodePoint>>
{
  private UnicodeData Ucd = null!;

  /// <summary>
  /// Initializes index from a UnicodeData object.
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
    if (this.TryGetValue(category, out var value))
    {
      value.Add(codePoint);
    }
    else
    {
      this.Add(category, [codePoint]);
    }
  }

}
