using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Qhta.TypeUtils;

namespace Qhta.DeepCompare;

/// <summary>
/// Comparer class
/// </summary>
public static class DeepComparer
{
  /// <summary>
  /// Determines whether the specified object is equal to other one.
  /// </summary>
  /// <param name="testObject">First object to compare (tested object)</param>
  /// <param name="refObject">Second object to compare (referenced object)</param>
  /// <param name="diffs">Optional differences list (to fill)</param>
  /// <param name="objName">Optional tested object name</param>
  /// <param name="propName">Optional tested property name</param>
  public static bool IsEqual(object? testObject, object? refObject, DiffList? diffs = null, string? objName = null, string? propName = null)
  {
    if (testObject != null && refObject != null)
    {
      var testType = testObject.GetType();
      var refType = refObject.GetType();
      if (testType != refType)
      {
        diffs?.Add(testType.Name, "Type", refType, testType);
        return false;
      }
      var properties = refType.GetProperties().Where(item=>item.GetIndexParameters().Count()==0).ToArray();
      if (properties.Count() == 0)
      {
        var comparerType = typeof(Comparer<>).MakeGenericType(refType);
        if (comparerType == null)
          throw new InvalidOperationException($"Comparer type not found for {refType} type");
        var comparer = comparerType
                         .GetProperty("Default", BindingFlags.Public | BindingFlags.Static)?
                         .GetValue(null);
        if (comparer == null)
          throw new InvalidOperationException($"Default comparer not found for {refType} type");
        var compareFunc = comparerType
                     .GetMethod("Equals", BindingFlags.Public | BindingFlags.Instance);
        if (compareFunc == null)
          throw new InvalidOperationException($"Equals func not found for {refType} type");
        var cmp = (bool?)compareFunc.Invoke(refObject, new[] { testObject });
        if (cmp == true) return true;
        diffs?.Add(objName ?? refType.Name, propName, refObject, testObject);
        return false;
      }
      var ok = true;
      foreach (var prop in properties)
      {
        var propType = prop.PropertyType;
        if (propType.IsNullable(out var baseType))
          propType = baseType;
        var testValue = prop.GetValue(testObject);
        var refValue = prop.GetValue(refObject);
        if (!IsEqual(refValue, testValue, diffs, StringUtils.Concat2(objName, ".", propName)))
          ok = false;
      }
      return ok;
    }
    if (testObject == null && refObject != null)
    {
      diffs?.Add(refObject.GetType().Name, null, refObject, testObject);
      return false;
    }
    if (testObject != null && refObject == null)
    {
      diffs?.Add(testObject.GetType().Name, null, refObject, testObject);
      return false;
    }
    return true;
  }

  ///// <summary>
  ///// Determines whether the specified integer test value is equal to referenced value.
  ///// </summary>
  ///// <param name="testValue">The test value.</param>
  ///// <param name="refValue">The reference value.</param>
  ///// <param name="diffs">The diffs.</param>
  ///// <param name="objName">Name of the object.</param>
  ///// <param name="propName">Name of the property.</param>
  ///// <returns>
  /////   <c>true</c> if the specified test value is equal; otherwise, <c>false</c>.
  ///// </returns>
  //public static bool IsEqual(int? testValue, int? refValue, DiffList? diffs = null, string? objName = null, string? propName = null)
  //{
  //  if (testValue!=null && refValue!=null)
  //  {
  //    if ((int)testValue == (int)refValue) return true;
  //    diffs?.Add(objName, propName, testValue, refValue);
  //    return false;
  //  }
  //  if (testValue == null && refValue != null)
  //  {
  //    diffs?.Add(refValue.GetType().Name, null, refValue, testValue);
  //    return false;
  //  }
  //  if (testValue != null && refValue == null)
  //  {
  //    diffs?.Add(testValue.GetType().Name, null, refValue, testValue);
  //    return false;
  //  }
  //  return true;
  //}
}
