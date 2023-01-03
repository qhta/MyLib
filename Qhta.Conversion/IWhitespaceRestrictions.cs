namespace Qhta.Conversion;

public enum WhitespaceBehavior
{
  Preserve = 0,
  Replace = 1,
  Collapse = 2
}

public interface IWhitespaceRestrictions
{
  public WhitespaceBehavior Whitespaces { get; set; }
  public bool WhitespacesFixed { get; set; }
}