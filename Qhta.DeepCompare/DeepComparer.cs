using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Qhta.TypeUtils;

namespace Qhta.DeepCompare;

/// <summary>
/// Comparer class
/// </summary>
public static class DeepComparer
{

  /// <summary>
  /// Gets the type of the object and if the type is Nullable, returns its baseType
  /// </summary>
  public static Type GetNotNullableType(this object obj)
  {
    Type type = obj.GetType() ?? typeof(object);
    if (type.IsNullable(out var baseType) && baseType != null && baseType != type)
      type = baseType;
    return type;
  }

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
      var refType = refObject.GetNotNullableType();
      var testType = refObject.GetNotNullableType();
      if (objName == null)
        objName = refType.Name ?? testType.Name;
      if (refType != testType)
      {
        diffs?.Add(objName, propName ?? "Type", refType, refType);
        return false;
      }
      var ok = true;
      if (refType.FullName == "System.RuntimeType")
      {
        var cmp = refObject == testObject;
        if (!cmp == true)
        {
          diffs?.Add(objName, propName, testObject, refObject);
          ok = false;
        }
      }
      else
      if (refType == typeof(string))
      {
        var cmp = refObject.Equals(testObject);
        if (!cmp == true)
        {
          diffs?.Add(objName, propName, testObject, refObject);
          ok = false;
        }
      }
      else
      {
        var properties = refType.GetProperties().Where(item => item.GetIndexParameters().Count() == 0).ToArray();
        if (properties.Count() == 0 || refType.IsSimple())
        {
          if (refType == typeof(string) || !refType.IsClass)
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
            try
            {
              var cmp = (bool?)compareFunc.Invoke(refObject, new[] { testObject });
              if (!cmp == true)
              {
                diffs?.Add(objName ?? refType.Name, propName, testObject, refObject);
                ok = false;
              }
            }
            catch
            {
              Debug.WriteLine($"Error comparing two {refType} type values");
            }
          }
          else if (testObject is IStructuralEquatable iStructuralEquatable)
          {
            var cmp = iStructuralEquatable.Equals(refObject);
            if (!cmp == true)
            {
              diffs?.Add(objName, propName, testObject, refObject);
              ok = false;
            }
          }

        }
        else
        if (refType.BaseType?.Name.StartsWith("LinkedList") != true)
        {
          foreach (var prop in properties)
          {
            if (prop.GetCustomAttribute<NonComparableAttribute>() != null)
              continue;
            if (prop.Name == "FirstNode" || prop.Name == "LastNode")
              continue;
            var propType = prop.PropertyType;
            if (propType.IsNullable(out var baseType))
              propType = baseType;
            try
            {
              var testValue = prop.GetValue(testObject);
              var refValue = prop.GetValue(refObject);
              if (refValue != refObject && testValue != testObject)
                if (!IsEqual(testValue, refValue, diffs, Diff.Concat(objName, propName), prop.Name))
                  ok = false;
            }
            catch
            {
              Debug.WriteLine($"Error comparing two {refType}.{prop.Name} properties");
            }
          }
        }
        if (refType.IsEnumerable() && refType!=typeof(string))
        {
          var testEnumerator = (testObject as IEnumerable)?.GetEnumerator();
          var refEnumerator = (refObject as IEnumerable)?.GetEnumerator();
          if (testEnumerator != null && refEnumerator != null)
          {
            for (int i = 0; i < int.MaxValue; i++)
            {
              if (testEnumerator.MoveNext() && refEnumerator.MoveNext())
              {
                var testItem = testEnumerator.Current;
                var refItem = refEnumerator.Current;
                if (refItem != refObject && testItem != testObject)
                {
                  if (!IsEqual(testItem, refItem, diffs, Diff.Concat(objName, propName), $"[{i}]"))
                    ok = false;
                }
              }
              else
                break;
            }
          }
        }
      }
      return ok;
    }
    if (testObject == null && refObject != null)
    {
      diffs?.Add(refObject.GetType().Name, null, testObject, refObject);
      return false;
    }
    if (testObject != null && refObject == null)
    {
      diffs?.Add(testObject.GetType().Name, null, testObject, refObject);
      return false;
    }
    return true;

  }
}
