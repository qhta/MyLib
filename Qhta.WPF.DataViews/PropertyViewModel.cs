using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using Qhta.MVVM;

namespace Qhta.WPF.DataViews
{
  public class PropertyViewModel: VisibleViewModel<PropertyInfo>
  {
    public PropertyViewModel(PropertyInfo model)
    {
      Model = model;
      var typeConverterName = model.GetCustomAttribute<TypeConverterAttribute>()?.ConverterTypeName;
      if (typeConverterName==null)
        typeConverterName = Type.GetCustomAttribute<TypeConverterAttribute>()?.ConverterTypeName;
      if (typeConverterName!=null)
      {
        //Debug.WriteLine(typeConverterName);
        var valueConverterType = Type.GetType(typeConverterName);
        if (valueConverterType!=null)
        {
          var valueConverter = valueConverterType.GetConstructor(new Type[0]).Invoke(new object[0]);
          ValueConverter = valueConverter as IValueConverter;
          ValueTypeConverter = valueConverter as TypeConverter;
        }
      }
      if (ValueConverter==null && ValueTypeConverter==null && Type!=typeof(string))
      {
        if (Type.IsEnum)
          ValueTypeConverter = new EnumConverter(Type);
        else
        if (Type == typeof(Boolean))
          ValueTypeConverter = new BooleanConverter();
        else
        if (Type == typeof(SByte))
          ValueTypeConverter = new SByteConverter();
        else
        if (Type == typeof(Int16))
          ValueTypeConverter = new Int16Converter();
        else
        if (Type == typeof(Int32))
          ValueTypeConverter = new Int32Converter();
        else
        if (Type == typeof(Int64))
          ValueTypeConverter = new Int64Converter();
        else
        if (Type == typeof(Byte))
          ValueTypeConverter = new ByteConverter();
        else
        if (Type == typeof(UInt16))
          ValueTypeConverter = new UInt16Converter();
        else
        if (Type == typeof(UInt32))
          ValueTypeConverter = new UInt32Converter();
        else
        if (Type == typeof(UInt64))
          ValueTypeConverter = new UInt64Converter();
        else
        if (Type == typeof(Double))
          ValueTypeConverter = new DoubleConverter();
        else
        if (Type == typeof(Single))
          ValueTypeConverter = new SingleConverter();
        else
        if (Type == typeof(Decimal))
          ValueTypeConverter = new DecimalConverter();
        else
        if (Type == typeof(DateTime))
          ValueTypeConverter = new DateTimeConverter();
        else
        if (Type == typeof(TimeSpan))
          ValueTypeConverter = new TimeSpanConverter();
      }
    }

    public string Name => Model.Name;
    public Type Type => Model.PropertyType;
    public object Instance { get; set; }
    public object Value
    {
      get
      {
        var value = Model.GetValue(Instance);
        if (ValueConverter!=null)
          value = ValueConverter.ConvertBack(value, Type, ConverterParameter, CultureInfo.CurrentUICulture);
        if (ValueTypeConverter!=null)
          if (ValueTypeConverter.CanConvertTo(typeof(string)))
            value = ValueTypeConverter.ConvertTo(value, typeof(string));
        return value;
      }
      set
      {
        if (ValueConverter!=null)
          value = ValueConverter.Convert(value, Type, ConverterParameter, CultureInfo.CurrentUICulture);
        if (ValueTypeConverter!=null)
          if (ValueTypeConverter.CanConvertFrom(typeof(string)))
            value = ValueTypeConverter.ConvertFrom(value);
        Model.SetValue(Instance, value);
      }
    }

    public IValueConverter ValueConverter { get; set; }

    public TypeConverter ValueTypeConverter { get; set; }

    public object ConverterParameter { get; set; }

    public IEnumerable<string> ItemsSource
    {
      get
      {
        return Type.GetEnumNames();
      }
    }
  }
}
