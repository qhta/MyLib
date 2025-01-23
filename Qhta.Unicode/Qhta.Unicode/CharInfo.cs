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
    public CodePoint CodePoint { get; set; }
    /// <summary>
    /// Name of the character.
    /// </summary>
    public HashedName Name { get; set; } = null!;
    /// <summary>
    /// General category of the character.
    /// </summary>
    public UcdCategory Category { get; set; }
    /// <summary>
    /// Canonical Combining Class of the character.
    /// </summary>
    public CCClass CCClass { get; set; }
    /// <summary>
    /// Bidirectional class of the character.
    /// </summary>
    public BiDiClass? BiDiClass { get; set; }
    /// <summary>
    /// Decomposition of the character.
    /// </summary>
    public Decomposition? Decomposition { get; set; }
    /// <summary>
    /// Corresponding decimal digit value of the character.
    /// </summary>
    public string? DecDigit { get; set; }
    /// <summary>
    /// Corresponding digit value of the character.
    /// </summary>
    public string? Digit { get; set; }
    /// <summary>
    /// Corresponding numeric value of the character (integer or rational number).
    /// </summary>
    public string? NumVal { get; set; }
    /// <summary>
    /// Whether the character is mirrored in bidirectional text.
    /// </summary>
    public bool BidiMirrored { get; set; }
    //public string? Unicode1Name { get; set; }
    //public string? ISOComment { get; set; }
    /// <summary>
    /// Corresponding Uppercase character code point.
    /// </summary>
    public CodePoint? SimpleUppercaseMapping { get; set; }
    /// <summary>
    /// Corresponding Lowercase character code point.
    /// </summary>
    public CodePoint? SimpleLowercaseMapping { get; set; }
    /// <summary>
    /// Corresponding Titlecase character code point.
    /// </summary>
    public CodePoint? SimpleTitlecaseMapping { get; set; }
    /// <summary>
    /// Optional script of the character (4-character ISO 15924 script code).
    /// </summary>
    public string? Script { get; set; }
    /// <summary>
    /// Optional alias names for the character.
    /// </summary>
    public List<NameAlias>? Aliases { get; set; } = null;

    /// <summary>
    /// Enumerates all names for the character.
    /// First is the primary name, then any aliases.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<HashedName> GetAllNames()
    {
      yield return Name;
      if (Aliases!=null)
      {
        foreach (var alias in Aliases)
        {
          yield return alias.Alias;
        }
      }
    }
  }

}
