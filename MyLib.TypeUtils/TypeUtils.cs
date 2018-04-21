using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MyLib.TypeUtils
{
  /// <summary>
  /// Funkcje pomocnicze działające na typach i uzupełniające refleksję typów
  /// </summary>
  public static class TypeUtils
  {
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

    public static bool TrySetValue(this PropertyInfo property, object targetObject, object value)
    {
      if (value!=null && value.GetType()!=property.PropertyType)
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

    public static Dictionary<string, TypeConverter> KnownTypeConverters = new Dictionary<string, TypeConverter>();

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
      return typeConverter!=null;
    }
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

    public static bool TryParseEnum(this Type enumType, string str, out object value)
    {
      bool ok=false;
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
    /// Podaje typ elementów składowych danego typu złożonego.
    /// Rozszerza metodę <see cref="Type.GetElementType"/> 
    /// o badania interfejsów implementowanych przez dany typ.
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
    ///   Zmienia metodę <see cref="Type.GetMembers"/> dla przypadku, gdy parametr <param name="Flags"/>
    ///   nie ma ustawionej opcji <see cref="BindingFlags.DeclaredOnly"/>.
    ///   Wówczas metody są pobierane również z klas nadrzędnych w hierarchii dziedziczenia,
    ///   ale są też uporządkowane w kolejności dziedziczenia.
    ///  </summary>
    public static MemberInfo[] GetMembersByInheritance(this Type aType, BindingFlags Flags)
    {
      if ((Flags & BindingFlags.DeclaredOnly) != 0)
        return aType.GetMembers(Flags);

      Type bType = aType.BaseType;
      if (bType != null)
      {
        MemberInfo[] inheritedMembers = GetMembersByInheritance(bType, Flags);
        MemberInfo[] declaredMembers = aType.GetMembers(Flags | BindingFlags.DeclaredOnly);
        List<MemberInfo> aList = new List<MemberInfo>();
        aList.AddRange(inheritedMembers);
        foreach (MemberInfo declaredMember in declaredMembers)
        {
          int foundAt = -1;
          for (int i = 0; i < inheritedMembers.Length; i++)
          {
            MemberInfo inheritedMember = inheritedMembers[i];
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
      else
      {
        if (aType.IsInterface)
        {
          Type[] interfaces = aType.GetInterfaces();
          List<MemberInfo> aList = new List<MemberInfo>();
          foreach (Type bIntf in interfaces)
          {
            MemberInfo[] inheritedMembers = GetMembersByInheritance(bIntf, Flags);
            aList.AddRange(inheritedMembers);
          }

          MemberInfo[] declaredMembers = aType.GetMembers(Flags | BindingFlags.DeclaredOnly);
          foreach (MemberInfo declaredMember in declaredMembers)
          {
            int foundAt = -1;
            for (int i = 0; i < aList.Count; i++)
            {
              MemberInfo inheritedMember = aList[i];
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
      }
      return aType.GetMembers(Flags);
    }

    /// <summary>
    /// Sprawdzenie, czy składowa <param name="aInfo"/> redefiniuje składową <param name="bInfo">.
    /// </summary>
    public static bool Redefines(this MemberInfo aInfo, MemberInfo bInfo)
    {
      // jeśli to nie są składowe tego samego typu, to na pewno nie
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
    /// Sprawdzenie, czy pole <param name="aInfo"/> redefiniuje pole <param name="bInfo">.
    /// </summary>
    public static bool Redefines(this FieldInfo aInfo, FieldInfo bInfo)
    {
      return aInfo.Name == bInfo.Name;
    }

    /// <summary>
    /// Sprawdzenie, czy właściwość <param name="aInfo"/> redefiniuje właściwość <param name="bInfo">.
    /// </summary>
    public static bool Redefines(this PropertyInfo aInfo, PropertyInfo bInfo)
    {
      return aInfo.Name == bInfo.Name;
    }

    /// <summary>
    /// Sprawdzenie, czy metoda <param name="aInfo"/> redefiniuje metodę <param name="bInfo">.
    /// </summary>
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
    /*
        /// <summary>
        ///   Zmienia metodę <see cref="MemberInfo.GetCustomAttributes"/> dla przypadku, gdy parametr <param name="inherit"/>
        ///   jest ustawiony na <c>true</c>.
        ///   Wówczas atrybuty są pobierane również z klas nadrzędnych w hierarchii dziedziczenia,
        ///   ale są też uporządkowane w kolejności dziedziczenia.
        ///  </summary>
        public static object[] GetCustomAttibutesByInheritance(this MemberInfo aMember, bool inherit)
        {
          if (!inherit)
            return aMember.GetCustomAttributes(inherit);

          Type bType = aMember.DeclaringType.BaseType;
          if (bType != null)
          {
            aMember.
            MemberInfo[] redefinedMember = bType.GetMembersByInheritance(BindingFlags.
            object[] inheritedAttributes = bType.GetCustomAttibutesByInheritance(inherit);
            object[] declaredAttributes = aMember.GetCustomAttributes(false);
            List<object> aList = new List<object>(inheritedAttributes);
            aList.AddRange(declaredAttributes);
            return aList.ToArray();
          }
          return aMember.GetCustomAttributes(inherit);
        }
    */
    /// <summary>
    ///   Zmienia metodę <see cref="Type.GetCustomAttributes"/> dla przypadku, gdy parametr <param name="inherit"/>
    ///   jest ustawiony na <c>true</c>.
    ///   Wówczas atrybuty są pobierane również z klas nadrzędnych w hierarchii dziedziczenia,
    ///   ale są też uporządkowane w kolejności dziedziczenia.
    ///  </summary>
    /// <typeparam name="TAttribute">Typ do którego muszą należeć atrybuty, które mają zostać zwrócone</typeparam>
    /// <param name="inherit">Czy brać pod uwagę nadklasy</param>
    /// <param name="inheritedFirst">czy dziedziczone mają być zwracane najpierw</param>
    /// <returns>Tablica atrybutów z zachowaną kolejnością dziedziczenia</returns>
    public static object[] GetCustomAttibutesByInheritance(this Type aType, bool inherit, bool inheritedFirst = false)
    {
      if (!inherit)
        return aType.GetCustomAttributes(inherit);

      Type bType = aType.BaseType;
      if (bType != null)
      {
        object[] inheritedAttributes = bType.GetCustomAttibutesByInheritance(inherit, inheritedFirst);
        object[] declaredAttributes = aType.GetCustomAttributes(false);
        List<object> aList = new List<object>();
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
    ///   Zmienia metodę <see cref="Type.GetCustomAttributes"/> dla przypadku, gdy parametr <param name="inherit"/>
    ///   jest ustawiony na <c>true</c>.
    ///   Wówczas atrybuty są pobierane również z klas nadrzędnych w hierarchii dziedziczenia,
    ///   ale są też uporządkowane w kolejności dziedziczenia. Metoda zwraca tylko atrybuty o podanym typie.
    /// </summary>
    /// <typeparam name="TAttribute">Typ do którego muszą należeć atrybuty, które mają zostać zwrócone</typeparam>
    /// <param name="inherit">Czy brać pod uwagę nadklasy</param>
    /// <param name="inheritedFirst">czy dziedziczone mają być zwracane najpierw</param>
    /// <returns>Tablica atrybutów z zachowaną kolejnością dziedziczenia</returns>
    public static object[] GetCustomAttibutesByInheritance<TAttribute>(this Type aType, bool inherit, bool inheritedFirst = false) where TAttribute : Attribute
    {
      if (!inherit)
        return aType.GetCustomAttributes(typeof(TAttribute), inherit);

      Type bType = aType.BaseType;
      if (bType != null)
      {
        object[] inheritedAttributes = bType.GetCustomAttibutesByInheritance<TAttribute>(inherit);
        object[] declaredAttributes = aType.GetCustomAttributes(typeof(TAttribute), false);
        List<object> aList = new List<object>();
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

    /// <summary>
    /// Kopiuje publiczne właściwości z obiektu <paramref name="source"/> do obiektu <paramref name="target"/>.
    /// Obiekty mogą być różnych typów. Właściwości są ze sobą parowane przez nazwy.
    /// Indeksery i właściwości specjalne nie są kopiowane
    /// Kopiowane są tylko  te właściwości, które w obiekcie źródłowym mają metodę odczytu,
    /// a w obiekcie docelowym mają metodę zapisu.
    /// Właściwości mogą być różnych typów, 
    /// ale właściwość docelowa musi mieć możliwość przyjęcia danej z właściwości źródłowej.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns>nazwy przekopiowanych właściwości</returns>
    public static string[] CopyProperties(object source, object target, bool revertConversion=false)
    {
      Dictionary<string, CopyPropertyMethod> delegates;
      if (revertConversion)
        delegates = GetDeclaredCopyDelegatesReverse(source.GetType(), target.GetType());
      else
        delegates = GetDeclaredCopyDelegates(source.GetType(), target.GetType());
      return CopyProperties(source, target, delegates);
    }

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
            if (targetValue!=null)
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

    public class PropertyComparer : IEqualityComparer<PropertyInfo>
    {

      public bool Equals(PropertyInfo x, PropertyInfo y)
      {
        return x.Name == y.Name;
      }

      public int GetHashCode(PropertyInfo obj)
      {
        return obj.Name.GetHashCode();
      }
    }

    public delegate object CopyPropertyMethod (object source, object sourceValue, object target, object targetValue);

  }

  public class CopyPropertyDelegate
  {
    private Type TargetType;
    private string TargetMethod;

    public CopyPropertyDelegate(Type targetType, string targetMethod) 
    {
      TargetType = targetType;
      TargetMethod = targetMethod;
    }

    public object CopyProperty(object source, object sourceValue, object target, object targetValue)
    {
      MethodInfo targetMethod = TargetType.GetMethod(TargetMethod,BindingFlags.Public | BindingFlags.Static);
      if (targetMethod == null)
        throw new InvalidOperationException(String.Format("Public static method {1} not found in type {0}", TargetType.Name, TargetMethod));
      Delegate delegator = Delegate.CreateDelegate(typeof(TypeUtils.CopyPropertyMethod), null, targetMethod);
      return delegator.DynamicInvoke(new object[] { source, sourceValue, target, targetValue });
    }
  }

  public interface IEmpty
  {
    bool IsEmpty { get; }
  }

  public class CopyItemsDelegate
  {
    public CopyItemsDelegate(Type targetPropertyType)
    {
      TargetPropertyType = targetPropertyType;
    }

    public void Add(Type sourceItemType, Type targetItemType)
    {
      typePairs.Add(sourceItemType, targetItemType);
    }
    Type TargetPropertyType;
    Dictionary<Type, Type> typePairs = new Dictionary<Type, Type>();

    public object CopyItems(object source, object sourceValue, object target, object targetValue)
    {
      if (sourceValue == null)
      {
        IEmpty checker = source as IEmpty;
        if (checker!=null && checker.IsEmpty)
          return null;
        return targetValue;
      }
      IEnumerable<object> sourceItems = sourceValue as IEnumerable<object>;
      IList targetItems = targetValue as IList;
      object targetItemsTemp = null;
      if (targetItems == null)
      {
        ConstructorInfo constructor = TargetPropertyType.GetConstructor(new Type[]{});
        if (constructor!=null)
        {
          targetItemsTemp = constructor.Invoke(new object[]{});
          targetItems = targetItemsTemp as IList;
        }
        if (targetItems==null)
          targetItems = new List<object>();
      }
      else
        targetItems.Clear();
      foreach (object sourceItem in sourceItems.Where(item => item!=null))
      {
        Type sourceItemType = sourceItem.GetType();
        Type targetItemType;
        Type targetType = target.GetType();
        if (typePairs.TryGetValue(sourceItem.GetType(), out targetItemType))
        {
          object targetItem=null;
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
        Array targetArray = Array.CreateInstance(elementType, new int[]{targetItems.Count});
        targetItems.CopyTo(targetArray,0);
        return targetArray;
      }
      return targetItems;
    }

  }

  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
  public class CopyPropertyConversionAttribute : Attribute
  {
    public CopyPropertyConversionAttribute(string propertyName, string methodName)
    {
      PropertyName = propertyName;
      MethodName = methodName;
    }

    public string PropertyName { get; set; }

    public string MethodName { get; set; }
  }

  [AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=true)]
  public class CopyPropertyItemConversionAttribute : Attribute
  {
    public CopyPropertyItemConversionAttribute(string propertyName, Type sourceItemType, Type targetItemType)
    {
      PropertyName = propertyName;
      SourceItemType = sourceItemType;
      TargetItemType = targetItemType;
    }

    public string PropertyName { get; set; }

    public Type SourceItemType { get; set; }

    public Type TargetItemType { get; set; }
  }

}
