namespace Qhta.MVVM
{

  /// <summary>
  /// Interface for an object which can be compared to another object.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface ISameAs<T>
  {
    /// <summary>
    /// A method of comparison an object with another object.
    /// It does not need to be shallow or deep equality.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    bool SameAs(T other);
  }
}
