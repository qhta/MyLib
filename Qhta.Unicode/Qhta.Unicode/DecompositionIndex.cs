namespace Qhta.Unicode;

/// <summary>
/// An index of codePoints with specific DecompositionType
/// </summary>
public class DecompositionIndex: Dictionary<DecompositionType, List<CodePoint>>
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
      if (charInfo.Decomposition is not null)
      {
        Add(charInfo.Decomposition.Type, charInfo.CodePoint);
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
    if (this.TryGetValue(type, out var value))
    {
      value.Add(codePoint);
    }
    else
    {
      this.Add(type, [codePoint]);
    }
  }

}
