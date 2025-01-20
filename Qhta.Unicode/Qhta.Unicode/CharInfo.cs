namespace Qhta.Unicode
{
  public record CharInfo
  {
    public int CodePoint { get; set; }
    public HashedName Name { get; set; } = string.Empty;
    public Category Category { get; set; }
    public CCClass CCClass { get; set; }
    public BiDiClass? BiDiClass { get; set; }
    public Decomposition? Decomposition { get; set; }
    public string? NumericValue { get; set; }
    public string? NumericType { get; set; }
    public bool BidiMirrored { get; set; }
    public string? Unicode1Name { get; set; }
    public string? ISOComment { get; set; }
    public string? SimpleUppercaseMapping { get; set; }
    public string? SimpleLowercaseMapping { get; set; }
    public string? SimpleTitlecaseMapping { get; set; }
    public List<NameAlias>? Aliases { get; set; } = null;

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
