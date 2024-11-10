using System;
using System.Collections.Generic;
using System.Text;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Represents the properties of a text. These properties are used to format the text.
/// </summary>
public record TextProperties
{

  /// <summary>
  /// Type of the script used to select the appropriate font.
  /// </summary>
  public ScriptType? ScriptType { get; set; }

  /// <summary>
  /// Name of the font.
  /// </summary>
  public string? FontName { get; set; }

  /// <summary>
  /// Size of the font.
  /// </summary>
  public int? FontSize { get; set; }

  /// <summary>
  /// Bold attribute of the text.
  /// </summary>
  public bool? Bold { get; set; }

  /// <summary>
  /// Italic attribute of the text.
  /// </summary>
  public bool? Italic { get; set; }

  /// <summary>
  /// StrikeThrough attribute of the text (0 - none, 1 - single, 2 - double).
  /// </summary>
  public int? StrikeThrough { get; set; }

  /// <summary>
  /// Underline attribute of the text.
  /// </summary>
  public DXW.UnderlineValues? Underline { get; set; }

  /// <summary>
  /// Underline color. Valid if underline attribute is set.
  /// </summary>
  public DXO13W.Color? UnderlineColor { get; set; }
}

