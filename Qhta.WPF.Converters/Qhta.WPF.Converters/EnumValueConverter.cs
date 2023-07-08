using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

namespace Qhta.WPF.Converters
{
  /// <summary>
  /// Enumerated value converter.
  /// Converts an enumerated value to a numeric value according to the given list of values.
  /// Allows you to display a list of values in national languages.
  /// </summary>
  public class EnumValueConverter : IValueConverter
  {
    /// <summary>
    /// Enumerated values. They can be set in the XAML file.
    /// </summary>
    public List<EnumValue> EnumValues
    {
      get
      {
        if (enumValues == null)
          enumValues = new List<EnumValue>();
        return enumValues;
      }
    }
    private List<EnumValue> enumValues = null!;

    /// <summary>
    /// Enumeration type. Specifies a list of enumerated values.
    /// </summary>
    public Type EnumType
    {
      get { return _Type; }
      set
      {
        if (value.IsEnum)
        {
          _Type = value;
          //EnumValues.Clear();
          //EnumValues.AddRange(Enum.GetValues(value).Cast<object>());
        }
      }
    }
    private Type _Type = typeof(object);

    /// <summary>
    /// Direct conversion - same as reverse conversion.
    /// Conversion direction recognized by the <c>targetType</c> parameter
    /// </summary>
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      if (targetType.FullName == EnumType.FullName)
        return ConvertToEnum(value, targetType, parameter, culture);
      else if (targetType == typeof(object))
        return ConvertFromEnum(value, typeof(EnumValue), parameter, culture);
      return ConvertFromEnum(value, targetType, parameter, culture);
    }

    /// <summary>
    /// Backward conversion - same as direct conversion.
    /// Conversion direction recognized by the <c>targetType</c> parameter
    /// </summary>
    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      if (targetType.FullName == EnumType.FullName)
        return ConvertToEnum(value, targetType, parameter, culture);
      else
        return ConvertFromEnum(value, targetType, parameter, culture);
    }

    /// <summary>
    /// Convert a string, boolean, or number to an enumeration.
    /// </summary>
    public object? ConvertToEnum(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (EnumValues.Count == 0)
        InitEnumValues();
      if (value is EnumValue)
      {
        object? n = ((EnumValue)value).Value;
        MethodInfo? castMethod = this.GetType()?.GetMethod("Cast")?.MakeGenericMethod(targetType);
        object? castedObject = null;
        if (castMethod != null && n!=null)
          castedObject = castMethod.Invoke(null, new object[] { n });
        return castedObject;
      }
      else if (value is bool)
        return this.EnumValues.ElementAtOrDefault(System.Convert.ToByte(value));
      else if (value is byte)
        return this.EnumValues.ElementAtOrDefault(System.Convert.ToByte(value));
      else if (value is short)
        return this.EnumValues.ElementAtOrDefault(System.Convert.ToInt16(value));
      else if (value is int)
        return this.EnumValues.ElementAtOrDefault(System.Convert.ToInt32(value));
      else if (value is long)
        return this.EnumValues.ElementAtOrDefault(System.Convert.ToInt32(value));
      else if (value is Enum)
        return this.EnumValues.ElementAtOrDefault(System.Convert.ToInt32(value));
      throw new InvalidOperationException(string.Format("Invalid input value of type '{0}'", value.GetType()));
    }

    /// <summary>
    /// Auxiliary procedure to cast to the selected type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="o"></param>
    /// <returns></returns>
    public static T Cast<T>(object o)
    {
      return (T)o;
    }

    /// <summary>
    /// Converting an enumerated value to a numeric value.
    /// </summary>
    public object? ConvertFromEnum(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (EnumValues.Count == 0)
        InitEnumValues();
      if (targetType == typeof(IEnumerable))
        return EnumValues;
      if (value != null)
      {
        int n = (int)value;
        EnumValue? v = EnumValues.FirstOrDefault(item => (item.Value is int) ? (int)(item.Value) == (int)value : item.Value.Equals(value));
        if (targetType == typeof(EnumValue))
          return v;
        if (v != null)
          return v.Name;
        else if (value != null)
          return value.ToString();
      }
      return null;
    }

    /// <summary>
    /// Initialization of a list of values from an enumeration type.
    /// </summary>
    private void InitEnumValues()
    {
      foreach (object enumValue in Enum.GetValues(EnumType).Cast<object>())
      {
        EnumValue newValue = new EnumValue { Name = enumValue.ToString()??"", Value = (int)enumValue };
        //if (NameTranslation != null)
        //{
        //  TextTranslateEventArgs args = new TextTranslateEventArgs(enumValue.ToString());
        //  NameTranslation(newValue, args);
        //  newValue.Name = args.NewValue;
        //}
        EnumValues.Add(newValue);
      }
    }

    ///// <summary>
    ///// Name translation event - usable,
    ///// when we don't want the original names from the enum type.
    ///// </summary>
    //public event EventHandler<TextTranslateEventArgs> NameTranslation;

  }
}
