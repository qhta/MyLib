namespace Qhta.Unicode;

/// <summary>
/// An index of codePoints with specific script. Script is a 4-character ISO 15924 script code.
/// </summary>
public class ScriptIndex
{
  private UnicodeData Ucd = null!;
  private readonly Dictionary<string, List<CodePoint>> Index = new();

  /// <summary>
  /// Initializes ScriptIndex from a UnicodeData object.
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
    if (Index.TryGetValue(script, out var value))
    {
      value.Add(codePoint);
    }
    else
    {
      Index.Add(script, [codePoint]);
    }
  }

  /// <summary>
  /// Number of code points in the index.
  /// </summary>
  public int Count => Index.Count;

  /// <summary>
  /// Get the code points for a specific script. Script is a 4-character ISO 15924 script code.
  /// </summary>
  /// <param name="script"></param>
  /// <returns></returns>
  public List<CodePoint> this[string script] => Index.TryGetValue(script, out var value) ? value : new List<CodePoint>();

  /// <summary>
  /// Get the first count code points in the index.
  /// </summary>
  /// <param name="count"></param>
  /// <returns></returns>
  public IEnumerable<KeyValuePair<string, List<CodePoint>>> Take(int count) => Index.Take(count);
}
