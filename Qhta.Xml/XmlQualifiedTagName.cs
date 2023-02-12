namespace Qhta.Xml;

//[TypeConverter(typeof(QualifiedNameTypeConverter))]
public record XmlQualifiedTagName: /*IComparable<QualifiedName>, */IEquatable<XmlQualifiedName>
{
  [XmlAttribute] public string Name { get; set; }

  [XmlAttribute] public string XmlNamespace { get; set; }

  [XmlAttribute] public string? Prefix { get; set; } = null;

  [XmlIgnore] public string Namespace => Prefix ?? XmlNamespace;

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

  //public static XmlQualifiedName Empty => new("");

  //public int CompareTo(XmlQualifiedName other)
  //{
  //  var cmp = String.Compare(XmlNamespace, other.Namespace, StringComparison.Ordinal);
  //  if (cmp != 0) return cmp;
  //  return String.Compare(Name, other.Name, StringComparison.Ordinal);
  //}


  public override string ToString()
  {
    if (!String.IsNullOrEmpty(XmlNamespace))
      return XmlNamespace + ":" + Name;
    if (!String.IsNullOrEmpty(Prefix))
      return Prefix + ":" + Name;
    return Name;
  }

  //public static implicit operator string(QualifiedName value) => value.ToString();
  public static implicit operator XmlQualifiedTagName(string value) => new XmlQualifiedTagName(value);

  #region Equality members

  public bool Equals(XmlQualifiedName? other)
  {
    return Name == other?.Name && XmlNamespace == other?.Namespace;
  }

  //public override bool Equals(object? obj)
  //{
  //  return obj is QualifiedName other && Equals(other);
  //}

  public override int GetHashCode()
  {
    return new { Name, XmlNamespace }.GetHashCode();
  }

  //public static bool operator ==(QualifiedName @this, QualifiedName other) => @this.Equals(other);

  //public static bool operator !=(QualifiedName @this, QualifiedName other) => !@this.Equals(other);

  #endregion

  public static XmlQualifiedTagName operator +(XmlQualifiedTagName value, string str)
  {
    return new XmlQualifiedTagName{ Name = value.Name + str, XmlNamespace = value.XmlNamespace, Prefix = value.Prefix };
  }
}