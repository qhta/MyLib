﻿namespace Qhta.Xml.Reflection;

/// <summary>
/// Qualified name that contains a name and namespace
/// </summary>
[TypeConverter(typeof(QualifiedNameTypeConverter))]
public struct QualifiedName : IComparable<QualifiedName>, IEquatable<QualifiedName>
{
  /// <summary>
  /// Gets or sets the namespace.
  /// </summary>
  /// <value>
  /// The namespace.
  /// </value>
  [XmlAttribute] public string Namespace { get; set; }

  /// <summary>
  /// Gets or sets the name.
  /// </summary>
  /// <value>
  /// The name.
  /// </value>
  [XmlAttribute] public string Name { get; set; }

  /// <summary>
  /// Initializes a new instance of the <see cref="QualifiedName"/> struct.
  /// </summary>
  public QualifiedName()
  {
    Namespace = "";
    Name = "";
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="QualifiedName"/> struct
  /// using compound name.
  /// </summary>
  /// <param name="compoundName">The string thas consists of namespace, colon and name.</param>
  public QualifiedName(string compoundName)
  {
    if (!compoundName.Contains(':') && compoundName.Contains("."))
      compoundName = compoundName.ReplaceLast(".", ":");
    var ss = compoundName.Split(':');
    if (ss.Length == 2)
    {
      Namespace = ss[0];
      Name = ss[1];
    }
    else
    {
      Namespace = "";
      Name = compoundName;
    }
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="QualifiedName"/> struct.
  /// </summary>
  /// <param name="name">The name to init</param>
  /// <param name="nspace">The namespace to init</param>
  public QualifiedName(string name, string? nspace)
  {
    Namespace = nspace ?? "";
    Name = name;
  }

  /// <summary>
  /// Determines whether the name is an empty string.
  /// </summary>
  /// <returns>
  ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
  /// </returns>
  public bool IsEmpty()
  {
    return Name == "";
  }

  /// <summary>
  /// Gets the empty instance (with empty name).
  /// </summary>
  /// <value>
  /// The empty.
  /// </value>
  public static QualifiedName Empty => new("");

  /// <summary>
  /// Compares namespace first, then name.
  /// </summary>
  /// <param name="other">An object to compare with this instance.</param>
  /// <returns>
  /// A value that indicates the relative order of the objects being compared. The return value has these meanings:
  /// <list type="table"><listheader><term> Value</term><description> Meaning</description></listheader><item><term> Less than zero</term><description> This instance precedes <paramref name="other" /> in the sort order.</description></item><item><term> Zero</term><description> This instance occurs in the same position in the sort order as <paramref name="other" />.</description></item><item><term> Greater than zero</term><description> This instance follows <paramref name="other" /> in the sort order.</description></item></list>
  /// </returns>
  public int CompareTo(QualifiedName other)
  {
    var cmp = String.Compare(Namespace, other.Namespace, StringComparison.Ordinal);
    if (cmp != 0) return cmp;
    return String.Compare(Name, other.Name, StringComparison.Ordinal);
  }

  /// <summary>
  /// Converts to string that consists of namespace, colon and name.
  /// </summary>
  /// <returns>
  /// A <see cref="System.String" /> that represents this instance.
  /// </returns>
  public override string ToString()
  {
    if (Namespace == "")
      return Name;
    return Namespace + ":" + Name;
  }

  /// <summary>
  /// Performs an implicit conversion from <see cref="QualifiedName"/> to <see cref="System.String"/>.
  /// </summary>
  /// <param name="value">The value.</param>
  /// <returns>
  /// The result of the conversion.
  /// </returns>
  public static implicit operator string(QualifiedName value)
  {
    return value.ToString();
  }

  /// <summary>
  /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="QualifiedName"/>.
  /// </summary>
  /// <param name="value">The value.</param>
  /// <returns>
  /// The result of the conversion.
  /// </returns>
  public static implicit operator QualifiedName(string value)
  {
    return new(value);
  }

  #region Equality members

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
    return (Namespace+"."+Name).GetHashCode();
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