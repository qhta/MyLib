using System;
using System.Collections.Generic;

namespace Qhta.Collections;

/// <summary>
/// Dictionary of (string, string), which can contains wildcard '*' character, that can be encompassed to a fragment of string.
/// </summary>
public class WildcardStringDictionary : WildcardStringDictionary<string>
{
  /// <summary>
  /// Default constructor using <see cref="WildcardStringComparer"/>
  /// </summary>
  public WildcardStringDictionary(): base()
  {
  }

  /// <summary>
  /// Constructor using <see cref="WildcardStringComparer"/> and specifying StringComparison
  /// </summary>
  public WildcardStringDictionary(StringComparison comparison): base(comparison)
  {
  }

  /// <summary>
  /// Constructor using <see cref="WildcardStringComparer"/> and specifying StringComparison
  /// </summary>
  public WildcardStringDictionary(StringComparer comparer): base(comparer)
  {
  }
}