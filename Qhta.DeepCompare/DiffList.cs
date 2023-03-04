namespace Qhta.DeepCompare;

/// <summary>
/// List of compare differences.
/// </summary>
public class DiffList: List<Diff>
{
  /// <summary>
  ///   Helper method to add a difference.
  /// </summary>
  /// <param name="objectName">Name of the object.</param>
  /// <param name="propertyName">Name of the property.</param>
  /// <param name="expValue">Expected value.</param>
  /// <param name="recValue">Received value.</param>
  public void Add(string? objectName, string? propertyName, object? expValue, object? recValue)
  {
    Add(new Diff(objectName, propertyName, expValue, recValue));
  }
}