using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.TypeUtils
{
  public class StaticTypeConverter
  {
    public static object ConvertToString(object value, TypeConverter typeConverter, Type targetType)
    {
      object result = (string)typeConverter.ConvertTo(value, targetType);
      return result;
    }
  }
}
