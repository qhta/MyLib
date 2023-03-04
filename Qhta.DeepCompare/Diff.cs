namespace Qhta.DeepCompare;

/// <summary>
/// Class representing a single difference
/// </summary>
public class Diff
{
  /// <summary>
  /// The name of the checked object. Usually type name.
  /// </summary>
  public string? ObjectName { get; set; }

  /// <summary>
  /// Property name.
  /// </summary>
  public string? PropertyName { get; set; }

  /// <summary>
  /// Expected value.
  /// </summary>
  public object? ExpectedValue { get; set; }

  /// <summary>
  /// Received value.
  /// </summary>
  public object? ReceivedValue { get; set; }

  /// <summary>
  /// Initializes a new instance of the <see cref="Diff"/> class.
  /// </summary>
  /// <param name="objectName">Name of the checked object.</param>
  /// <param name="propertyName">Name of the different property.</param>
  /// <param name="expValue">Expected value.</param>
  /// <param name="recValue">Received value.</param>
  public Diff(string? objectName, string? propertyName, object? expValue, object? recValue)
  {
    ObjectName = objectName;
    PropertyName = propertyName;
    ExpectedValue = expValue;
    ReceivedValue = recValue;
  }

  /// <summary>
  /// Converts to string (for debugging purpose).
  /// </summary>
  public override string ToString()
  {
    var result = $"{StringUtils.Concat2(ObjectName, ".", PropertyName)}";
    if (ReceivedValue != null && ExpectedValue == null)
      result += $" is {ReceivedValue} but should be null";
    else if (ReceivedValue == null && ExpectedValue != null)
      result += $" is null but should be {ExpectedValue}";
    else if (ReceivedValue == null && ExpectedValue == null)
      result += $" is null and should be null";
    else
      result += $" is {ReceivedValue} but should be {ExpectedValue}";
    return result;
  }

}
