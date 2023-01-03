using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Qhta.TypeUtils;

/// <summary>
///   Extension methods for getting members in order of inheritance
/// </summary>
public static class TypeReflectionByInheritance
{
  #region GetMembers

  /// <summary>
  ///   Replacement for a <c>Type.GetMembers</c> method.
  ///   The members are taken also from superclasses,
  ///   but are also ordered with inheritance order (from top superclass first).
  /// </summary>
  public static MemberInfo[] GetMembersByInheritance(this Type aType)
  {
    return GetMembersByInheritance(aType, BindingFlags.Public | BindingFlags.Instance);
  }

  /// <summary>
  ///   Replacement for a <c>Type.GetMembers</c> method in case
  ///   when a <paramref name="flags" /> parameter does not have option
  ///   <c>BindingFlags.DeclaredOnly</c>. Then members are taken also from superclasses,
  ///   but are also ordered with inheritance order (from top superclass first).
  /// </summary>
  public static MemberInfo[] GetMembersByInheritance(this Type aType, BindingFlags flags)
  {
    if ((flags & BindingFlags.DeclaredOnly) != 0)
      return aType.GetMembers(flags);

    var bType = aType.BaseType;
    if (bType != null)
    {
      var inheritedMembers = GetMembersByInheritance(bType, flags);
      var declaredMembers = aType.GetMembers(flags | BindingFlags.DeclaredOnly);
      var aList = new List<MemberInfo>();
      aList.AddRange(inheritedMembers);
      foreach (var declaredMember in declaredMembers)
      {
        var foundAt = -1;
        for (var i = 0; i < inheritedMembers.Length; i++)
        {
          var inheritedMember = inheritedMembers[i];
          if (declaredMember.Redefines(inheritedMember))
          {
            foundAt = i;
            break;
          }
        }
        // jeśli składowa była zdefiniowana w klasie nadrzędnej,
        // a w danej klasie jest redefiniowana, to nadpisuje
        // deklarację z klasy nadrzędnej - nie ważne czy jest
        // redefiniowana przez override czy przez new
        if (foundAt >= 0)
          aList[foundAt] = declaredMember;
        else
          aList.Add(declaredMember);
      }
      return aList.ToArray();
    }
    if (aType.IsInterface)
    {
      var interfaces = aType.GetInterfaces();
      var aList = new List<MemberInfo>();
      foreach (var bIntf in interfaces)
      {
        var inheritedMembers = GetMembersByInheritance(bIntf, flags);
        aList.AddRange(inheritedMembers);
      }

      var declaredMembers = aType.GetMembers(flags | BindingFlags.DeclaredOnly);
      foreach (var declaredMember in declaredMembers)
      {
        var foundAt = -1;
        for (var i = 0; i < aList.Count; i++)
        {
          var inheritedMember = aList[i];
          if (declaredMember.Redefines(inheritedMember))
          {
            foundAt = i;
            break;
          }
        }
        if (foundAt >= 0)
          aList[foundAt] = declaredMember; // override czy new - bez znaczenia
        else
          aList.Add(declaredMember);
      }
      return aList.ToArray();
    }
    return aType.GetMembers(flags);
  }

  #endregion

  #region GetMethods

  /// <summary>
  ///   Replacement for a <c>Type.GetMethods</c> method.
  ///   The methods are taken also from superclasses,
  ///   but are also ordered with inheritance order (from top superclass first).
  /// </summary>
  public static MethodInfo[] GetMethodsByInheritance(this Type aType)
  {
    return GetMethodsByInheritance(aType, BindingFlags.Public | BindingFlags.Instance);
  }

  /// <summary>
  ///   Replacement for a <c>Type.GetMethods(BindingFlags)</c> method in case
  ///   when a <paramref name="flags" /> parameter does not have option
  ///   <c>BindingFlags.DeclaredOnly</c>. Then methods are taken also from superclasses,
  ///   but are also ordered with inheritance order (from top superclass first).
  /// </summary>
  public static MethodInfo[] GetMethodsByInheritance(this Type aType, BindingFlags flags)
  {
    var memberInfo = GetMembersByInheritance(aType, flags);
    var methodInfos = memberInfo.OfType<MethodInfo>().ToList();
    for (var i = methodInfos.Count - 1; i > 0; i--)
    {
      var currentMethod = methodInfos[i];
      for (var j = i - 1; j >= 0; j--)
        if (IsSameMethod(methodInfos[j], currentMethod))
        {
          methodInfos[j] = currentMethod;
          methodInfos.RemoveAt(i);
          break;
        }
    }
    return methodInfos.ToArray();
  }

