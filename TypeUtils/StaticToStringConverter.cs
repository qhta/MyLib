using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.TypeUtils
{
  public static class StaticToStringConverter
  {
    public static string CharToString(Char value)
    {
      return value.ToString();
    }

    public static string BoolToString(bool value)
    {
      return value.ToString();
    }

    public static string SByteToString(SByte value)
    {
      return value.ToString();
    }

    public static string Int16ToString(Int16 value)
    {
      return value.ToString();
    }

    public static string Int32ToString(Int32 value)
    {
      return value.ToString();
    }

    public static string Int64ToString(Int64 value)
    {
      return value.ToString();
    }

    public static string ByteToString(Byte value)
    {
      return value.ToString();
    }

    public static string UInt16ToString(UInt16 value)
    {
      return value.ToString();
    }

    public static string UInt32ToString(UInt32 value)
    {
      return value.ToString();
    }

    public static string UInt64ToString(UInt64 value)
    {
      return value.ToString();
    }

    public static string FloatToString(float value)
    {
      return value.ToString();
    }

    public static string DoubleToString(double value)
    {
      return value.ToString();
    }

    public static string DateTimeToString(DateTime value)
    {
      return value.ToString();
    }

    public static string TimeSpanToString(TimeSpan value)
    {
      return value.ToString();
    }

    public static string EnumToString(object value)
    {
      if (value == null)
        return null;
      string result = value.ToString();
      result = result.Replace(", ", "+");
      return result;
    }

    public static string NullableCharToString(Char? value)
    {
      return value.ToString();
    }

    public static string NullableBoolToString(bool? value)
    {
      return value.ToString();
    }

    public static string NullableSByteToString(SByte? value)
    {
      return value.ToString();
    }

    public static string NullableInt16ToString(Int16? value)
    {
      return value.ToString();
    }

    public static string NullableInt32ToString(Int32? value)
    {
      return value.ToString();
    }

    public static string NullableInt64ToString(Int64? value)
    {
      return value.ToString();
    }

    public static string NullableByteToString(Byte? value)
    {
      return value.ToString();
    }

    public static string NullableUInt16ToString(UInt16? value)
    {
      return value.ToString();
    }

    public static string NullableUInt32ToString(UInt32? value)
    {
      return value.ToString();
    }

    public static string NullableUInt64ToString(UInt64? value)
    {
      return value.ToString();
    }

    public static string NullableFloatToString(float? value)
    {
      return value.ToString();
    }

    public static string NullableDoubleToString(double? value)
    {
      return value.ToString();
    }

    public static string NullableDateTimeToString(DateTime? value)
    {
      return value.ToString();
    }

    public static string NullableTimeSpanToString(TimeSpan? value)
    {
      return value.ToString();
    }

    public static string NullableEnumToString(object value)
    {
      if (value == null)
        return null;
      string result = value.ToString();
      result = result.Replace(", ", "+");
      return result;
    }

    public static string ClassToString(object value)
    {
      string result = value.ToString();
      return result;
    }
  }
}
