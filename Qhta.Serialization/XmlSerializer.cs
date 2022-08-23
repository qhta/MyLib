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
using Qhta.TypeUtils;

namespace Qhta.Xml.Serialization
{
  public partial class XmlSerializer
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
        RegisterType(type);
      if (extraTypes != null)
        foreach (Type t in extraTypes)
          RegisterType(t);
    }

    public KnownTypesDictionary KnownTypes => SerializationInfoMapper.KnownTypes;

    public SerializationOptions Options { get; init; } = new SerializationOptions();

    public XmlSerializationInfoMapper SerializationInfoMapper { get; init; }

    public SerializationTypeInfo? RegisterType(Type aType)
    {     
      return SerializationInfoMapper.RegisterType(aType);
    }

    public SerializationTypeInfo? AddKnownType(Type aType)
    {
      return SerializationInfoMapper.GetKnownType(aType);
    }
    #endregion

    #region helper methods
    public virtual bool IsSimple(Type aType)
    {
      if (aType.Name.StartsWith("`Nullable"))
        aType = aType.GetGenericArguments().First();
      return aType.IsSimple();
    }

    //public virtual bool IsSimple(object propValue)
    //{
    //  bool isSimpleValue = false;
    //  if (propValue is string)
    //    isSimpleValue = true;
    //  else if (propValue is bool)
    //    isSimpleValue = true;
    //  else if (propValue is int)
    //    isSimpleValue = true;
    //  return isSimpleValue;
    //}

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
