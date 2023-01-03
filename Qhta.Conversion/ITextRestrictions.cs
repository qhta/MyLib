namespace Qhta.Conversion;

public interface ITextRestrictions
{
  public string[]? Patterns { get; set; }

  public string[]? Enumerations { get; set; }

  public bool CaseInsensitive { get; set; }
}