using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Xml;

using Qhta.TestHelper;
using Qhta.TypeUtils;

namespace Qhta.Conversion;

public class TypeNameConverter : BaseTypeConverter
{
  /// <summary>
  ///   Prefix to namespace conversion
  /// </summary>
  public virtual Dictionary<string, string>? KnownPrefixes { get; set; }

  public TypeNameConverter()
  {
  }

  public TypeNameConverter(IEnumerable<Type> types, Dictionary<string, string>? knownNamespaces = null)
  {
    if (KnownTypes == null)
      KnownTypes = new();
    foreach (var type in types)
    {
      if (type.FullName != null)
        KnownTypes.Add(type.FullName, type);
    }
    KnownNamespaces = knownNamespaces;
    if (knownNamespaces != null)
      KnownPrefixes = knownNamespaces.ToDictionary(item => item.Value, item => item.Key);
  }

  public TypeNameConverter(Dictionary<string, Type> types, Dictionary<string, string>? knownNamespaces)
  {
    KnownTypes = types;
    KnownNamespaces = knownNamespaces;
    if (knownNamespaces != null)
      KnownPrefixes = knownNamespaces.ToDictionary(item => item.Value, item => item.Key);
  }

  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    return destinationType == typeof(string);
  }

  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value == null)
      return null;
    if (value is Type type)
    {
      var typeName = TypeNaming.GetTypeName(type);
      if (typeName != null && typeName != type.FullName)
        return typeName;
      if (KnownNamespaces != null)
      {
        var ns = type.Namespace;
        if (ns != null && KnownNamespaces.TryGetValue(ns, out var prefix))
        {
          if (!string.IsNullOrEmpty(prefix))
            return prefix + ":" + type.Name;
          else
            return type.Name;
        }
      }
      return type.FullName;
    }
    return base.ConvertTo(context, culture, value, destinationType);
  }

  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
  {
    return sourceType == typeof(string);
  }

  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    if (value == null)
      return null;
    if (value is string str)
    {
      if (str == String.Empty)
        return null;
      //if (str=="HeadingPairs")
      //  TestTools.Stop();
      var aType = TypeNaming.GetType(str);
      if (aType != null)
        return aType;
      if (KnownPrefixes != null)
      {
        var prefix = "";
        var name = str;
        var ss = str.Split(':');
        if (ss.Length > 1)
        {
          prefix = ss[0];
          name = ss[1];
        }
        if (KnownPrefixes.TryGetValue(prefix, out var ns))
          str = ns+"."+name;
      }
      if (KnownTypes != null && KnownTypes.TryGetValue(str, out var type))
        return type;
      type = Type.GetType(str);
      return type;
    }
    return base.ConvertFrom(context, culture, value);
  }
}