  private static bool IsSameMethod(MethodInfo method1, MethodInfo method2)
  {
    if (method1.Name != method2.Name)
      return false;
    if (method1.ReturnParameter != method2.ReturnParameter)
      return false;
    var params1 = method1.GetParameters();
    var params2 = method2.GetParameters();
    if (params1.Length != params2.Length)
      return false;
    for (var i = 0; i < params1.Length; i++)
      if (!IsSameParam(params1[i], params2[i]))
        return false;
    var args1 = method1.GetGenericArguments();
    var args2 = method2.GetGenericArguments();
    if (args1.Length != args2.Length)
      return false;
    for (var i = 0; i < args1.Length; i++)
      if (args1[i] != args2[i])
        return false;
    return true;
  }

  private static bool IsSameParam(ParameterInfo param1, ParameterInfo param2)
  {
    if (param1.ParameterType != param2.ParameterType)
      return false;
    var modifiers1 = param1.GetRequiredCustomModifiers();
    var modifiers2 = param2.GetRequiredCustomModifiers();
    if (modifiers1.Length != modifiers2.Length)
      return false;
    for (var i = 0; i < modifiers1.Length; i++)
      if (modifiers1[i] != modifiers2[i])
        return false;
    return true;
  }

  /// <summary>
  ///   Replacement for a <c>Type.GetMethod(string)</c> method.
  ///   The methods are taken also from superclasses,
  ///   but are also ordered with inheritance order (from top superclass first).
  /// </summary>
  public static MethodInfo? GetMethodByInheritance(this Type aType, string methodName)
  {
    return GetMethodByInheritance(aType, methodName, BindingFlags.Public | BindingFlags.Instance);
  }

  /// <summary>
  ///   Replacement for a <c>Type.GetMethod(string, BindingFlags)</c> method in case
  ///   when a <paramref name="flags" /> parameter does not have option
  ///   <c>BindingFlags.DeclaredOnly</c>. Then methods are taken also from superclasses,
  ///   but are also ordered with inheritance order (from top superclass first).
  /// </summary>
  public static MethodInfo? GetMethodByInheritance(this Type aType, string methodName, BindingFlags flags)
  {
    var methodInfos = GetMethodsByInheritance(aType, flags);
    return methodInfos.FirstOrDefault(item => item.Name == methodName);
  }

  /// <summary>
  ///   Replacement for a <c>Type.GetMethod(string, Type[])</c> method.
  ///   The methods are taken also from superclasses,
  ///   but are also ordered with inheritance order (from top superclass first).
  /// </summary>
  public static MethodInfo? GetMethodByInheritance(this Type aType, string methodName, Type[] types)
  {
    return GetMethodByInheritance(aType, methodName, BindingFlags.Public | BindingFlags.Instance, types);
  }

  /// <summary>
  ///   Replacement for a <c>Type.GetMethod(string, BindingFlags, Type[])</c> method in case
  ///   when a <paramref name="flags" /> parameter does not have option
  ///   <c>BindingFlags.DeclaredOnly</c>. Then methods are taken also from superclasses,
  ///   but are also ordered with inheritance order (from top superclass first).
  /// </summary>
  public static MethodInfo? GetMethodByInheritance(this Type aType, string methodName, BindingFlags flags, Type[] types)
  {
    var methodInfos = GetMethodsByInheritance(aType, flags);
    var methods = methodInfos.Where(item => item.Name == methodName);
    foreach (var methodInfo in methods)
    {
      var paramInfos = methodInfo.GetParameters();
      if (paramInfos.Length != types.Length)
        continue;
      var paramsOK = true;
      for (var i = 0; i < paramInfos.Length; i++)
        if (paramInfos[i].ParameterType != types[i])
        {
          paramsOK = false;
          break;
        }
      if (paramsOK)
        return methodInfo;
    }
    return null;
  }

  #endregion

  #region GetProperties

