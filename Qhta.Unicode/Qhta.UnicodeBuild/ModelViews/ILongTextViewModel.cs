namespace Qhta.UnicodeBuild.ViewModels;

public interface ILongTextViewModel
{
  public bool CanExpandRowHeight { get; }
  public bool IsRowHeightExpanded { get; set; }
}