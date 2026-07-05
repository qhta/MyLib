namespace Qhta.DeepCompare;

/// <summary>
/// Data structure to count how many times was each property compared
/// </summary>
public class PropertyInfoCount
{
  /// <summary>
  /// Reflected property info
  /// </summary>
  public PropertyInfo Property { get; set; }

  /// <summary>
  /// Count of compare
  /// </summary>
  public int Count { get; set; }

  /// <summary>
  /// Simplifies instance creation
  /// </summary>
  /// <param name="property"></param>
  public PropertyInfoCount(PropertyInfo property)
  {
    Property = property;
  }
}

/// <summary>
/// Comparer class
/// </summary>
public static class DeepComparer
{
  /// <summary>
  /// Used to speed up finding of compare functions
  /// </summary>
  public static Dictionary<Type, MethodInfo> KnownCompareFunctions = new();

  /// <summary>
  /// Used to speed up selection of properties to compare
  /// </summary>
  public static Dictionary<Type, PropertyInfoCount[]> KnownProperties = new();

  /// <summary>
  /// Gets the type of the object and if the type is Nullable, returns its baseType
  /// </summary>
  private static Type GetNotNullableType(this object obj)
  {
    Type type = obj.GetType() ?? typeof(object);
    if (type.IsNullable(out var baseType) && baseType != null && baseType != type)
      type = baseType;
    return type;
  }

  /// <summary>
  /// Determines whether the specified object is equal to other one.
  /// Compares all properties of the objects and returns true if they are equal, false otherwise.
  /// </summary>
  /// <param name="testObject">First object to compare (tested object)</param>
  /// <param name="refObject">Second object to compare (referenced object)</param>
  /// <returns>True if objects are equal, false otherwise </returns>
  public static bool DeepEquals(this object? testObject, object? refObject)
  {
    return IsEqual(testObject, refObject, null, null, null, null,new HashSet<(int, int)>());
  }

