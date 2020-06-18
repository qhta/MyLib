using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Qhta.WPF.Utils
{
  public class EnumValuesProvider: ObjectDataProvider
  {
    public Array GetValues(Type enumtype)
    {
      Array result = enumtype.GetEnumValues();
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
