namespace Qhta.Xml.Serialization;

public struct QualifiedName: IComparable<QualifiedName>
{
  [XmlAttribute]
  public string Namespace { get; set; }

  [XmlAttribute]
  public string Name { get; set; }

  public QualifiedName()
  {
    Namespace = "";
    Name = "";
  }

  public QualifiedName(string name)
  {
    Namespace = "";
    Name = name;
  }

  public QualifiedName(string name, string? nspace)
  {
    Namespace = nspace ?? "";
    Name = name;
  }

  public bool IsEmpty() => Name == "" && Namespace == "";

  public int CompareTo(QualifiedName other)
  {
    var cmp = String.Compare(Namespace, other.Namespace, StringComparison.Ordinal);
    if (cmp != 0) return cmp;
    return String.Compare(Name, other.Name, StringComparison.Ordinal);
  }

  public override string ToString()
  {
    if (Namespace=="")
      return Name;
    return Namespace+":"+Name;
  }
}