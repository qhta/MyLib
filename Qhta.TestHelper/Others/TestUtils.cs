using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Qhta.TestHelper;

public static class TestTools
{
  public static string? MethodName([CallerMemberName] string? callerName = null)
  { 
    return callerName;
  }

  public static void Stop([CallerMemberName] string? callerName = null)
  { }

  public static TraceTextWriter? TraceWriter { get; set; }

  public static void WriteLine(string str)
  {
    if (TraceWriter == null)
      TraceWriter = new TraceTextWriter(false, true);
    TraceWriter.WriteLine(str);
  }

  public static List<PropertyInfo> GetPropsToTest (Type type)
  {
    List<PropertyInfo> props = new();
    foreach (var prop in type.GetProperties ()) 
    {
      var propType = prop.PropertyType;
      if (propType.Name.StartsWith("Nullable`1"))
      {
        if (propType.Name.StartsWith("Nullable`1"))
          propType = propType.GetGenericArguments()[0];
        if (propType==typeof(string) || propType.IsValueType)
          props.Add (prop);
      }
    }
    return props;
  }

  public static Dictionary<string, int> GetPropsToTestCount(Type type)
    => GetPropsToTest(type).ToDictionary(prop => prop.Name, prop => 0);

  public static List<PropertyInfo> PropsFilled(object obj)
  {
    List<PropertyInfo> props = new();
    foreach (var prop in obj.GetType().GetProperties())
    {
      var propValue = prop.GetValue(obj);
      if  (propValue != null)
      {
        props.Add(prop);
      }
    }
    return props;

  }

}