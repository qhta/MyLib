namespace Qhta.UnicodeBuild.ViewModels;

public interface ILongTextViewModel
{
  public bool CanBeWrapped { get; }
  public bool IsWrapped { get; set; }
}