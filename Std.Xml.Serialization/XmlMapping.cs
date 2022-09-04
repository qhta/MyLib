// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Xml.Serialization;

namespace Qhta.Xml.Serialization
{
  [Flags]
  public enum XmlMappingAccess
  {
    None = 0x00,
    Read = 0x01,
    Write = 0x02,
  }

  ///<publiconly/>
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public abstract class XmlMapping
  {
    private readonly TypeScope? _scope;
    private bool _generateSerializer;
    private bool _isSoap;
    private readonly ElementAccessor _accessor;
    private string? _key;
    private readonly bool _shallow;
    private readonly XmlMappingAccess _access;

    public XmlMapping(TypeScope? scope, ElementAccessor accessor) : this(scope, accessor, XmlMappingAccess.Read | XmlMappingAccess.Write)
    {
    }

    public XmlMapping(TypeScope? scope, ElementAccessor accessor, XmlMappingAccess access)
    {
      _scope = scope;
      _accessor = accessor;
      _access = access;
      _shallow = scope == null;
    }

    public ElementAccessor Accessor
    {
      get { return _accessor; }
    }

    public TypeScope? Scope
    {
      get { return _scope; }
    }

    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string ElementName
    {
      get { return System.Xml.Serialization.Accessor.UnescapeName(Accessor.Name); }
    }

    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string XsdElementName
    {
      get { return Accessor.Name; }
    }

    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string? Namespace
    {
      get { return _accessor.Namespace; }
    }

    public bool GenerateSerializer
    {
      get { return _generateSerializer; }
      set { _generateSerializer = value; }
    }

    public bool IsReadable
    {
      get { return ((_access & XmlMappingAccess.Read) != 0); }
    }

    public bool IsWriteable
    {
      get { return ((_access & XmlMappingAccess.Write) != 0); }
    }

    public bool IsSoap
    {
      get { return _isSoap; }
      set { _isSoap = value; }
    }

    ///<publiconly/>
    public void SetKey(string? key)
    {
      SetKeypublic(key);
    }

    ///<publiconly/>
    public void SetKeypublic(string? key)
    {
      _key = key;
    }

    public static string GenerateKey(Type type, XmlRootAttribute? root, string? ns)
    {
      if (root == null)
      {
        root = (XmlRootAttribute?)XmlAttributes.GetAttr(type, typeof(XmlRootAttribute));
      }
      return type.FullName + ":" + (root == null ? string.Empty : root.GetKey()) + ":" + (ns == null ? string.Empty : ns);
    }

    public string? Key { get { return _key; } }

    public void CheckShallow()
    {
      if (_shallow)
      {
        throw new InvalidOperationException(SR.XmlMelformMapping);
      }
    }

    public static bool IsShallow(XmlMapping[] mappings)
    {
      for (int i = 0; i < mappings.Length; i++)
      {
        if (mappings[i] == null || mappings[i]._shallow)
          return true;
      }
      return false;
    }
  }
}