  /// <summary>
  ///   Replacement for a <c>Type.GetProperties</c> method.
  ///   The properties are taken also from superclasses,
  ///   but are also ordered with inheritance order (from top superclass first).
  /// </summary>
  public static PropertyInfo[] GetPropertiesByInheritance(this Type aType)
  {
    return GetPropertiesByInheritance(aType, BindingFlags.Public | BindingFlags.Instance);
  }

  /// <summary>
  ///   Replacement for a <c>Type.GetProperties(BindingFlags)</c> method in case
  ///   when a <paramref name="flags" /> parameter does not have option
  ///   <c>BindingFlags.DeclaredOnly</c>. Then properties are taken also from superclasses,
  ///   but are also ordered with inheritance order (from top superclass first).
  /// </summary>
  public static PropertyInfo[] GetPropertiesByInheritance(this Type aType, BindingFlags flags)
  {
    var memberInfo = GetMembersByInheritance(aType, flags);
    var propInfos = memberInfo.OfType<PropertyInfo>().ToList();
    for (var i = propInfos.Count - 1; i > 0; i--)
    {
      var currentProp = propInfos[i];
      for (var j = i - 1; j >= 0; j--)
        if (IsSameProperty(propInfos[j], currentProp))
        {
          propInfos[j] = currentProp;
          propInfos.RemoveAt(i);
          break;
        }
    }
    return propInfos.ToArray();
  }

  private static bool IsSameProperty(PropertyInfo property1, PropertyInfo property2)
  {
    if (property1.Name != property2.Name)
      return false;
    if (property1.PropertyType != property2.PropertyType)
      return false;
    var params1 = property1.GetIndexParameters();
    var params2 = property2.GetIndexParameters();
    if (params1.Length != params2.Length)
      return false;
    for (var i = 0; i < params1.Length; i++)
      if (!IsSameParam(params1[i], params2[i]))
        return false;
    return true;
  }

  /// <summary>
  ///   Replacement for a <c>Type.GetProperty(string)</c> method.
  ///   The properties are taken also from superclasses,
  ///   but are also ordered with inheritance order (from top superclass first).
  /// </summary>
  public static PropertyInfo? GetPropertyByInheritance(this Type aType, string propertyName)
  {
    return GetPropertyByInheritance(aType, propertyName, BindingFlags.Public | BindingFlags.Instance);
  }

  /// <summary>
  ///   Replacement for a <c>Type.GetProperty(string, BindingFlags)</c> method in case
  ///   when a <paramref name="flags" /> parameter does not have option
  ///   <c>BindingFlags.DeclaredOnly</c>. Then properties are taken also from superclasses,
  ///   but are also ordered with inheritance order (from top superclass first).
  /// </summary>
  public static PropertyInfo? GetPropertyByInheritance(this Type aType, string propertyName, BindingFlags flags)
  {
    var propInfos = GetPropertiesByInheritance(aType, flags);
    return propInfos.FirstOrDefault(item => item.Name == propertyName);
  }

  #endregion

  #region GetFields

  /// <summary>
  ///   Replacement for a <c>Type.GetFields</c> method.
  ///   The properties are taken also from superclasses,
  ///   but are also ordered with inheritance order (from top superclass first).
  /// </summary>
  public static FieldInfo[] GetFieldsByInheritance(this Type aType)
  {
    return GetFieldsByInheritance(aType, BindingFlags.Public | BindingFlags.Instance);
  }

  /// <summary>
  ///   Replacement for a <c>Type.GetFields(BindingFlags)</c> method in case
  ///   when a <paramref name="flags" /> parameter does not have option
  ///   <c>BindingFlags.DeclaredOnly</c>. Then properties are taken also from superclasses,
  ///   but are also ordered with inheritance order (from top superclass first).
  /// </summary>
  public static FieldInfo[] GetFieldsByInheritance(this Type aType, BindingFlags flags)
  {
    var memberInfo = GetMembersByInheritance(aType, flags);
    var propInfos = memberInfo.OfType<FieldInfo>().ToList();
    for (var i = propInfos.Count - 1; i > 0; i--)
    {
      var currentProp = propInfos[i];
      for (var j = i - 1; j >= 0; j--)
        if (IsSameField(propInfos[j], currentProp))
        {
          propInfos[j] = currentProp;
          propInfos.RemoveAt(i);
          break;
        }
    }
    return propInfos.ToArray();
  }

