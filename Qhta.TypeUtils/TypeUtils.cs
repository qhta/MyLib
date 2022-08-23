using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Qhta.TypeUtils
{
  /// <summary>
  /// Helper functions that operate on types and supplement <c>System.Reflection</c> library
  /// </summary>
  public static class TypeUtils
  {
    /// <summary>
    /// When a class defines a new method with the same name as an inherited method,
    /// a "GetMethod" function return an error. 
    /// This "GetTopmostMethod" method searches the original class first 
    /// and if it will not find a method, then searches the base class recursively.
    /// </summary>
    /// <param name="aType"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static MethodInfo GetTopmostMethod(this Type aType, string methodName)
    {
      var bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
      var methodInfo = aType.GetMethod(methodName, bindingFlags);
      if (methodInfo == null && aType.BaseType!=null)
        methodInfo = GetTopmostMethod(aType.BaseType, methodName);
      return methodInfo;
    }

    /// <summary>
    /// Checks if the given value is the default value of the given type
    /// </summary>
    /// <param name="valueType"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsDefaultValue(this Type valueType, object value)
    {
      if (valueType.Name.StartsWith("Nullable`1"))
      {
        var argTypes = valueType.GenericTypeArguments;
        if (argTypes.Length == 1)
          valueType = argTypes[0];
      }
      var isDefault = false;
      if (valueType == typeof(bool))
        isDefault = (bool)value == false;
      else if (valueType.IsEnum)
      {
        var underlyingType = valueType.GetEnumUnderlyingType();
        if (underlyingType == typeof(byte))
          isDefault = (byte)value == 0;
        else
        if (underlyingType == typeof(SByte))
          isDefault = (SByte)value == 0;
        else
        if (underlyingType == typeof(Int16))
          isDefault = (Int16)value == 0;
        else
        if (underlyingType == typeof(UInt16))
          isDefault = (UInt16)value == 0;
        else
        if (underlyingType == typeof(UInt32))
          isDefault = (UInt32)value == 0;
        else
        if (underlyingType == typeof(Int64))
          isDefault = (Int64)value == 0;
        else
        if (underlyingType == typeof(UInt64))
          isDefault = (UInt64)value == 0;
        else
          isDefault = (int)value == 0;
      }
      else if (valueType == typeof(Double))
        isDefault = Double.IsNaN((double)value);
      else if (valueType == typeof(Single))
        isDefault = Double.IsNaN((Single)value);
      return isDefault;
    }

    /// <summary>
    /// Converts string to enum value for an enum type. Returns false if no enum value recognized
    /// </summary>
    /// <param name="valueType">enum value type</param>
    /// <param name="text">enum name to convert</param>
    /// <param name="value">enum value after conversion</param>
    /// <returns></returns>
    public static bool TryGetEnumValue(this Type valueType, string text, out object value)
    {
      bool ok = false;
      value = null;
      if (String.IsNullOrEmpty(text))
        return ok;
      string[] enumNames = valueType.GetEnumNames();
      Array enumValues = valueType.GetEnumValues();
      for (int i = 0; i < enumNames.Length; i++)
      {
        if (text.Equals(enumNames[i], StringComparison.CurrentCultureIgnoreCase))
        {
          value = enumValues.GetValue(i);
          ok = true;
          break;
        }
      }
      return ok;
    }

    /// <summary>
    /// Safely sets a property of the target object to some value. Invokes <c>TryGetConverter</c> method.
    /// </summary>
    /// <param name="property">property info as get from type reflection</param>
    /// <param name="targetObject">target object to set value</param>
    /// <param name="value">value to set</param>
    /// <returns></returns>
    public static bool TrySetValue(this PropertyInfo property, object targetObject, object value)
    {
      if (value != null && value.GetType() != property.PropertyType)
      {
        object conValue;
        if (property.PropertyType.TryConvertValue(value, out conValue))
          value = conValue;
      }
      try
      {
        property.SetValue(targetObject, value);
        return true;
      }
      catch { }
      return false;
    }

    /// <summary>
    /// A list of known type converters for <c>TryGetConverter</c> method. 
    /// Is filled after successful invoke of <c>TryGetConverter</c>.
    /// Can be preset by a developer.
    /// </summary>
    public static Dictionary<string, TypeConverter> KnownTypeConverters = new Dictionary<string, TypeConverter>();

    /// <summary>
    /// Tries to get an instance of a value converter for a type using a <c>TypeConverterAttribute</c> of the given type.
    /// </summary>
    /// <param name="valueType"></param>
    /// <param name="typeConverter"></param>
    /// <returns></returns>
    public static bool TryGetConverter(this Type valueType, out TypeConverter typeConverter)
    {
      typeConverter = null;
      TypeConverterAttribute typeConverterAttribute = valueType.GetCustomAttribute<TypeConverterAttribute>();
      if (typeConverterAttribute != null)
      {
        string typeConverterName = typeConverterAttribute.ConverterTypeName;
        if (typeConverterName != null)
        {
          if (!KnownTypeConverters.TryGetValue(typeConverterName, out typeConverter))
          {
            string[] typeNameStrings = typeConverterName.Split(',');
            if (typeNameStrings.Length > 1)
            {
              Assembly typeConverterAssembly = Assembly.Load(typeNameStrings[1]);
              if (typeConverterAssembly != null)
              {
                Type converterType = typeConverterAssembly.GetType(typeNameStrings[0]);
                if (converterType != null)
                {
                  ConstructorInfo constructor = null;
                  if (valueType.IsGenericType)
                  {
                    constructor = converterType.GetConstructor(new Type[] { typeof(Type) });
                    if (constructor != null)
                    {
                      try
                      {
                        object obj = constructor.Invoke(new object[] { valueType.GenericTypeArguments[0] });
                        typeConverter = obj as TypeConverter;
                      }
                      catch (Exception ex)
                      {
                        Debug.WriteLine(String.Format("Error while {0} creation", converterType.Name));
                        Debug.WriteLine(ex.Message);
                      }
                    }
                  }
                  else
                  {
                    constructor = converterType.GetConstructor(new Type[] { });
                    if (constructor != null)
                    {
                      try
                      {
                        object obj = constructor.Invoke(new object[] { });
                        typeConverter = obj as TypeConverter;
                      }
                      catch (Exception ex)
                      {
                        Debug.WriteLine(String.Format("Error while {0} creation", converterType.Name));
                        Debug.WriteLine(ex.Message);
                      }
                    }
                  }
                }
              }
            }
            KnownTypeConverters.Add(typeConverterName, typeConverter);
          }
        }
      }
      return typeConverter != null;
    }

    /// <summary>
    /// Tries to convert a value of the given type using its converter <c>ConvertFrom</c> method
    /// </summary>
    /// <param name="valueType">given type</param>
    /// <param name="value">value to convert from</param>
    /// <param name="result">conversion result</param>
    /// <returns></returns>
    public static bool TryConvertValue(this Type valueType, object value, out object result)
    {
      bool ok = false;
      result = null;
      TypeConverter typeConverter;
      if (TryGetConverter(valueType, out typeConverter))
      {
        result = typeConverter.ConvertFrom(value);
        ok = true;
      }
      return ok;
    }


    /// <summary>
    /// Tries to convert enum string to enum value
    /// </summary>
    /// <param name="enumType">given enum type</param>
    /// <param name="str">string to convert from</param>
    /// <param name="value">conversion result</param>
    /// <returns></returns>
    public static bool TryParseEnum(this Type enumType, string str, out object value)
    {
      bool ok = false;
      value = null;
      if (!enumType.IsEnum)
        return ok;
      List<string> enumNames = enumType.GetEnumNames().ToList();
      string enumName = enumNames.FirstOrDefault(item => item.Equals(str,
        StringComparison.InvariantCultureIgnoreCase));
      if (enumName != null)
      {
        Array enumValues = enumType.GetEnumValues();
        value = enumValues.GetValue(enumNames.IndexOf(enumName));
        ok = true;
      }
      return ok;
    }

    /// <summary>
    /// Expanded <see cref="Type.GetElementType"/> method
    /// with <see cref="Type.GetInterfaces"/>
    /// </summary>
    public static Type GetElementType(this Type aType)
    {
      if (aType.HasElementType)
        return aType.GetElementType();
      else
      {
        Type result = null;
        Type[] interfaces = aType.GetInterfaces();
        foreach (Type aInterface in interfaces)
        {
          if (aInterface.Name.StartsWith("IEnumerable"))
          {
            if (aInterface.IsGenericType)
            {
              Type[] arguments = aInterface.GetGenericArguments();
              if (arguments.Length > 0)
              {
                result = arguments[0];
                break;
              }
            }
          }
        }
        return result;
      }
    }

    /// <summary>
    /// Checking in a <paramref name="aInfo"/> member redefinines a <paramref name="bInfo"/> member.
    /// </summary>
    /// <param name="aInfo">info of a member that redefines</param>
    /// <param name="bInfo">info of a member that is redefined</param>
    public static bool Redefines(this MemberInfo aInfo, MemberInfo bInfo)
    {
      if (aInfo.GetType() != bInfo.GetType())
        return false;

      if (aInfo.GetType() == typeof(FieldInfo))
        return (aInfo as FieldInfo).Redefines(bInfo as FieldInfo);
      if (aInfo.GetType() == typeof(PropertyInfo))
        return (aInfo as PropertyInfo).Redefines(bInfo as PropertyInfo);
      if (aInfo.GetType() == typeof(MethodInfo))
        return (aInfo as MethodInfo).Redefines(bInfo as MethodInfo);
      return false;
    }

    /// <summary>
    /// Checking in a <paramref name="aInfo"/> field redefinines a <paramref name="bInfo"/> field.
    /// </summary>
    /// <param name="aInfo">info of a field that redefines</param>
    /// <param name="bInfo">info of a field that is redefined</param>
    public static bool Redefines(this FieldInfo aInfo, FieldInfo bInfo)
    {
      return aInfo.Name == bInfo.Name;
    }

    /// <summary>
    /// Checking in a <paramref name="aInfo"/> property redefinines a <paramref name="bInfo"/> property.
    /// </summary>
    /// <param name="aInfo">info of a property that redefines</param>
    /// <param name="bInfo">info of a property that is redefined</param>

    public static bool Redefines(this PropertyInfo aInfo, PropertyInfo bInfo)
    {
      return aInfo.Name == bInfo.Name;
    }

    /// <summary>
    /// Checking in a <paramref name="aInfo"/> method redefinines a <paramref name="bInfo"/> method.
    /// </summary>
    /// <param name="aInfo">info of a method that redefines</param>
    /// <param name="bInfo">info of a method that is redefined</param>

    public static bool Redefines(this MethodInfo aInfo, MethodInfo bInfo)
    {
      if (aInfo.Name != bInfo.Name)
        return false;
      ParameterInfo[] aParameters = aInfo.GetParameters();
      ParameterInfo[] bParameters = bInfo.GetParameters();
      if (aParameters.Length != bParameters.Length)
        return false;
      for (int i = 0; i < aParameters.Length - 1; i++)
        if (aParameters[i].ParameterType != bParameters[i].ParameterType)
          return false;
      return true;
    }


    /// <summary>
    /// Copying public properties from a <paramref name="source"/> object to a <paramref name="target"/> object.
    /// Object can be of different types. Properties are paired through names.
    /// Indexers and special properties are not copied.
    /// Returns names of copied properties.
    /// </summary>
    /// <param name="source">Source object</param>
    /// <param name="target">Target object</param>
    /// <param name="revertConversion">use GetDeclaredCopyDelegatesReverse</param>
    /// <returns>Names of copied properties</returns>
    public static string[] CopyProperties(object source, object target, bool revertConversion = false)
    {
      Dictionary<string, CopyPropertyMethod> delegates;
      if (revertConversion)
        delegates = GetDeclaredCopyDelegatesReverse(source.GetType(), target.GetType());
      else
        delegates = GetDeclaredCopyDelegates(source.GetType(), target.GetType());
      return CopyProperties(source, target, delegates);
    }

    /// <summary>
    /// Gets declared delegates for copy of properties. Result - one method for each property name.
    /// </summary>
    /// <param name="sourceType"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    public static Dictionary<string, CopyPropertyMethod> GetDeclaredCopyDelegates(Type sourceType, Type targetType)
    {
      Dictionary<string, CopyPropertyMethod> delegates = new Dictionary<string, CopyPropertyMethod>();
      object lastKey = null;
      try
      {
        foreach (CopyPropertyConversionAttribute attrib in targetType.GetCustomAttributes(typeof(CopyPropertyConversionAttribute), true).Cast<CopyPropertyConversionAttribute>())
        {
          if (attrib.PropertyName != null && attrib.MethodName != null)
          {
            CopyPropertyDelegate delegator = new CopyPropertyDelegate(targetType, attrib.MethodName);
            lastKey = attrib.PropertyName;
            delegates.Add(attrib.PropertyName, delegator.CopyProperty);
          }
        }
        Dictionary<string, CopyItemsDelegate> delegators = new Dictionary<string, CopyItemsDelegate>();
        foreach (CopyPropertyItemConversionAttribute attrib in targetType.GetCustomAttributes(typeof(CopyPropertyItemConversionAttribute), true).Cast<CopyPropertyItemConversionAttribute>())
        {
          if (attrib.PropertyName != null && attrib.SourceItemType != null && attrib.TargetItemType != null)
          {
            CopyItemsDelegate delegator;
            if (!delegators.TryGetValue(attrib.PropertyName, out delegator))
            {
              PropertyInfo targetProperty = targetType.GetProperty(attrib.PropertyName);
              delegator = new CopyItemsDelegate(targetProperty.PropertyType);
              lastKey = attrib.PropertyName;
              delegators.Add(attrib.PropertyName, delegator);
              lastKey = attrib.PropertyName;
              delegates.Add(attrib.PropertyName, delegator.CopyItems);
            }
            lastKey = attrib.SourceItemType;
            delegator.Add(attrib.SourceItemType, attrib.TargetItemType);
          }
        }
      }
      catch (Exception ex)
      {
        throw new InvalidFilterCriteriaException(String.Format("Duplicate key \"{0}\"", lastKey, ex));
      }
      return delegates;
    }

    /// <summary>
    /// Gets declared delegates for reverse copy of properties. Result - one method for each property name.
    /// </summary>
    /// <param name="sourceType"></param>
    /// <param name="targetType"></param>
    public static Dictionary<string, CopyPropertyMethod> GetDeclaredCopyDelegatesReverse(Type sourceType, Type targetType)
    {
      Dictionary<string, CopyPropertyMethod> delegates = new Dictionary<string, CopyPropertyMethod>();
      Dictionary<string, CopyItemsDelegate> delegators = new Dictionary<string, CopyItemsDelegate>();
      foreach (CopyPropertyItemConversionAttribute attrib in sourceType.GetCustomAttributes(typeof(CopyPropertyItemConversionAttribute), true).Cast<CopyPropertyItemConversionAttribute>())
      {
        if (attrib.PropertyName != null && attrib.SourceItemType != null && attrib.TargetItemType != null)
        {
          CopyItemsDelegate delegator;
          if (!delegators.TryGetValue(attrib.PropertyName, out delegator))
          {
            PropertyInfo targetProperty = targetType.GetProperty(attrib.PropertyName);
            delegator = new CopyItemsDelegate(targetProperty.PropertyType);
            delegators.Add(attrib.PropertyName, delegator);
            delegates.Add(attrib.PropertyName, delegator.CopyItems);
          }
          delegator.Add(attrib.TargetItemType, attrib.SourceItemType);
        }
      }
      return delegates;
    }

    /// <summary>
    /// Simple copy of properties from source object to target object using methods delegated by property names.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="delegates"></param>
    /// <returns></returns>
    public static string[] CopyProperties(object source, object target, Dictionary<string, CopyPropertyMethod> delegates)
    {
      Type sourceType = source.GetType();
      Type targetType = target.GetType();
      List<string> copiedProperties = new List<string>();
      PropertyInfo[] sourceProperties = sourceType.GetProperties()
        .Where(sourceProp => sourceProp.Name != "this").ToArray();
      PropertyInfo[] targetProperties = targetType.GetProperties()
        .Where(targetProp => targetProp.Name != "this").ToArray();
      PropertyComparer propertyComparer = new PropertyComparer();
      PropertyInfo[] sameProperties = sourceProperties.Where(sourceProp => targetProperties.Contains(sourceProp, propertyComparer)).ToArray();

      foreach (PropertyInfo sourceProperty in sameProperties)
      {
        MethodInfo getMethod = sourceProperty.GetGetMethod();
        if (getMethod != null)
        {
          PropertyInfo targetProperty = targetProperties.First(targetProp => targetProp.Name == sourceProperty.Name);
          MethodInfo setMethod = targetProperty.GetSetMethod();
          if (setMethod != null)
          {
            object sourceValue = sourceProperty.GetValue(source, new object[0]);
            object targetValue = sourceValue;

            if (delegates != null)
            {
              CopyPropertyMethod duplicator;
              if (delegates.TryGetValue(sourceProperty.Name, out duplicator))
              {
                try
                {
                  getMethod = targetProperty.GetGetMethod();
                  if (getMethod != null)
                    targetValue = targetProperty.GetValue(target, new object[0]);
                  targetValue = duplicator(source, sourceValue, target, targetValue);
                }
                catch (Exception ex1)
                {
                  Debug.WriteLine(ex1.Message);
                }
              }
            }
            Type targetValueType = null;
            if (targetValue != null)
              targetValueType = targetValue.GetType();
            Type targetPropertyType = targetProperty.PropertyType;
            //if (targetPropertyType!=targetValueType && targetValueType!=null)
            //{
            //  Debug.Assert(true);
            //}
            try
            {
              targetProperty.SetValue(target, targetValue, new object[0]);
              copiedProperties.Add(targetProperty.Name);
            }
            catch (Exception ex)
            {
              Debug.WriteLine(ex.Message);
              //throw ex;
            }
          }
        }
      }
      return copiedProperties.ToArray();
    }

    /// <summary>
    /// Helper class for property comparison
    /// </summary>
    public class PropertyComparer : IEqualityComparer<PropertyInfo>
    {

      /// <summary>
      /// Check if one property equals other property.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="y"></param>
      /// <returns></returns>
      public bool Equals(PropertyInfo x, PropertyInfo y)
      {
        return x.Name == y.Name;
      }

      /// <summary>
      /// A method needed to supply <c>Equals</c> method.
      /// </summary>
      /// <param name="obj"></param>
      /// <returns></returns>
      public int GetHashCode(PropertyInfo obj)
      {
        return obj.Name.GetHashCode();
      }
    }

    /// <summary>
    /// A delegate method for property copying
    /// </summary>
    /// <param name="source"></param>
    /// <param name="sourceValue"></param>
    /// <param name="target"></param>
    /// <param name="targetValue"></param>
    /// <returns></returns>
    public delegate object CopyPropertyMethod(object source, object sourceValue, object target, object targetValue);

  }

  /// <summary>
  /// A delegate class to property copying.
  /// Holds a target type and a target method name
  /// </summary>
  public class CopyPropertyDelegate
  {
    private Type TargetType;
    private string TargetMethod;

    /// <summary>
    /// Constructor to init delegate
    /// </summary>
    /// <param name="targetType"></param>
    /// <param name="targetMethod"></param>
    public CopyPropertyDelegate(Type targetType, string targetMethod)
    {
      TargetType = targetType;
      TargetMethod = targetMethod;
    }

    /// <summary>
    /// Copies a property using a target method
    /// </summary>
    /// <param name="source"></param>
    /// <param name="sourceValue"></param>
    /// <param name="target"></param>
    /// <param name="targetValue"></param>
    /// <returns></returns>
    public object CopyProperty(object source, object sourceValue, object target, object targetValue)
    {
      MethodInfo targetMethod = TargetType.GetMethod(TargetMethod, BindingFlags.Public | BindingFlags.Static);
      if (targetMethod == null)
        throw new InvalidOperationException(String.Format("Public static method {1} not found in type {0}", TargetType.Name, TargetMethod));
      Delegate delegator = Delegate.CreateDelegate(typeof(TypeUtils.CopyPropertyMethod), null, targetMethod);
      return delegator.DynamicInvoke(new object[] { source, sourceValue, target, targetValue });
    }
  }

  /// <summary>
  /// An interface to check if a type instance is empty.
  /// </summary>
  public interface IEmpty
  {
    /// <summary>
    /// Simple property to check empty instance
    /// </summary>
    bool IsEmpty { get; }
  }

  /// <summary>
  /// A delegate class to class items copying.
  /// </summary>
  public class CopyItemsDelegate
  {
    /// <summary>
    /// Initializing constructor
    /// </summary>
    /// <param name="targetPropertyType"></param>
    public CopyItemsDelegate(Type targetPropertyType)
    {
      TargetPropertyType = targetPropertyType;
    }

    /// <summary>
    /// A method to add type pairs used to copy items
    /// </summary>
    /// <param name="sourceItemType"></param>
    /// <param name="targetItemType"></param>
    public void Add(Type sourceItemType, Type targetItemType)
    {
      typePairs.Add(sourceItemType, targetItemType);
    }
    Type TargetPropertyType;
    Dictionary<Type, Type> typePairs = new Dictionary<Type, Type>();

    /// <summary>
    /// A method to copy items from source to target
    /// </summary>
    /// <param name="source">source object</param>
    /// <param name="sourceValue">source value - must be <c>IEnumerable</c></param>
    /// <param name="target">target object</param>
    /// <param name="targetValue">target value - must be <c>IList</c></param>
    /// <returns></returns>
    public object CopyItems(object source, object sourceValue, object target, object targetValue)
    {
      if (sourceValue == null)
      {
        IEmpty checker = source as IEmpty;
        if (checker != null && checker.IsEmpty)
          return null;
        return targetValue;
      }
      IEnumerable<object> sourceItems = sourceValue as IEnumerable<object>;
      IList targetItems = targetValue as IList;
      object targetItemsTemp = null;
      if (targetItems == null)
      {
        ConstructorInfo constructor = TargetPropertyType.GetConstructor(new Type[] { });
        if (constructor != null)
        {
          targetItemsTemp = constructor.Invoke(new object[] { });
          targetItems = targetItemsTemp as IList;
        }
        if (targetItems == null)
          targetItems = new List<object>();
      }
      else
        targetItems.Clear();
      foreach (object sourceItem in sourceItems.Where(item => item != null))
      {
        Type sourceItemType = sourceItem.GetType();
        Type targetItemType;
        Type targetType = target.GetType();
        if (typePairs.TryGetValue(sourceItem.GetType(), out targetItemType))
        {
          object targetItem = null;
          ConstructorInfo constructor = targetItemType.GetConstructor(new Type[] { targetType, sourceItemType });
          if (constructor != null)
            targetItem = constructor.Invoke(new object[] { target, sourceItem });
          else
          {
            constructor = targetItemType.GetConstructor(new Type[] { sourceItemType });
            if (constructor != null)
              targetItem = constructor.Invoke(new object[] { sourceItem });
            else
            {
              constructor = targetItemType.GetConstructor(new Type[] { targetType });
              if (constructor != null)
                targetItem = constructor.Invoke(new object[] { target });
              else
              {
                constructor = targetItemType.GetConstructor(new Type[] { });
                if (constructor != null)
                  targetItem = constructor.Invoke(new object[] { });
              }
              if (targetItem != null)
                TypeUtils.CopyProperties(sourceItem, targetItem);
            }
          }
          targetItems.Add(targetItem);
        }
      }
      if (TargetPropertyType.IsArray)
      {
        Type elementType = TargetPropertyType.GetElementType();
        Array targetArray = Array.CreateInstance(elementType, new int[] { targetItems.Count });
        targetItems.CopyTo(targetArray, 0);
        return targetArray;
      }
      return targetItems;
    }

  }

  /// <summary>
  /// An attribute to define conversion of properties for a type while copying
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
  public class CopyPropertyConversionAttribute : Attribute
  {
    /// <summary>
    /// Initializing constructir
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="methodName"></param>
    public CopyPropertyConversionAttribute(string propertyName, string methodName)
    {
      PropertyName = propertyName;
      MethodName = methodName;
    }

    /// <summary>
    /// Name of a property to copy
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>
    /// Name of a method used to convert while property copying
    /// </summary>
    public string MethodName { get; set; }
  }

  /// <summary>
  /// An attribute to define conversion of compound property items for a type while copying
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
  public class CopyPropertyItemConversionAttribute : Attribute
  {
    /// <summary>
    /// Initializing constructor
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="sourceItemType"></param>
    /// <param name="targetItemType"></param>
    public CopyPropertyItemConversionAttribute(string propertyName, Type sourceItemType, Type targetItemType)
    {
      PropertyName = propertyName;
      SourceItemType = sourceItemType;
      TargetItemType = targetItemType;
    }

    /// <summary>
    /// Name of a property to copy
    /// </summary>

    public string PropertyName { get; set; }

    /// <summary>
    /// A type of source items
    /// </summary>
    public Type SourceItemType { get; set; }

    /// <summary>
    /// A type of target items
    /// </summary>
    public Type TargetItemType { get; set; }

  }

}
