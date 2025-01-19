using System;
using System.Collections.Generic;
using System.Text;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Utilities for working with characters.
/// </summary>
public static class CharUtils
{
  /// <summary>
  /// A set of characters that can be used as superscript
  /// </summary>
  public static readonly char[] SupChars =
  {
    '\u2070', '\u00B9', '\u00B2', '\u00B3', '\u2074', '\u2075', '\u2076', '\u2077',
    '\u2078', '\u2079', '\u207A', '\u207B', '\u207C', '\u207D', '\u207E', '\u2071', '\u207F',
  };

  /// <summary>
  /// A set of characters that can be used as subscript
  /// </summary>
  public static readonly char[] SubChars =
  {
    '\u2080', '\u2081', '\u2082', '\u2083', '\u2084', '\u2085', '\u2086', '\u2087', '\u2088',
    '\u2089', '\u208A', '\u208B', '\u208C', '\u208D', '\u208E', '\u2090', '\u2091', '\u2092',
    '\u2093', '\u2094', '\u2095', '\u2096', '\u2097', '\u2098', '\u2099', '\u209A', '\u209B', '\u209C', '\u2C7C',
  };

  /// <summary>
  /// A set of characters that represent roman numerals
  /// </summary>
  public static readonly char[] RomanChars =
  {
    '\u2160', '\u2161', '\u2162', '\u2163', '\u2164', '\u2165', '\u2166', '\u2167',
    '\u2168', '\u2169', '\u216A', '\u216B', '\u216C', '\u216D', '\u216E', '\u216F',
    '\u2170', '\u2171', '\u2172', '\u2173', '\u2174', '\u2175', '\u2176', '\u2177',
    '\u2178', '\u2179', '\u217A', '\u217B', '\u217C', '\u217D', '\u217E', '\u217F',
  };
}
