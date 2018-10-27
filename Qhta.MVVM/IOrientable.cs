namespace Qhta.MVVM
{
  public enum OrientationType
  {
    Horizontal = 0,
    Vertical = 1
  }

  public interface IOrientable
  {
    OrientationType Orientation { get; }
  }
}
