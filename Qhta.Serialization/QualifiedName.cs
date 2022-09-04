namespace Qhta.Xml.Serialization;

public class QualifiedName: IComparable<QualifiedName>
{
  public string Namespace { get; }
  public string Name { get;}

  public QualifiedName(string name)
  {
    Namespace = "";
    Name = name;
  }

  public QualifiedName(string name, string nspace)
  {
    Namespace = nspace;
    Name = name;
  }

  public int CompareTo(QualifiedName? other)
  {
    if (other == null) return 1;
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