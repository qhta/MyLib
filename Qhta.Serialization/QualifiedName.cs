namespace Qhta.Xml.Serialization;

[TypeConverter(typeof(QualifiedNameTypeConverter))]
public struct QualifiedName : IComparable<QualifiedName>, IEquatable<QualifiedName>
{
  [XmlAttribute] public string Namespace { get; set; }

  [XmlAttribute] public string Name { get; set; }

  public QualifiedName()
  {
    Namespace = "";
    Name = "";
  }

  public QualifiedName(string str)
  {
    if (!str.Contains(':') && str.Contains("."))
      str = str.ReplaceLast(".", ":");
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

  public QualifiedName(string name, string? nspace)
  {
    Namespace = nspace ?? "";
    Name = name;
  }

  public bool IsEmpty()
  {
    return Name == "";
  }

  public static QualifiedName Empty => new("");

  public int CompareTo(QualifiedName other)
  {
    var cmp = String.Compare(Namespace, other.Namespace, StringComparison.Ordinal);
    if (cmp != 0) return cmp;
    return String.Compare(Name, other.Name, StringComparison.Ordinal);
  }

  public override string ToString()
  {
    if (Namespace == "")
      return Name;
    return Namespace + ":" + Name;
  }

  public static implicit operator string(QualifiedName value)
  {
    return value.ToString();
  }

  public static implicit operator QualifiedName(string value)
  {
    return new(value);
  }

  #region Equality members

  public bool Equals(QualifiedName other)
  {
    return Name == other.Name && Namespace == other.Namespace;
  }

  public override bool Equals(object? obj)
  {
    return obj is QualifiedName other && Equals(other);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(Namespace, Name);
  }

  public static bool operator ==(QualifiedName @this, QualifiedName other)
  {
    return @this.Equals(other);
  }

  public static bool operator !=(QualifiedName @this, QualifiedName other)
  {
    return !@this.Equals(other);
  }

  #endregion
}