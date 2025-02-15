using System.Diagnostics;

namespace Qhta.Unicode
{
  /// <summary>
  /// Unicode character information.
  /// </summary>
  public class CharInfo
  {
    /// <summary>
    /// Code point - integer value of the Unicode character converted to hexadecimal.
    /// </summary>
    public CodePoint CodePoint { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }
    /// <summary>
    /// Name of the character.
    /// </summary>
    public HashedName Name { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = null!;
    /// <summary>
    /// General category of the character.
    /// </summary>
    public UcdCategory Category { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }
    /// <summary>
    /// Canonical Combining Class of the character.
    /// </summary>
    public CCClass CCClass { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }
    /// <summary>
    /// Bidirectional class of the character.
    /// </summary>
    public BiDiClass? BiDiClass { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }
    /// <summary>
    /// Decomposition of the character.
    /// </summary>
    public Decomposition? Decomposition { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }
    /// <summary>
    /// Corresponding decimal digit value of the character.
    /// </summary>
    public string? DecDigit { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }
    /// <summary>
    /// Corresponding digit value of the character.
    /// </summary>
    public string? Digit { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }
    /// <summary>
    /// Corresponding numeric value of the character (integer or rational number).
    /// </summary>
    public string? NumVal { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }
    /// <summary>
    /// Whether the character is mirrored in bidirectional text.
    /// </summary>
    public bool BidiMirrored { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }
    //public string? Unicode1Name { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }
    //public string? ISOComment { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }
    /// <summary>
    /// Corresponding Uppercase character code point.
    /// </summary>
    public CodePoint? SimpleUppercaseMapping { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }
    /// <summary>
    /// Corresponding Lowercase character code point.
    /// </summary>
    public CodePoint? SimpleLowercaseMapping { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }
    /// <summary>
    /// Corresponding Titlecase character code point.
    /// </summary>
    public CodePoint? SimpleTitlecaseMapping { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }
    /// <summary>
    /// Optional script of the character (4-character ISO 15924 script code).
    /// </summary>
    public string? Script { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

    /// <summary>
    /// Optional old name
    /// </summary>
    public HashedName? OldName { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

    /// <summary>
    /// Optional alias names for the character.
    /// </summary>
    public List<NameAlias>? Aliases { [DebuggerStepThrough] get; [DebuggerStepThrough] set; } = null;

    /// <summary>
    /// Enumerates all names for the character.
    /// First is the primary name, then any aliases.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<HashedName> GetAllNames()
    {
      yield return Name;
      if (OldName != null)
        yield return OldName;
      if (Aliases!=null)
      {
        foreach (var alias in Aliases)
        {
          yield return alias.Name;
        }
      }
    }

    /// <summary>
    /// Enumerates all names for the character.
    /// First is the primary name, then any aliases.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> GetLongNames()
    {
      yield return Name;
      if (OldName != null)
        yield return OldName;
      if (Aliases != null)
      {
        foreach (var alias in Aliases)
        {
          yield return alias.Name;
        }
      }
    }
  }

}
