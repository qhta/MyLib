using System.Collections.Generic;
using System.Linq;

namespace Qhta.Collections;

/// <summary>
/// It is a sorted set of strings, which can contains wildcard '*' character, that can be encompassed to a fragment of string.
/// </summary>
public class WildcardStrings : SortedSet<string>
{
  /// <summary>
  /// Overriden "Contains" funtions that uses "IsLike" method.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public override bool Contains(string str)
  {
    return this.AsEnumerable<string>().Any(item => item == str
    //|| item.StartsWith("*") && item.EndsWith("*") && str.Contains(item.Substring(1, item.Length - 2))
    //|| item.StartsWith("*") && str.EndsWith(item.Substring(1))
    //|| item.EndsWith("*") && str.StartsWith(item.Substring(0, item.Length - 1))
    || item.Contains("*") && Qhta.TextUtils.StringUtils.IsLike(str, item));
  }
}