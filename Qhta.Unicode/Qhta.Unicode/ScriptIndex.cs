namespace Qhta.Unicode;

/// <summary>
/// An index of codePoints with specific script. Script is a 4-character ISO 15924 script code.
/// </summary>
public class ScriptIndex: Dictionary<string, List<CodePoint>>
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
      if (charInfo.Script is not null)
      {
        Add(charInfo.Script!, charInfo.CodePoint);
      }
    }
  }

  /// <summary>
  /// Add a code point to the index.
  /// </summary>
  /// <param name="script">Script is a 4-character ISO 15924 script code.</param>
  /// <param name="codePoint"></param>
  private void Add(string script, int codePoint)
  {
    if (this.TryGetValue(script, out var value))
    {
      value.Add(codePoint);
    }
    else
    {
      this.Add(script, [codePoint]);
    }
  }

}
