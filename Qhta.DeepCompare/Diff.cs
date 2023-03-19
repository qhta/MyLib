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
    var receivedValue = ReceivedValue;
    if (receivedValue is string str1)
      receivedValue = "\""+str1+"\"";
    var expectedValue = ExpectedValue;
    if (expectedValue is string str2)
      expectedValue = "\""+str2+"\"";
    var result = Concat(ObjectName, PropertyName);
    if (ReceivedValue != null && ExpectedValue == null)
      result += $" is {receivedValue} but should be null";
    else if (ReceivedValue == null && ExpectedValue != null)
      result += $" is null but should be {expectedValue}";
    else if (ReceivedValue == null && ExpectedValue == null)
      result += $" is null and should be null";
    else
      result += $" is {receivedValue} but should be {expectedValue}";
    return result;
  }

  /// <summary>
  /// Concats the specified object name with property name with separating dot. 
  /// If property name is indexing clause in bracket, dot is ommited.
  /// </summary>
  /// <param name="objectName">Name of the object.</param>
  /// <param name="propertyName">Name of the property.</param>
  /// <returns></returns>
  public static string Concat(string? objectName, string? propertyName)
  {
     var result = propertyName?.StartsWith("[")==true 
      ? $"{String.Concat(objectName, propertyName)}" 
      : $"{StringUtils.Concat2(objectName, ".", propertyName)}";
    return result;
  }
}
