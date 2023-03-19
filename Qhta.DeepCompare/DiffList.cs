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

  /// <summary>
  ///   Helper method to add a difference.
  /// </summary>
  /// <param name="objectName">Name of the object.</param>
  /// <param name="propertyName">Name of the property.</param>
  /// <param name="index">Index of the compared values.</param>
  /// <param name="expValue">Expected value.</param>
  /// <param name="recValue">Received value.</param>
  public void Add(string? objectName, string? propertyName, object? index, object? expValue, object? recValue)
  {
    Add(new Diff(objectName, propertyName, index, expValue, recValue));
  }

  /// <summary>
  ///   Message build for assertions.
  ///   First difference is shown as full message.
  ///   If there are more, their count is shown.
  /// </summary>
  public string? AssertMessage
  {
    get
    {
      var result = this.FirstOrDefault()?.ToString();
      if (this.Count()>1)
        result += $" and {this.Count()-1} more differences";
      return result;
    }
  }
}