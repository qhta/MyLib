using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.TypeUtils
{
  public class DynamicTypeConverter
  {
    private TypeConverter TypeConverter;
    private Type TargetType;

    public DynamicTypeConverter(TypeConverter typeConverter, Type targetType)
    {
      TypeConverter = typeConverter;
      TargetType = targetType;
    }

    public object Convert(object value)
    {
      return TypeConverter.ConvertFrom(value);
    }
  }
}
