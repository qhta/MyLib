using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;

namespace Qhta.WPF.Utils
{
  public class EnumValuesProvider : DataSourceProvider
  {
    public Type ObjectType { get; set; }

    public Type ItemType { get; set; }


    public bool AddNull { get; set; }


    protected override void BeginQuery()
    {
      Array values;
      if (AddNull)
        values = GetValuesWithNull(ObjectType);
      else
        values = GetValues(ObjectType);
      base.OnQueryFinished(values);
    }

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

    public Array GetValuesWithNull(Type enumType)
    {
      Array values = GetValues(enumType);
      Array result = Array.CreateInstance(typeof(object), values.Length + 1);
      result.SetValue(null, 0);
      values.CopyTo(result, 1);
      return result;
    }

    public string[] GetNames(Type enumtype, CultureInfo culture = null)
    {
      var typeConverterAttribute = enumtype.GetCustomAttributes(false)
        .FirstOrDefault(item => item is TypeConverterAttribute) as TypeConverterAttribute;
      if (typeConverterAttribute != null)
      {
        var converterTypeName = typeConverterAttribute.ConverterTypeName;
        if (converterTypeName != null)
        {
          var converterType = Type.GetType(converterTypeName);
          TypeConverter converter = (TypeConverter)converterType.GetConstructor(new Type[0]).Invoke(new object[0]);
          if (converter.CanConvertTo(typeof(string)))
          {
            Array values = enumtype.GetEnumValues();
            string[] names = new string[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
              var name = (string)converter.ConvertTo(values.GetValue(i), typeof(string));
              names[i] = name;
            }
            return names;
          }
        }
      }
      string[] result = enumtype.GetEnumNames();
      return result;
    }

  }
}
