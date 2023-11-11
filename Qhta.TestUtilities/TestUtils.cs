﻿using System.Collections.Generic;

namespace Qhta.TestUtilities;

/// <summary>
/// This static class contains extension operations to be used for test/debug application.
/// </summary>
public static class TestUtils
{
  /// <summary>
  /// Throws this error or its inner exception
  /// </summary>
  /// <param name="ex"></param>
  public static void ThrowError(this Exception ex)
  {
    if (ex.InnerException != null)
      ThrowError(ex.InnerException);
    throw ex;
  }

  /// <summary>
  /// Converts object to string which shows its content.
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? ToDumpString(this object? value)
  {
    if (value == DBNull.Value)
      return "DbNull";
    if (value is null)
      return null;
    if (value is string str)
      return "\"" + str + "\"";
    var type = value.GetType();
    if (type.IsArray)
    {
      var ss = new List<string>();
      Array array = (Array)value;
      var n = array.Length;
      for (int i = 0; i < System.Math.Min(n, 10); i++)
      {
        var val = array.GetValue(i);
        if (val != null)
          ss.Add($"{ToDumpString(val)}");
        else
          ss.Add("");
      }
      return type.Name + " { " + string.Join(", ", ss) + " }";
    }
    else
    if (type.IsClass)
    {
      var ss = new List<string>();
      foreach (var propInfo in type.GetProperties())
      {
        if (propInfo.GetCustomAttribute<DataMemberAttribute>() != null)
        {
          var getMethod = propInfo.GetGetMethod();
          if (getMethod != null)
          {
            var val = getMethod.Invoke(value, new object[] { });
            if (val != null)
              ss.Add($"{propInfo.Name} = {ToDumpString(val)}");
          }
        }
      }
      var enumerable = value as IEnumerable;
      if (enumerable != null)
      {
        foreach (var item in enumerable)
        {
          if (item != null)
            ss.Add($"{ToDumpString(item)}");
          else
            ss.Add("");
        }
      }
      var result = type.Name + " { " + string.Join(", ", ss) + " }";
      return result;
    }
    if (value is Single r4)
      return r4.ToString(CultureInfo.InvariantCulture);
    if (value is Double r8)
      return r8.ToString(CultureInfo.InvariantCulture);
    if (value is Decimal dm)
      return dm.ToString(CultureInfo.InvariantCulture);
    return value.ToString();
  }
}