  private static bool IsSameField(FieldInfo field1, FieldInfo field2)
  {
    if (field1.Name != field2.Name)
      return false;
    if (field1.FieldType != field2.FieldType)
      return false;
    return true;
  }

  /// <summary>
  ///   Replacement for a <c>Type.GetField(string)</c> method.
  ///   The properties are taken also from superclasses,
  ///   but are also ordered with inheritance order (from top superclass first).
  /// </summary>
  public static FieldInfo? GetFieldByInheritance(this Type aType, string fieldName)
  {
    return GetFieldByInheritance(aType, fieldName, BindingFlags.Public | BindingFlags.Instance);
  }

  /// <summary>
  ///   Replacement for a <c>Type.GetField(string, BindingFlags)</c> method in case
  ///   when a <paramref name="flags" /> parameter does not have option
  ///   <c>BindingFlags.DeclaredOnly</c>. Then properties are taken also from superclasses,
  ///   but are also ordered with inheritance order (from top superclass first).
  /// </summary>
  public static FieldInfo? GetFieldByInheritance(this Type aType, string fieldName, BindingFlags flags)
  {
    var propInfos = GetFieldsByInheritance(aType, flags);
    return propInfos.FirstOrDefault(item => item.Name == fieldName);
  }

  #endregion

  #region GetCustomAttributes

  /// <summary>
  ///   Replacement for a <c>Type.GetCustomAttributes</c> method in case
  ///   when an <paramref name="inherit" /> parameter is set for <c>true</c>
  ///   Then attributes are taken also from superclasses,
  ///   but are also ordered with inheritance order.
  /// </summary>
  /// <param name="aType">A type which attributes are searched</param>
  /// <param name="inherit">Search in superclasses?</param>
  /// <param name="inheritedFirst">Should inherited attributes be ordered first?</param>
  /// <returns>A table of attributes</returns>
  public static object[] GetCustomAttibutesByInheritance(this Type aType, bool inherit, bool inheritedFirst = false)
  {
    if (!inherit)
      return aType.GetCustomAttributes(inherit);

    var bType = aType.BaseType;
    if (bType != null)
    {
      var inheritedAttributes = bType.GetCustomAttibutesByInheritance(inherit, inheritedFirst);
      object[] declaredAttributes = aType.GetCustomAttributes(false);
      var aList = new List<object>();
      if (inheritedFirst)
      {
        aList.AddRange(inheritedAttributes);
        aList.AddRange(declaredAttributes);
      }
      else
      {
        aList.AddRange(declaredAttributes);
        aList.AddRange(inheritedAttributes);
      }
      return aList.ToArray();
    }
    return aType.GetCustomAttributes(inherit);
  }

  /// <summary>
  ///   Replacement for a <c>Type.GetCustomAttributes</c> method in case
  ///   when an <paramref name="inherit" /> parameter is set for <c>true</c>
  ///   Then attributes are taken also from superclasses,
  ///   but are also ordered with inheritance order.
  ///   Only attributes of a specified type are returned.
  /// </summary>
  /// <typeparam name="TAttribute">A type of searched attributes</typeparam>
  /// <param name="aType">A type which attributes are searched</param>
  /// <param name="inherit">Search in superclasses?</param>
  /// <param name="inheritedFirst">Should inherited attributes be ordered first?</param>
  /// <returns>A table of attributes</returns>
  public static object[] GetCustomAttibutesByInheritance<TAttribute>(this Type aType, bool inherit, bool inheritedFirst = false)
    where TAttribute : Attribute
  {
    if (!inherit)
      return aType.GetCustomAttributes(typeof(TAttribute), inherit);

    var bType = aType.BaseType;
    if (bType != null)
    {
      var inheritedAttributes = bType.GetCustomAttibutesByInheritance<TAttribute>(inherit);
      object[] declaredAttributes = aType.GetCustomAttributes(typeof(TAttribute), false);
      var aList = new List<object>();
      if (inheritedFirst)
      {
        aList.AddRange(inheritedAttributes);
        aList.AddRange(declaredAttributes);
      }
      else
      {
        aList.AddRange(declaredAttributes);
        aList.AddRange(inheritedAttributes);
      }
      return aList.ToArray();
    }
    return aType.GetCustomAttributes(typeof(TAttribute), inherit);
  }

  #endregion
}