using System;

#nullable enable

namespace Qhta.TestHelper;

/// <summary>
/// Defines operations needed to write text in color to console. Extends ITextWriter.
/// </summary>
public interface IConsoleWriter: ITextWriter
{
  /// <summary>
  /// If color operations will be enabled
  /// </summary>
  bool ColorEnabled { get; set; }
  /// <summary>
  /// Represents a color of text foreground.
  /// </summary>
  ConsoleColor ForegroundColor {get; set; }

  /// <summary>
  /// Represents a color of text background.
  /// </summary>
  ConsoleColor BackgroundColor { get; set; }

  /// <summary>
  /// Resets colors to the default values.
  /// </summary>
  void ResetColors();

}