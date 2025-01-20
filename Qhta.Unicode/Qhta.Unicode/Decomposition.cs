using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.Unicode;

/// <summary>
/// Decoded Unicode decomposition.
/// </summary>
public class Decomposition
{
  public DecompositionType? Type { get; set; }
  public List<int> CodePoints { get; set; } = new();

  public static implicit operator string(Decomposition hashedName)
  {
    return hashedName.ToString();
  }

  public static implicit operator Decomposition?(string? name)
  {
    if (name is null)
      return null;
    var ss = name.Split([' '], StringSplitOptions.RemoveEmptyEntries);
    if (ss.Length == 0)
      return new Decomposition();
    int i = 0;
    DecompositionType? type = null;
    if (ss[0].StartsWith('<'))
    {
      i = 1;
      if (ss[0].EndsWith('>'))
      {
        if (!Enum.TryParse<DecompositionType>(ss[0][1..^1], true, out var val))
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

  public override string ToString()
  {
    var sb = new StringBuilder();
    if (Type!=null)
    {
      sb.Append($"<{Type.ToString().ToLower()}>");
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
