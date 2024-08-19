using System;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Encoding for the <c>DocumentSecurity</c>> property
/// </summary>
[Flags]
public enum DocumentSecurity
{
  /// <summary>
  /// Document is password protected
  /// </summary>
  PasswordProtected = 1,

  /// <summary>
  ///  Document is recommended to be opened as read-only
  /// </summary>
  ReadOnlyRecommended = 2,

  /// <summary>
  ///  Document is recommended to be opened as read-only
  /// </summary>
  ReadOnlyForced = 4,

  /// <summary>
  ///  Document is locked for annotation
  /// </summary>
  LockedForAnnotation = 8,

}
