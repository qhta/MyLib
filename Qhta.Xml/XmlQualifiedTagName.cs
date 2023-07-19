namespace Qhta.Xml;

/// <summary>
/// Declares an Xml Qualified Tag Name. Replaces <see cref="System.Xml.XmlQualifiedName"/>.
/// </summary>
public record XmlQualifiedTagName: IEquatable<XmlQualifiedName>
{
  /// <summary>
  /// Local name of the tag.
  /// </summary>
  [XmlAttribute] public string Name { get; set; }

  /// <summary>
  /// Full namespace of the tag.
  /// </summary>
  [XmlAttribute] public string Namespace { get; set; }

  /// <summary>
  /// Prefix representing a namespace of the tag.
  /// </summary>
  [XmlAttribute] public string? Prefix { get; set; } = null;

  /// <summary>
  /// Default constructor initializing "Name" and "Namespace" to empty strings.
  /// </summary>
  public XmlQualifiedTagName()
  {
    Name = "";
    Namespace = "";
  }

  /// <summary>
  /// Initializing constructor that splits a string with a colon (':').
  /// </summary>
  /// <param name="str"></param>
  public XmlQualifiedTagName(string str)
  {
    var ss = str.Split(':');
    if (ss.Length == 2)
    {
      Namespace = ss[0];
      Name = ss[1];
    }
    else
    {
      Namespace = "";
      Name = str;
    }
  }

  /// <summary>
  /// Initializing constructor with a name and optional namespace.
  /// </summary>
  /// <param name="name"></param>
  /// <param name="nspace"></param>
  public XmlQualifiedTagName(string name, string? nspace)
  {
    Namespace = nspace ?? "";
    Name = name;
  }

  /// <summary>
  /// Checks if "Name" is empty.
  /// </summary>
  /// <returns></returns>
  public bool IsEmpty()
  {
    return Name == "";
  }

  /// <summary>
  /// Converts to string using a colon as a namespace or prefix separator.
  /// Uses prefix when namespace is empty.
  /// </summary>
  /// <returns></returns>
  public override string ToString()
  {
    if (!String.IsNullOrEmpty(Namespace))
      return Namespace + ":" + Name;
    if (!String.IsNullOrEmpty(Prefix))
      return Prefix + ":" + Name;
    return Name;
  }

  /// <summary>
  /// Converts to string using a colon as a namespace or prefix separator.
  /// Uses namespace when prefix is empty or null.
  /// </summary>
  /// <returns></returns>
  public string ToPrefixedString()
  {
    if (!String.IsNullOrEmpty(Prefix))
      return Prefix + ":" + Name;
    if (!String.IsNullOrEmpty(Namespace))
      return Namespace + ":" + Name;
    return Name;
  }

  /// <summary>
  /// Converts from a string
  /// </summary>
  /// <param name="value"></param>
  //public static implicit operator string(QualifiedName value) => value.ToString();
  public static implicit operator XmlQualifiedTagName(string value) => new XmlQualifiedTagName(value);

  #region Equality members

  /// <summary>
  /// Checks if a XmlQualifiedTagName object equals System.Xml.XmlQualifiedName object.
  /// </summary>
  /// <param name="other"></param>
  /// <returns></returns>
  public bool Equals(XmlQualifiedName? other)
  {
    return Name == other?.Name && Namespace == other?.Namespace;
  }


  /// <summary>
  /// Returs a hash code using name and namespace.
  /// </summary>
  /// <returns></returns>
  public override int GetHashCode()
  {
    return new { Name, Namespace }.GetHashCode();
  }

  //public static bool operator ==(QualifiedName @this, QualifiedName other) => @this.Equals(other);

  //public static bool operator !=(QualifiedName @this, QualifiedName other) => !@this.Equals(other);

  #endregion

  /// <summary>
  /// A plus operator to add a string to name. Namespace an prefix are returned without change.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="str"></param>
  /// <returns></returns>
  public static XmlQualifiedTagName operator +(XmlQualifiedTagName value, string str)
  {
    return new XmlQualifiedTagName{ Name = value.Name + str, Namespace = value.Namespace, Prefix = value.Prefix };
  }
}