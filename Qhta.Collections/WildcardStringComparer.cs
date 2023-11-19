using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Qhta.Collections;

/// <summary>
/// String comparer that uses wildcard '*' character in str2, that can be encompassed to a fragment of string.
/// </summary>
public class WildcardStringComparer : StringComparer, IEqualityComparer<string>, IComparer<string>
{

  /// <summary>
  /// Default constructor
  /// </summary>
  public WildcardStringComparer() { }

  /// <summary>
  /// Constructor that enables letter case ignore.
  /// </summary>
  /// <param name="stringComparison"></param>
  public WildcardStringComparer(StringComparison stringComparison)
  {
    Comparison = stringComparison;
  }

  /// <summary>
  /// Specifies whether letter case is ignored.
  /// </summary>
  public StringComparison Comparison { get; set; }

  /// <inheritdoc/>
  public override bool Equals(string? str1, string? str2)
  {
    if (str1 == null || str2 == null)
      return String.Equals(str1, str2, Comparison);
    return str1 == str2
      || str2.Contains("*") && Qhta.TextUtils.StringUtils.IsLike(str1, str2);
  }

  /// <inheritdoc/>
  public override int Compare(string? str1, string? str2)
  {
    if (str1 == null || str2 == null)
      return String.Compare(str1, str2, Comparison);
    if (str2.Contains("*"))
    {
      if (Qhta.TextUtils.StringUtils.IsLike(str1, str2))
        return 0;
    }
    return String.Compare(str1, str2, Comparison);
  }

  /// <inheritdoc/>
  public override int GetHashCode(string obj)
  {
    return obj.GetHashCode();
  }
}
