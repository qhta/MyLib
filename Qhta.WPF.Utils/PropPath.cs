namespace Qhta.WPF.Utils;

/// <summary>
/// Holds info of column binding properties.
/// As binding path may complex, it is an array of property info items, which values must be evaluated in cascade.
/// </summary>
public class PropPath : IEnumerable<PropertyInfo>
{
  private List<PropertyInfo> _properties = new List<PropertyInfo>();

  /// <summary>
  /// Default constructor
  /// </summary>
  public PropPath(){ }

  /// <summary>
  /// Creates new PropPath starting from type of dataContext.
  /// </summary>
  /// <param name="dataContextType"></param>
  /// <param name="path"></param>
  public PropPath(Type dataContextType, string path)
  {
    var ss = path.Split('.');
    foreach (var s in ss)
    { 
      var str = s;
      var k = str.IndexOf('[');
      if (k != -1)
        str = str.Substring(0, k);
      var propInfo = dataContextType.GetProperty(str);
      if (propInfo == null)
        throw new InvalidOperationException($"Property \"{str}\" not found in {dataContextType}");
      Add(propInfo);
      dataContextType = propInfo.PropertyType;
    }
  }

  /// <summary>
  /// Enumerator of PropertyInfo
  /// </summary>
  /// <returns></returns>
  public IEnumerator<PropertyInfo> GetEnumerator()
  {
    return ((IEnumerable<PropertyInfo>)_properties).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return ((IEnumerable)_properties).GetEnumerator();
  }

  /// <summary>
  /// Adding propertyInfo to the sequence.
  /// </summary>
  /// <param name="property"></param>
  public void Add(PropertyInfo property)
  {
    _properties.Add(property);
  }

  /// <summary>
  /// Clears a path.
  /// </summary>
  public void Clear() => _properties.Clear();

  /// <inheritdoc />
  public override string? ToString()
  {
    return String.Join(".",_properties.Select(item=>item.Name));
  }
}
