namespace Qhta.WPF.Utils;

/// <summary>
/// DataSourceProvider that provides enum type values.
/// </summary>
public class EnumValuesProvider : DataSourceProvider
{
  /// <summary>
  /// Enum type to get values.
  /// </summary>
  public Type ObjectType { get; set; } = null!;

  /// <summary>
  /// Type of the item.
  /// </summary>
  public Type? ItemType { get; set; }

  /// <summary>
  /// Specifies whether it should add a null value.
  /// </summary>
  public bool AddNull { get; set; }


  /// <summary>
  /// Starts querying.
  /// </summary>
  protected override void BeginQuery()
  {
    Array values;
    if (AddNull)
      values = GetValuesWithNull(ObjectType);
    else
      values = GetValues(ObjectType);
    base.OnQueryFinished(values);
  }

  /// <summary>
  /// Gets values array
  /// </summary>
  /// <param name="enumtype"></param>
  /// <returns></returns>
  public Array GetValues(Type enumtype)
  {
    Array values = enumtype.GetEnumValues();
    if (ItemType == null)
      return values;
    var constructor0 = ItemType.GetConstructor(new Type[] { });
    var constructor1 = ItemType.GetConstructor(new Type[] { ObjectType });
    var result = new List<object>();
    foreach (var value in values)
    {
      if (constructor1 != null)
        result.Add(constructor1.Invoke(new object[] { value }));
      else
      if (constructor0 != null)
        result.Add(constructor0.Invoke(new object[] { }));
    }
    return result.ToArray();
  }

  /// <summary>
  /// Gets value array with null value.
  /// </summary>
  /// <param name="enumType"></param>
  /// <returns></returns>
  public Array GetValuesWithNull(Type enumType)
  {
    Array values = GetValues(enumType);
    Array result = Array.CreateInstance(typeof(object), values.Length + 1);
    result.SetValue(null, 0);
    values.CopyTo(result, 1);
    return result;
  }

  /// <summary>
  /// Get names of the values.
  /// </summary>
  /// <param name="enumtype"></param>
  /// <param name="culture"></param>
  /// <returns></returns>
  public string[] GetNames(Type enumtype, CultureInfo? culture = null)
  {
    var typeConverterAttribute = enumtype.GetCustomAttributes(false)
      .FirstOrDefault(item => item is TypeConverterAttribute) as TypeConverterAttribute;
    if (typeConverterAttribute != null)
    {
      var converterTypeName = typeConverterAttribute.ConverterTypeName;
      if (converterTypeName != null)
      {
        var converterType = Type.GetType(converterTypeName);
        if (converterType != null)
        {
          TypeConverter? converter = (TypeConverter?)converterType.GetConstructor(new Type[0])?.Invoke(new object[0]);
          if (converter != null)
          {
            if (converter.CanConvertTo(typeof(string)))
            {
              Array values = enumtype.GetEnumValues();
              string[] names = new string[values.Length];
              for (int i = 0; i < values.Length; i++)
              {
                var name = (string?)converter.ConvertTo(values.GetValue(i), typeof(string));
                if (name!=null)
                  names[i] = name;
              }
              return names;
            }
          }
        }
      }
    }
    string[] result = enumtype.GetEnumNames();
    return result;
  }

}
