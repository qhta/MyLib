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
  /// index of the checked item
  /// </summary>
  public object? Index { get; set; }

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
  /// Initializes a new instance of the <see cref="Diff"/> class.
  /// </summary>
  /// <param name="objectName">Name of the checked object.</param>
  /// <param name="propertyName">Name of the different property.</param>
  /// <param name="index">Name of the different property.</param>
  /// <param name="expValue">Expected value.</param>
  /// <param name="recValue">Received value.</param>
  public Diff(string? objectName, string? propertyName, object? index, object? expValue, object? recValue)
  {
    ObjectName = objectName;
    PropertyName = propertyName;
    Index = index;
    ExpectedValue = expValue;
    ReceivedValue = recValue;
  }

  /// <summary>
  /// Converts to string (for debugging purpose).
  /// </summary>
  public override string ToString()
  {
    string receivedStr = (ReceivedValue is string str1) ? "\""+str1+"\"" : ReceivedValue?.ToString() ?? "null";
    string expectedStr = (ExpectedValue is string str2) ? "\""+str2+"\"" : ExpectedValue?.ToString() ?? "null";
    string? indexStr = (Index != null) ? "[" + Index.ToString()+"]" : null;
    var result = Concat(ObjectName, PropertyName, indexStr);
    if (ReceivedValue != null && ExpectedValue == null)
      result += $" is {receivedStr} but should be null";
    else if (ReceivedValue == null && ExpectedValue != null)
      result += $" is null but should be {expectedStr}";
    else if (ReceivedValue == null && ExpectedValue == null)
      result += $" is null and should be null";
    else
      result += $" is {receivedStr} but should be {expectedStr}";
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

  /// <summary>
  /// Concats the specified object name with property name with separating dot. 
  /// If property name is indexing clause in bracket, dot is ommited.
  /// </summary>
  /// <param name="objectName">Name of the object.</param>
  /// <param name="propertyName">Name of the property.</param>
  /// <param name="indexStr">String of the index.</param>
  /// <returns></returns>
  public static string Concat(string? objectName, string? propertyName, string? indexStr)
  {
     if (propertyName==null)
      propertyName = indexStr;
     else if (indexStr!=null)
      propertyName += indexStr;
     var result = propertyName?.StartsWith("[")==true 
      ? $"{String.Concat(objectName, propertyName)}" 
      : $"{StringUtils.Concat2(objectName, ".", propertyName)}";
    return result;
  }
}
