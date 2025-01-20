namespace Qhta.Unicode
{
  public record CharInfo
  {
    public int CodePoint { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public byte CanonicalCombiningClass { get; set; }
    public string? BidiClass { get; set; }
    public string? DecompositionType { get; set; }
    public string? NumericValue { get; set; }
    public string? NumericType { get; set; }
    public bool BidiMirrored { get; set; }
    public string? Unicode1Name { get; set; }
    public string? ISOComment { get; set; }
    public string? SimpleUppercaseMapping { get; set; }
    public string? SimpleLowercaseMapping { get; set; }
    public string? SimpleTitlecaseMapping { get; set; }
    public List<NameAlias>? Aliases { get; set; } = null;
  }
}
