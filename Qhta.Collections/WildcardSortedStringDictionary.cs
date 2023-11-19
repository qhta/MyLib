using System;
using System.Collections.Generic;

namespace Qhta.Collections;

/// <summary>
/// Sorted dictionary of (string, string>, which can contains wildcard '*' character, that can be encompassed to a fragment of string.
/// </summary>
public class WildcardSortedStringDictionary : WildcardSortedStringDictionary<string>
{
  /// <summary>
  /// Default constructor using <see cref="WildcardStringComparer"/>
  /// </summary>
  public WildcardSortedStringDictionary(): base(new WildcardStringComparer())
  {
  }

  /// <summary>
  /// Constructor using <see cref="WildcardStringComparer"/> and specifying StringComparison
  /// </summary>
  public WildcardSortedStringDictionary(StringComparison comparison): base(new WildcardStringComparer(comparison))
  {
  }
}