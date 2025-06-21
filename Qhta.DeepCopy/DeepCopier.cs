namespace Qhta.DeepCopy;
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
/// Copier class
/// </summary>
public static class DeepCopier
{
  /// <summary>
  /// Used to speed up finding of clone functions
  /// </summary>
  public static Dictionary<Type, MethodInfo> KnownCloneFunctions = new();
  /// <summary>
  /// Used to speed up selection of properties to compare
  /// </summary>
  public static Dictionary<Type, PropertyInfoCount[]> KnownProperties = new();

  /// <summary>
  /// Gets the type of the object and if the type is Nullable, returns its baseType
  /// </summary>
  public static Type GetNotNullableType(this object? obj)
  {
    Type type = obj?.GetType() ?? typeof(object);
    if (type.IsNullable(out var baseType) && baseType != type)
      type = baseType!;
    return type;
  }

  /// <summary>
  /// Creates a deep copy of the source object.
  /// </summary>
  /// <param name="sourceObject">First object to compare (tested object)</param>
  public static object? CopyFrom(object? sourceObject)
  {
    if (sourceObject == null)
      return null;
    if (sourceObject is string str)
      return str;
    if (sourceObject is Boolean bval)
      return bval;
    if (sourceObject is Enum enumVal)
      return enumVal;
    if (sourceObject is Int32 intVal)
      return intVal;
    if (sourceObject is UInt32 uintVal)
      return uintVal;
    if (sourceObject is Int64 int64Val)
      return int64Val;
    if (sourceObject is UInt64 uint64Val)
      return uint64Val;
    if (sourceObject is byte byteVal)
      return byteVal;
    if (sourceObject is sbyte sByteVal)
      return sByteVal;
    if (sourceObject is Int16 int16Val)
      return int16Val;
    if (sourceObject is UInt64 uint16Val)
      return uint16Val;
    if (sourceObject is float floatVal)
      return floatVal;
    if (sourceObject is double doubleVal)
      return doubleVal;
    if (sourceObject is decimal decimalVal)
      return decimalVal;
    if (sourceObject is DateTime dateTimeVal)
      return dateTimeVal;
    if (sourceObject is TimeSpan timeSpanVal)
      return timeSpanVal;
#if NET6_0_OR_GREATER
    if (sourceObject is DateOnly dateOnlyVal)
      return dateOnlyVal;
    if (sourceObject is TimeOnly timeOnlyVal)
      return timeOnlyVal;
#endif
    var sourceType = sourceObject.GetNotNullableType();
    if (sourceType.IsSimple())
      throw new InvalidOperationException($"Simple type {sourceType} can't be copied");
    var copyingConstructor = sourceType.GetConstructor(new Type[] { sourceType });
    if (copyingConstructor != null)
      return copyingConstructor.Invoke(null, new object[] { sourceObject });

    var defaultConstructor = sourceType.GetConstructor(new Type[] { });
    if (defaultConstructor == null)
      throw new InvalidOperationException($"{sourceType} has no default constructor");

    var targetObject = defaultConstructor.Invoke(new object[] { });
    if (!KnownProperties.TryGetValue(sourceType, out var properties))
    {
      properties = sourceType.GetProperties()
     .Where(item =>
       item.DeclaringType?.Name.StartsWith("LinkedList") != true
       && !item.GetIndexParameters().Any()
       && item.GetCustomAttribute<NonComparableAttribute>() == null)
     .Select(item => new PropertyInfoCount(item)).ToArray();
      KnownProperties.Add(sourceType, properties);
    }
    foreach (var propInfoCount in properties)
    {
      var prop = propInfoCount.Property;
      var propType = prop.PropertyType.GetNotNullableType();
      var sourceValue = prop.GetValue(sourceObject);
      try
      {
        if (prop.CanWrite && !propType.IsCollection())
        {
          var newValue = CopyFrom(sourceValue);
          prop.SetValue(targetObject, newValue);
        }
        else
        {
          var oldValue = prop.GetValue(targetObject);
          CopyDeep(oldValue, sourceValue);
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Can't copy property {prop.Name}. {ex.Message}");
      }
    }
    if (sourceObject is IEnumerable sourceCollection && sourceType.IsCollection(out var itemType))
    {
      var collectionType = typeof(ICollection<>).MakeGenericType(itemType);
      var addMethod = collectionType.GetMethod("Add");
      if (addMethod != null)
        foreach (var item in sourceCollection)
        {
          var newItem = CopyFrom(item);
          addMethod.Invoke(targetObject, new object?[]{ newItem });
        }
    }
    return targetObject;
  }

  /// <summary>
  /// Copies properties from source object to target object.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="sourceObject"></param>
  /// <param name="targetObject"></param>
  /// <exception cref="ArgumentNullException"></exception>
  /// <exception cref="ArgumentException"></exception>
  public static void CopyDeep<T>(T? targetObject, T? sourceObject)
  {
    if (sourceObject == null || targetObject == null)
      throw new ArgumentNullException(nameof(sourceObject), "Source or target object is null");
    if (sourceObject.GetType() != targetObject.GetType())
      throw new ArgumentException("Source and target objects must be of the same type");
    var sourceType = sourceObject.GetNotNullableType();
    if (!KnownProperties.TryGetValue(sourceType, out var properties))
    {
      properties = sourceType.GetProperties()
     .Where(item =>
       item.DeclaringType?.Name.StartsWith("LinkedList") != true
       && !item.GetIndexParameters().Any()
       && item.GetCustomAttribute<NonComparableAttribute>() == null)
     .Select(item => new PropertyInfoCount(item)).ToArray();
      KnownProperties.Add(sourceType, properties);
    }
    foreach (var propInfoCount in properties)
    {
      var prop = propInfoCount.Property;
      var propType = prop.PropertyType.GetNotNullableType();

      var sourceValue = prop.GetValue(sourceObject);
      try
      {
        if (prop.CanWrite && !propType.IsCollection())
        {
          var newValue = CopyFrom(sourceValue);
          prop.SetValue(targetObject, newValue);
        }
        else
        {
          var oldValue = prop.GetValue(targetObject);
          CopyDeep(oldValue, sourceValue);
        }
      } catch (Exception ex)
      {
        Debug.WriteLine($"Can't copy property {prop.Name}. {ex.Message}");
      }
    }
  }
}

