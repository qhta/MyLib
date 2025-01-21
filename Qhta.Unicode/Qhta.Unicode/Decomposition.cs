using System.Globalization;
using System.Text;

namespace Qhta.Unicode;

/// <summary>
/// Decoded Unicode decomposition.
/// </summary>
public class Decomposition
{
  /// <summary>
  /// Gets or sets the decomposition type.
  /// </summary>
  public DecompositionType? Type { get; set; }
  /// <summary>
  /// Gets or sets the code points.
  /// </summary>
  public List<int> CodePoints { get; set; } = new();

  #region implicit operators
  /// <summary>
  /// Implicit conversion from Decomposition to string.
  /// </summary>
  /// <param name="decomposition"></param>
  public static implicit operator string?(Decomposition? decomposition)
  {
    if (decomposition is null)
      return null;
    return decomposition.ToString();
  }

  /// <summary>
  /// Implicit conversion from string to Decomposition.
  /// </summary>
  /// <param name="name"></param>
  public static implicit operator Decomposition?(string? name)
  {
    if (name is null)
      return null;
    return Parse(name);
  }
  #endregion

  /// <summary>
  /// Parses a decomposition from a string.
  /// </summary>
  /// <param name="name"></param>
  /// <returns></returns>
  /// <exception cref="FormatException"></exception>
  public static Decomposition Parse(string name)
  {
    var ss = name.Split([' '], StringSplitOptions.RemoveEmptyEntries);
    if (ss.Length == 0)
      return new Decomposition();
    int i = 0;
    DecompositionType? type = null;
    if (ss[0].StartsWith("<"))
    {
      i = 1;
      if (ss[0].EndsWith(">"))
      {
        if (!Enum.TryParse<DecompositionType>(ss[0].Substring(1, ss[0].Length-2), true, out var val))
          throw new FormatException($"Invalid decomposition type {ss[0]}");
        type = val;
      }
      else
        throw new FormatException("Invalid decomposition format");
    }
    var result = new Decomposition { Type = type };
    for (; i < ss.Length; i++)
    {
      if (!int.TryParse(ss[i], NumberStyles.HexNumber, null, out var codePoint))
        throw new FormatException($"Invalid code point {ss[i]}");
      result.CodePoints.Add(codePoint);
    }
    return result;
  }

  /// <summary>
  /// Converts the decomposition to a string.
  /// </summary>
  /// <returns></returns>
  public override string ToString()
  {
    var sb = new StringBuilder();
    if (Type!=null)
    {
      sb.Append($"<{Type.ToString()?.ToLower()}>");
      sb.Append(' ');
    }
    foreach (var codePoint in CodePoints)
    {
      sb.Append(codePoint.ToString("X4"));
      sb.Append(' ');
    }
    return sb.ToString().TrimEnd();
  }

}
