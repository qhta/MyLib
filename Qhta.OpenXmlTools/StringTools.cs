namespace Qhta.OpenXmlTools;

/// <summary>
/// Extending methods for strings.
/// </summary>
public static class StringTools
{
  /// <summary>
  /// Checks if a string has a substring at a given position.
  /// </summary>
  /// <param name="s"></param>
  /// <param name="pos"></param>
  /// <param name="substring"></param>
  /// <returns></returns>
  public static bool HasSubstringAt(this string s, int pos, string substring) => 
    (pos>=0 && pos+substring.Length<=s.Length) && s.Substring(pos, substring.Length).Equals(substring);
}