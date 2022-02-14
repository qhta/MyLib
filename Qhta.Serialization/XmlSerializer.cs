using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using Qhta.TestHelper;

namespace Qhta.Serialization
{
  public partial class XmlSerializer : IXSerializer
  {
    #region Creation methods

    public XmlSerializer(Type type) : this(type, null, null) { }

    public XmlSerializer(Type type, Type[]? extraTypes) : this(type, extraTypes, null) { }

    public XmlSerializer(Type type, SerializationOptions? options) : this(type, null, options) { }

    public XmlSerializer(Type type, Type[]? extraTypes, SerializationOptions? options)
    {
      if (options != null)
        Options = options;
      SerializationInfoMapper = new XmlSerializationInfoMapper(Options, type.Namespace);
      if (type!=null)
        RegisterKnownType(type);
      if (extraTypes != null)
        foreach (Type t in extraTypes)
          RegisterKnownType(t);
    }

    public KnownTypesDictionary KnownTypes => SerializationInfoMapper.KnownTypes;

    public SerializationOptions Options { get; init; } = new SerializationOptions();

    protected XmlSerializationInfoMapper SerializationInfoMapper { get; init; }

    protected SerializationTypeInfo? RegisterKnownType(Type aType)
    {
      return SerializationInfoMapper.RegisterKnownType(aType);
    }

    protected SerializationTypeInfo? AddKnownType(Type aType)
    {
      return SerializationInfoMapper.AddKnownType(aType);
    }
    #endregion

    #region helper methods
    public virtual bool IsSimple(Type aType)
    {
      bool isSimpleValue = false;
      if (aType == typeof(string))
        isSimpleValue = true;
      else if (aType == typeof(bool))
        isSimpleValue = true;
      else if (aType == typeof(int))
        isSimpleValue = true;
      else if (aType.Name.StartsWith("`Nullable"))
        return IsSimple(aType.GetGenericArguments().First());
      return isSimpleValue;
    }

    public virtual bool IsSimple(object propValue)
    {
      bool isSimpleValue = false;
      if (propValue is string)
        isSimpleValue = true;
      else if (propValue is bool)
        isSimpleValue = true;
      else if (propValue is int)
        isSimpleValue = true;
      return isSimpleValue;
    }

    public virtual string LowercaseName(string str)
    {
      if (IsUpper(str))
        return str.ToLower();
      return ToLowerFirst(str);
    }

    public static string ToLowerFirst(string text)
    {
      if (string.IsNullOrEmpty(text))
        return text;
      char[] ss = text.ToCharArray();
      ss[0] = char.ToLower(ss[0]);
      return new string(ss);
    }
    public static bool IsUpper(string text)
    {
      if (text == null)
        return false;
      foreach (var ch in text)
        if (char.IsLetter(ch) && !Char.IsUpper(ch))
          return false;
      return true;
    }
    #endregion
  }
}
