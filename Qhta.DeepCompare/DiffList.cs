namespace Qhta.DeepCompare;

/// <summary>
/// Delegate to event hander invoked on adding a diff.
/// </summary>
/// <param name="sender"></param>
/// <param name="diff"></param>
public delegate void OnAddDiff(DiffList sender, Diff diff);

/// <summary>
/// List of compare differences.
/// </summary>
public class DiffList: List<Diff>
{

  /// <summary>
  /// Event invoked when a diff is added.
  /// </summary>
  public event OnAddDiff? AddDiff;

  /// <summary>
  ///   Helper method to add a difference.
  /// </summary>
  /// <param name="objectName">Name of the object.</param>
  /// <param name="expValue">Expected value.</param>
  /// <param name="recValue">Received value.</param>
  public void Add(string? objectName, object? expValue, object? recValue)
  {
    Add(objectName, null, null, expValue, recValue);
  }

  /// <summary>
  ///   Helper method to add a difference.
  /// </summary>
  /// <param name="objectName">Name of the object.</param>
  /// <param name="propertyName">Name of the property.</param>
  /// <param name="expValue">Expected value.</param>
  /// <param name="recValue">Received value.</param>
  public void Add(string? objectName, string? propertyName, object? expValue, object? recValue)
  {
    Add(objectName, propertyName, null, expValue, recValue);
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
    var diff = new Diff(objectName, propertyName, index, expValue, recValue);
    AddDiff?.Invoke(this, diff);
    Add(diff);
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