  /// <summary>
  /// Determines whether the specified object is equal to other one.
  /// </summary>
  /// <param name="testObject">First object to compare (tested object)</param>
  /// <param name="refObject">Second object to compare (referenced object)</param>
  /// <param name="diffs">Optional differences list (to fill)</param>
  /// <param name="objName">Optional tested object name</param>
  /// <param name="propName">Optional tested property name</param>
  /// <param name="index">Optional index of checked objects</param>
  /// <param name="visited">Visited pairs of compared objects</param>
  public static bool IsEqual
  (object? testObject, object? refObject, DiffList? diffs = null, string? objName = null, string? propName = null,
    int? index = null, HashSet<(int Left, int Right)>? visited = null)
  {
    visited ??= new HashSet<(int, int)>();


    var key = (RuntimeHelpers.GetHashCode(testObject), RuntimeHelpers.GetHashCode(refObject));
    if (!visited.Add(key))
      return true; // already compared this pair in this recursion tree


    if (testObject != null && refObject != null)
    {
      var refType = refObject.GetNotNullableType();
      var testType = testObject.GetNotNullableType();
      if (objName == null)
        objName = refType.Name ?? testType.Name;
      if (propName != null)
        objName += "." + propName;
      if (index != null)
        objName += $"[{index}]";
      if (refType != testType)
      {
        diffs?.Add(objName, testType, refType);
        return false;
      }
      var ok = testObject.GetHashCode() == refObject.GetHashCode();
      if (ok)
        return true;
      if (refType.Name == "Features")
        Debug.Assert(true);
      ok = true;
      if (refType.FullName == "System.RuntimeType")
      {
        var cmp = refObject == testObject;
        if (!cmp == true)
        {
          diffs?.Add(objName, testObject, refObject);
          ok = false;
        }
      }
      else if (refType == typeof(String))
      {
        var cmp = String.Equals((string)testObject, (string)refObject);
        if (!cmp == true)
        {
          diffs?.Add(objName, testObject, refObject);
          ok = false;
        }
      }
      else if (refType == typeof(Boolean))
      {
        var cmp = Boolean.Equals((Boolean)testObject, (Boolean)refObject);
        if (!cmp == true)
        {
          diffs?.Add(objName, testObject, refObject);
          ok = false;
        }
      }
      else if (refType == typeof(Int32))
      {
        var cmp = Int32.Equals((Int32)testObject, (Int32)refObject);
        if (!cmp == true)
        {
          diffs?.Add(objName, testObject, refObject);
          ok = false;
        }
      }
      else if (refType == typeof(UInt32))
      {
        var cmp = UInt32.Equals((UInt32)testObject, (UInt32)refObject);
        if (!cmp == true)
        {
          diffs?.Add(objName, testObject, refObject);
          ok = false;
        }
      }
      else if (refType == typeof(Byte))
      {
        var cmp = Byte.Equals((Byte)testObject, (Byte)refObject);
        if (!cmp == true)
        {
          diffs?.Add(objName, testObject, refObject);
          ok = false;
        }
      }
      else if (refType == typeof(SByte))
      {
        var cmp = SByte.Equals((SByte)testObject, (SByte)refObject);
        if (!cmp == true)
        {
          diffs?.Add(objName, testObject, refObject);
          ok = false;
        }
      }
      else if (refType == typeof(Int16))
      {
        var cmp = Int16.Equals((Int16)testObject, (Int16)refObject);
        if (!cmp == true)
        {
          diffs?.Add(objName, testObject, refObject);
          ok = false;
        }
      }
      else if (refType == typeof(UInt16))
      {
        var cmp = UInt16.Equals((UInt16)testObject, (UInt16)refObject);
        if (!cmp == true)
        {
          diffs?.Add(objName, testObject, refObject);
          ok = false;
        }
      }
      else if (refType == typeof(Int64))
      {
        var cmp = Int64.Equals((Int64)testObject, (Int64)refObject);
        if (!cmp == true)
        {
          diffs?.Add(objName, testObject, refObject);
          ok = false;
        }
      }
      else if (refType == typeof(UInt64))
      {
        var cmp = UInt64.Equals((UInt64)testObject, (UInt64)refObject);
        if (!cmp == true)
        {
          diffs?.Add(objName, testObject, refObject);
          ok = false;
        }
      }
      else if (refType == typeof(Double))
      {
        var cmp = Double.Equals((Double)testObject, (Double)refObject);
        if (!cmp == true)
        {
          diffs?.Add(objName, testObject, refObject);
          ok = false;
        }
      }
      else if (refType == typeof(Single))
      {
        var cmp = Single.Equals((Single)testObject, (Single)refObject);
        if (!cmp == true)
        {
          diffs?.Add(objName, testObject, refObject);
          ok = false;
        }
      }
      else if (refType == typeof(Decimal))
      {
        var cmp = Decimal.Equals((Decimal)testObject, (Decimal)refObject);
        if (!cmp == true)
        {
          diffs?.Add(objName, testObject, refObject);
          ok = false;
        }
      }
      else if (refType == typeof(DateTime))
      {
        var cmp = DateTime.Equals((DateTime)testObject, (DateTime)refObject);
        if (!cmp == true)
        {
          diffs?.Add(objName, testObject, refObject);
          ok = false;
        }
      }
      else if (refType == typeof(Guid))
      {
        var cmp = Guid.Equals((Guid)testObject, (Guid)refObject);
        if (!cmp == true)
        {
          diffs?.Add(objName, testObject, refObject);
          ok = false;
        }
      }
      else if (refType == typeof(Uri))
      {
        var cmp = Uri.Equals((Uri)testObject, (Uri)refObject);
        if (!cmp == true)
        {
          diffs?.Add(objName, testObject, refObject);
          ok = false;
        }
      }
      else if (refType == typeof(Char))
      {
        var cmp = Char.Equals((Char)testObject, (Char)refObject);
        if (!cmp == true)
        {
          diffs?.Add(objName, testObject, refObject);
          ok = false;
        }
      }
      else if (refType.IsEnum)
      {
        var cmp = Int32.Equals((Int32)testObject, (Int32)refObject);
        if (!cmp == true)
        {
          diffs?.Add(objName, testObject, refObject);
          ok = false;
        }
      }

      else
      {
        if (!KnownProperties.TryGetValue(refType, out var properties))
        {
          properties = refType.GetProperties()
            .Where(item => item.DeclaringType?.Name.StartsWith("LinkedList") != true &&
                           !item.GetIndexParameters().Any() &&
                           item.GetCustomAttribute<NonComparableAttribute>() == null)
            .Select(item => new PropertyInfoCount(item)).ToArray();
          KnownProperties.Add(refType, properties);
        }
        if (!properties.Any() || refType.IsSimple())
        {
          if (!refType.IsClass)
          {
            if (!KnownCompareFunctions.TryGetValue(refType, out var compareFunc))
            {
              var comparerType = typeof(Comparer<>).MakeGenericType(refType);
              if (comparerType == null)
                throw new InvalidOperationException($"Comparer type not found for {refType} type");

              var comparer = comparerType.GetProperty("Default", BindingFlags.Public | BindingFlags.Static)
                ?.GetValue(null);
              if (comparer == null)
                throw new InvalidOperationException($"Default comparer not found for {refType} type");

              compareFunc = comparerType.GetMethod("Equals", BindingFlags.Public | BindingFlags.Instance);
              if (compareFunc == null)
                throw new InvalidOperationException($"Equals func not found for {refType} type");

              KnownCompareFunctions.Add(refType, compareFunc);
            }
            try
            {
              var cmp = (bool?)compareFunc.Invoke(refObject, [testObject]);
              if (!cmp == true)
              {
                diffs?.Add(objName, testObject, refObject);
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
              diffs?.Add(objName, testObject, refObject);
              ok = false;
            }
          }
        }
        else
        {
          foreach (var propInfoCount in properties)
          {
            var prop = propInfoCount.Property;
            propInfoCount.Count += 1;
            var propType = prop.PropertyType;
            if (propType.IsNullable(out var baseType))
              propType = baseType;
            try
            {
              var testValue = prop.GetValue(testObject);
              var refValue = prop.GetValue(refObject);
              if (refValue != refObject && testValue != testObject && (refValue != null || testValue != null))
              {
                if (!IsEqual(testValue, refValue, diffs, objName, prop.Name, null, visited))
                  ok = false;
              }
            }
            catch
            {
              Debug.WriteLine($"Error comparing two {refType}.{prop.Name} properties");
            }
          }
        }
        bool result = true;
        if (refType.IsEnumerable(out var itemType) && testObject is IEnumerable obj1Enumerable
                                          && refObject is IEnumerable obj2Enumerable)
        {
          IEnumerator? enumerator1 = null;
          IEnumerator? enumerator2 = null;
          try
          {
            enumerator1 = obj1Enumerable.GetEnumerator();
            enumerator2 = obj2Enumerable.GetEnumerator();

            //int itemCount = 0;
            while (enumerator1.MoveNext() && enumerator2.MoveNext())
            {
              var item1 = enumerator1.Current;
              var item2 = enumerator2.Current;
              if (!IsEqual(item1, item2, diffs, objName, propName, index, visited))
              {
                result = false;
                break;
              }

              //itemCount++;
            }
            if (result)
            {
              if (enumerator1.MoveNext())
              {
                result = false;
              }
              else if (enumerator2.MoveNext())
              {
                result = false;
              }
            }

          } catch
          {
            Debug.WriteLine($"Error comparing two {refType} enumerable properties");
          }
          finally
          {
            (enumerator1 as IDisposable)?.Dispose();
            (enumerator2 as IDisposable)?.Dispose();
          }
        }
      }

      //if (!ok)
      //  Debugger.Break();
      return ok;
    }
    if (testObject == null && refObject != null)
    {
      diffs?.Add(objName, propName, index, testObject, refObject);
      return false;
    }
    if (testObject != null && refObject == null)
    {
      diffs?.Add(objName, propName, index, testObject, refObject);
      return false;
    }
    return true;
  }
}