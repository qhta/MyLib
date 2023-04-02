namespace Qhta.DeepCompare;

/// <summary>
/// Data structure to count how many times was each property compared
/// </summary>
public class PropertyInfoCount
{
  /// <summary>
  /// Reflected property info
  /// </summary>
  public PropertyInfo Property { get; set; } = null!;
  
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
  /// <param name="index">Optional index of checked objects</param>
  public static bool IsEqual(object? testObject, object? refObject, DiffList? diffs = null, string? objName = null, string? propName = null, int? index = null)
  {
    if (testObject != null && refObject != null)
    {
      var refType = refObject.GetNotNullableType();
      var testType = refObject.GetNotNullableType();
      if (objName == null)
        objName = refType.Name ?? testType.Name;
      if (propName!=null)
        objName += "."+propName;
      if (index != null)
        objName += $"[{index}]";
      if (refType != testType)
      {
        diffs?.Add(objName, refType, refType);
        return false;
      }
      var ok = testObject.GetHashCode()==refObject.GetHashCode();
      if (ok)
        return true;
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
      else
      if (refType == typeof(String))
      {
        var cmp = String.Equals((string)refObject, (string)testObject);
        if (!cmp == true)
        {
          diffs?.Add(objName, testObject, refObject);
          ok = false;
        }
      }
      if (refType == typeof(Boolean))
      {
        var cmp = Boolean.Equals((Boolean)refObject, (Boolean)testObject);
        if (!cmp == true)
        {
          diffs?.Add(objName, testObject, refObject);
          ok = false;
        }
      }
      else
      if (refType == typeof(Int32))
      {
        var cmp = Int32.Equals((Int32)refObject, (Int32)testObject);
        if (!cmp == true)
        {
          diffs?.Add(objName, testObject, refObject);
          ok = false;
        }
      }
      else
      if (refType.IsEnum)
      {
        var cmp = Int32.Equals((Int32)refObject, (Int32)testObject);
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
          .Where(item =>
            item.DeclaringType?.Name.StartsWith("LinkedList") != true
            && item.GetIndexParameters().Count() == 0
            && item.GetCustomAttribute<NonComparableAttribute>() == null)
          .Select(item => new PropertyInfoCount(item)).ToArray();
          KnownProperties.Add(refType, properties);
        }
        if (properties.Count() == 0 || refType.IsSimple())
        {
          if (!refType.IsClass)
          {
            if (!KnownCompareFunctions.TryGetValue(refType, out var compareFunc))
            {
              var comparerType = typeof(Comparer<>).MakeGenericType(refType);
              if (comparerType == null)
                throw new InvalidOperationException($"Comparer type not found for {refType} type");
              var comparer = comparerType
                               .GetProperty("Default", BindingFlags.Public | BindingFlags.Static)?
                               .GetValue(null);
              if (comparer == null)
                throw new InvalidOperationException($"Default comparer not found for {refType} type");
              compareFunc = comparerType
                           .GetMethod("Equals", BindingFlags.Public | BindingFlags.Instance);
              if (compareFunc == null)
                throw new InvalidOperationException($"Equals func not found for {refType} type");
              KnownCompareFunctions.Add(refType, compareFunc);
            }
            try
            {
              var cmp = (bool?)compareFunc.Invoke(refObject, new[] { testObject });
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
              if (refValue != refObject && testValue != testObject 
                && (refValue != null || testValue != null))
              {
                if (!IsEqual(testValue, refValue, diffs, objName, prop.Name))
                  ok = false;
              }
            }
            catch
            {
              Debug.WriteLine($"Error comparing two {refType}.{prop.Name} properties");
            }
          }
        }
        if (refType.IsEnumerable())
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
                  if (!IsEqual(testItem, refItem, diffs, objName, null, i))
                    ok = false;
                }
              }
              else
                break;
            }
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
