namespace Qhta.Xml.Serialization;

//[TypeConverter(typeof(QualifiedNameTypeConverter))]
public struct XmlQualifiedTagName //: IComparable<QualifiedName>, IEquatable<QualifiedName>
{
  [XmlAttribute] public string Name { get; set; }

  [XmlAttribute] public string XmlNamespace { get; set; }

  [XmlAttribute] public string? Prefix { get; set; } = null;

  public XmlQualifiedTagName()
  {
    Name = "";
    XmlNamespace = "";
  }

  public XmlQualifiedTagName(string str)
  {
    var ss = str.Split(':');
    if (ss.Length == 2)
    {
      XmlNamespace = ss[0];
      Name = ss[1];
    }
    else
    {
      XmlNamespace = "";
      Name = str;
    }
  }

  public XmlQualifiedTagName(string name, string? nspace)
  {
    XmlNamespace = nspace ?? "";
    Name = name;
  }

  public bool IsEmpty()
  {
    return Name == "";
  }

  public static QualifiedName Empty => new("");

  public int CompareTo(QualifiedName other)
  {
    var cmp = String.Compare(XmlNamespace, other.Namespace, StringComparison.Ordinal);
    if (cmp != 0) return cmp;
    return String.Compare(Name, other.Name, StringComparison.Ordinal);
  }


  public override string ToString()
  {
    if (XmlNamespace == "")
      return Name;
    return XmlNamespace + ":" + Name;
  }

  //public static implicit operator string(QualifiedName value) => value.ToString();
  //public static implicit operator QualifiedName(string value) => new QualifiedName(value);

  #region Equality members

  public bool Equals(QualifiedName other)
  {
    return Name == other.Name && XmlNamespace == other.Namespace;
  }

  public override bool Equals(object? obj)
  {
    return obj is QualifiedName other && Equals(other);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(XmlNamespace, Name);
  }

  //public static bool operator ==(QualifiedName @this, QualifiedName other) => @this.Equals(other);

  //public static bool operator !=(QualifiedName @this, QualifiedName other) => !@this.Equals(other);

  #endregion
}