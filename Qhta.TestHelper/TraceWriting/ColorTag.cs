using System;

namespace Qhta.TestHelper;

/// <summary>
/// Color control element stored in a buffer
/// </summary>
public struct ColorTag
{
  /// <summary>
  /// Which operation is encoded
  /// </summary>
  public ColorOp Type;

  /// <summary>
  /// What specific color is stored
  /// </summary>
  public ConsoleColor? Color;

  /// <summary>
  /// Constructor forces at least color operation encoding.
  /// </summary>
  /// <param name="type">Color operation to store</param>
  /// <param name="color">Specific color to store</param>
  public ColorTag(ColorOp type, ConsoleColor? color = null) { Type = type; Color = color; }
}