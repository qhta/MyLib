namespace Qhta.UnicodeBuild.ViewModels;

public interface ILongTextViewModel
{
  public string? LongText { get; set; }
  public bool CanExpandLongText { get; }
  public bool IsLongTextExpanded { get; set; }
}