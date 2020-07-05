using System;
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

    public Array GetValues(Type enumType)
    {
      Array result = enumType.GetEnumValues();
      return result;
    }

    public Array GetValuesWithNull(Type enumType)
    {
      Array values = enumType.GetEnumValues();
      Array result = Array.CreateInstance(typeof(object), values.Length + 1);
      result.SetValue(null, 0);
      values.CopyTo(result, 1);
      return result;
    }

    public string[] GetNames(Type enumtype, CultureInfo culture = null)
    {
      var typeConverterAttribute = enumtype.GetCustomAttributes(false)
        .FirstOrDefault(item => item is TypeConverterAttribute) as TypeConverterAttribute;
      if (typeConverterAttribute!=null)
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
