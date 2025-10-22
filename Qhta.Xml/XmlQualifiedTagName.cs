namespace Qhta.Xml;

/// <summary>
/// Declares Xml Qualified Tag Name. Replaces <see cref="System.Xml.XmlQualifiedName"/>.
/// </summary>
public record XmlQualifiedTagName: IEquatable<XmlQualifiedName>
{
  /// <summary>
  /// Local name of the tag.
  /// </summary>
  [XmlAttribute] public QualifiedName QualifiedName { get; private set; }

  /// <summary>
  /// Full namespace of the tag.
  /// </summary>
  public string LocalName => QualifiedName.LocalName;

  /// <summary>
  /// Full namespace of the tag.
  /// </summary>
  public string Namespace => QualifiedName.Namespace;

  /// <summary>
  /// Prefix representing a namespace of the tag.
  /// </summary>
  [XmlAttribute] public string? Prefix { get; set; } = null;

  /// <summary>
  /// Default constructor initializing "Name" and "Namespace" to empty strings.
  /// </summary>
  public XmlQualifiedTagName()
  {
    QualifiedName = new QualifiedName();
  }

  /// <summary>
  /// Initializing constructor that splits a string with a colon (':').
  /// </summary>
  /// <param name="str"></param>
  public XmlQualifiedTagName(string str)
  {
    var ss = str.Split(':');
    if (ss.Length == 2)
    { QualifiedName = new QualifiedName(ss[1], ss[0]);
    }
    else
    {
      QualifiedName = new QualifiedName(str);
    }
  }

  /// <summary>
  /// Initializing constructor with a name and optional namespace.
  /// </summary>
  /// <param name="name"></param>
  /// <param name="nspace"></param>
  public XmlQualifiedTagName(string name, string? nspace)
  {
    QualifiedName = new QualifiedName(name, nspace);
  }

  /// <summary>
  /// Checks if "Name" is empty.
  /// </summary>
  /// <returns></returns>
  public bool IsEmpty()
  {
    return QualifiedName.IsEmpty();
  }

  /// <summary>
  /// Converts to string using a colon as a namespace or prefix separator.
  /// Uses prefix when namespace is empty.
  /// </summary>
  /// <returns></returns>
  public override string ToString()
  {
    if (!String.IsNullOrEmpty(QualifiedName.Namespace))
      return QualifiedName.Namespace + ":" + LocalName;
    if (!String.IsNullOrEmpty(Prefix))
      return Prefix + ":" + LocalName;
    return LocalName;
  }

  /// <summary>
  /// Converts to string using a colon as a namespace or prefix separator.
  /// Uses namespace when prefix is empty or null.
  /// </summary>
  /// <returns></returns>
  public string ToPrefixedString()
  {
    if (!String.IsNullOrEmpty(Prefix))
      return Prefix + ":" + LocalName;
    if (!String.IsNullOrEmpty(QualifiedName.Namespace))
      return QualifiedName.Namespace + ":" + LocalName;
    return LocalName;
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
    return QualifiedName.Equals(other);
  }


  /// <summary>
  /// Returns a hash code using name and namespace.
  /// </summary>
  /// <returns></returns>
  public override int GetHashCode()
  {
    return LocalName.GetHashCode();
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
    return new XmlQualifiedTagName (value) {Prefix = value.Prefix };
  }
}