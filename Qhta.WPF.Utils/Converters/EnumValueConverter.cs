using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

namespace Qhta.WPF.Utils
{
  /// <summary>
  /// Konwerter wartości wyliczeniowej.  
  /// Zamienia wartość wyliczeniową na liczbową zgodnie z podaną listą wartości.
  /// Umożliwia wyświetlanie listy wartości w językach narodowych
  /// </summary>
  public class EnumValueConverter : IValueConverter
  {
    /// <summary>
    /// Wartości wyliczeniowe. Mogą być ustawione w pliku XAML
    /// </summary>
    public List<EnumValue> EnumValues
    {
      get
      {
        if (enumValues == null)
          enumValues = new List<EnumValue> ();
        return enumValues;
      }
    }
    private List<EnumValue> enumValues;

    /// <summary>
    /// Typ wyliczeniowy. Określa listę wartości wyliczeniowych
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
    /// Konwersja wprost - identyczna jak konwersja wstecz.
    /// Kierunek konwersji rozpoznawany przez parametr <c>targetType</c>
    /// </summary>
    public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        throw new ArgumentNullException ("value");
      if (targetType.FullName == EnumType.FullName)
        return ConvertToEnum(value, targetType, parameter, culture);
      else if (targetType == typeof(object))
        return ConvertFromEnum(value, typeof(EnumValue), parameter, culture);
      return ConvertFromEnum(value, targetType, parameter, culture);
    }

    /// <summary>
    /// Konwersja wstecz - identyczna jak konwersja wprost.
    /// Kierunek konwersji rozpoznawany przez parametr <c>targetType</c>
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      if (targetType.FullName==EnumType.FullName)
        return ConvertToEnum(value, targetType, parameter, culture);
      else
        return ConvertFromEnum(value, targetType, parameter, culture);
    }

    /// <summary>
    /// Konwersja łańcucha, wartości logicznej lub liczby na wartość wyliczeniową
    /// </summary>
    public object ConvertToEnum (object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (EnumValues.Count == 0)
        InitEnumValues();
      if (value is EnumValue)
      {
        object n = (value as EnumValue).Value;
        MethodInfo castMethod = this.GetType().GetMethod("Cast").MakeGenericMethod(targetType);
        object castedObject = castMethod.Invoke(null, new object[] { n });
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
      throw new InvalidOperationException (string.Format ("Invalid input value of type '{0}'", value.GetType ()));
    }

    /// <summary>
    /// Pomocnicza procedura do rzutowania na wybrany typ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="o"></param>
    /// <returns></returns>
    public static T Cast<T>(object o)
    {
      return (T)o;
    }

    /// <summary>
    /// Konwersja wartości wyliczeniowej na liczbową
    /// </summary>
    public object ConvertFromEnum (object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (EnumValues.Count == 0)
        InitEnumValues();
      if (targetType == typeof(IEnumerable))
        return EnumValues;
      if (value!=null)
      {
        int n = (int)value;
        EnumValue v = EnumValues.FirstOrDefault(item => (item.Value is int) ? (int)(item.Value)==(int)value : item.Value.Equals(value));
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
    /// Inicjacja listy wartości z typu wyliczeniowego
    /// </summary>
    private void InitEnumValues()
    {
      foreach (object enumValue in Enum.GetValues(EnumType).Cast<object>())
      {
        EnumValue newValue = new EnumValue { Name = enumValue.ToString(), Value = (int)enumValue };
        //if (NameTranslation != null)
        //{
        //  TextTranslateEventArgs args = new TextTranslateEventArgs(enumValue.ToString());
        //  NameTranslation(newValue, args);
        //  newValue.Name = args.NewValue;
        //}
        EnumValues.Add(newValue);
      }
    }

    /// <summary>
    /// Zdarzenie tłumaczenia nazw - możliwe do wykorzystania, 
    /// gdy nie chcemy oryginalnych nazw z typu wyliczeniowego.
    /// </summary>
    //public event EventHandler<TextTranslateEventArgs> NameTranslation;

  }
}
