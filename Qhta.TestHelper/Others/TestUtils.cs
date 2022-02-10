using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.TestHelper
{
  public static class TestUtils
  {
    public static string? MethodName([CallerMemberName] string? callerName = null)
    { 
      return callerName;
    }

    public static void Stop([CallerMemberName] string? callerName = null)
    { }

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
}
