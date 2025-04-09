namespace Qhta.UnicodeBuild.ViewModels;

public interface ILongTextViewModel
{
  public string? LongText { get; set; }
  public bool CanExpandRowHeight { get; }
  public bool IsRowHeightExpanded { get; set; }
}