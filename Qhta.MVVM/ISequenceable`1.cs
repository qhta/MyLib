namespace Qhta.MVVM
{
  public interface ISequenceable<T> where T: class
  {
    T Next { get; set; }
    T Prior { get; set; }
  }
}
