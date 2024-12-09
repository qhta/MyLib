using System;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Options for trimming paragraphs.
/// </summary>
[Flags]
public enum TrimOptions
{
  /// <summary>
  /// Trim white spaces at the start of the paragraph.
  /// </summary>
  TrimStart = 1,
  /// <summary>
  /// Trim white spaces at the end of the paragraph.
  /// </summary>
  TrimEnd = 2,
}