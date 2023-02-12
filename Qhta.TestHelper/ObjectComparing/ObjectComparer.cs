using System;
using System.Linq;
using System.Reflection;

using Microsoft.VisualBasic;

namespace Qhta.TestHelper;

/// <summary>
/// A class to compare objects and write output to TraceTextWriter
/// </summary>
public class ObjectComparer
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public ObjectComparer(ITraceTextWriter? writer)
  {
    Writer = writer;
  }

  /// <summary>
  /// Writer to receive detailed results of comparison. Set up on comparer constructor.
  /// </summary>
  public ITraceTextWriter? Writer { get; init; }


  public bool DeepCompare(object? obj1, object? obj2)
  {
    if (object.ReferenceEquals(obj1, obj2))
      return true;
    if (obj1 == null)
    {
      Writer?.WriteLine($"Left object is null");
      return false;
    }
    if (obj2 == null)
    {
      Writer?.WriteLine($"Right object is null");
      return false;
    }
    if (obj1.GetType() != obj2.GetType())
    {
      Writer?.WriteLine($"Types are different. first is {obj1.GetType()}   second = {obj2.GetType()}");
      return false;
    }
    var aType = obj1.GetType();
    if (aType.IsValueType)
    {
      if (obj1 is bool bool1 && obj2 is bool bool2)
        return bool1 == bool2;
      if (obj1 is string str1 && obj2 is string str2)
        return str1 == str2;
      if (obj1 is int int1 && obj2 is int int2)
        return int1 == int2;
      if (obj1 is long long1 && obj2 is long long2)
        return long1 == long2;
      if (obj1 is short short1 && obj2 is short short2)
        return short1 == short2;
      if (obj1 is sbyte sbyte1 && obj2 is sbyte sbyte2)
        return sbyte1 == sbyte2;
      if (obj1 is uint uint1 && obj2 is uint uint2)
        return uint1 == uint2;
      if (obj1 is ulong ulong1 && obj2 is ulong ulong2)
        return ulong1 == ulong2;
      if (obj1 is ushort ushort1 && obj2 is ushort ushort2)
        return ushort1 == ushort2;
      if (obj1 is byte byte1 && obj2 is byte byte2)
        return byte1 == byte2;
      if (obj1 is float float1 && obj2 is float float2)
        return float1 == float2;
      if (obj1 is double double1 && obj2 is double double2)
        return double1 == double2;
      if (obj1 is decimal decimal1 && obj2 is decimal decimal2)
        return decimal1 == decimal2;
      if (obj1 is DateTime dateTime1 && obj2 is DateTime dateTime2)
        return dateTime1 == dateTime2;
      if (obj1 is TimeSpan timespan1 && obj2 is TimeSpan timespan2)
        return timespan1 == timespan2;
      if (obj1 is DateOnly dateonly1 && obj2 is DateOnly dateonly2)
        return dateonly1 == dateonly2;
      if (obj1 is TimeOnly timeonly1 && obj2 is TimeOnly timeonly2)
        return timeonly1 == timeonly2;
      if (aType.IsEnum)
        return Enum.Equals(obj1, obj2);
      throw new InvalidOperationException($"Unhandled type {aType}");
    }
    foreach (var fieldInfo in aType.GetFields())
    {
      var field1 = fieldInfo.GetValue(obj1);
      var field2 = fieldInfo.GetValue(obj2);
      if (field1 == obj1 && field1 == obj2)
        continue;
      if (!DeepCompare(field1, field2))
      {
        Writer?.WriteLine($"Fields {fieldInfo.Name} are different. first = {field1}   second = {field2}");
        return false;
      }
    }
    foreach (var propInfo in aType.GetProperties())
    {
      if (propInfo.GetIndexParameters().Any())
        continue;
      var prop1 = propInfo.GetValue(obj1);
      var prop2 = propInfo.GetValue(obj2);
      if (prop1==obj1 && prop2==obj2)
        continue;
      if (!DeepCompare(prop1, prop2))
      {
        Writer?.WriteLine($"Properties {propInfo.Name} are different. first = {prop1}   second = {prop2}");
        return false;
      }
    }
    return true;
  }
}