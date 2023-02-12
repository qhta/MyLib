using System;
using System.Collections.Generic;
using System.Reflection;

namespace Qhta.TestHelper;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class PropsCountTest
{
  record ChangeableKeyValuePair<KeyType, ValueType>
  {
    public ChangeableKeyValuePair(KeyType key, ValueType value)
    {
      Key = key;
      Value = value;
    }

    public KeyType Key;
    public ValueType Value;
  }

  public PropsCountTest(Type type)
  {
    Type = type;
    Properties = new();
    if (type.Name.StartsWith("KeyValuePair`"))
    {
      type = type.GetGenericArguments()[1];
    }
    foreach (var prop in type.GetProperties())
    {
      var propType = prop.PropertyType;
      {
        if (propType.Name.StartsWith("Nullable`1"))
          propType = propType.GetGenericArguments()[0];
        if (propType == typeof(string) || propType.IsValueType)
          Properties.Add(new(prop, 0));
      }
    }
  }

  public Type Type { get; init; }

  List<ChangeableKeyValuePair<PropertyInfo, int>> Properties { get; init; }

  public void CountFilledProps(object obj)
  {
    foreach (var item in Properties)
    {
      var prop = item.Key;
      var propValue = prop.GetValue(obj);
      if (propValue != null)
        item.Value += 1;
    }
  }

  public List<string> GetEmptyProps()
  {
    List<string> result = new();
    foreach (var item in Properties)
    {
      if (item.Value == 0)
        result.Add(item.Key.Name);
    }
    return result;
  }

}