using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;


namespace Qhta.Collections;

/// <summary>
/// Dictionary of (string, TValue), which can contains wildcard '*' character, that can be encompassed to a fragment of string.
/// </summary>
public class WildcardStringDictionary<TValue> : Dictionary<string, TValue>
{
  /// <summary>
  /// Default constructor using <see cref="WildcardStringComparer"/>
  /// </summary>
  public WildcardStringDictionary() : base(new WildcardStringComparer())
  {
  }

  /// <summary>
  /// Constructor using <see cref="WildcardStringComparer"/> and specifying StringComparison
  /// </summary>
  public WildcardStringDictionary(StringComparison comparison) : base(new WildcardStringComparer(comparison))
  {
  }

  /// <summary>
  /// Constructor using <see cref="WildcardStringComparer"/> and specifying StringComparison
  /// </summary>
  public WildcardStringDictionary(StringComparer comparer) : base(comparer)
  {
  }

  /// <summary>
  /// Founds exact key string or key which is like str and returns value
  /// </summary>
  /// <param name="str"></param>
  /// <param name="value"></param>
  /// <returns></returns>             
#if NETSTANDARD2_0
  public new bool TryGetValue(string str, out TValue? value)
#else
  public new bool TryGetValue(string str, [MaybeNullWhen(false)][NotNullWhen(true)] out TValue? value)
#endif
  {
    if (base.TryGetValue(str, out value) && value != null) return true;
    foreach (var item in this)
    {
      if (item.Key.Contains("*") && Qhta.TextUtils.StringUtils.IsLike(str, item.Key))
      {
        if (item.Value != null)
        {
          value = item.Value;
          return true;
        }
      }
    }
    value = default(TValue);
    return false;
  }
}