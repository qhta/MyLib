﻿using System.Runtime.CompilerServices;

namespace Qhta.Xml.Serialization;

/// <summary>
///   Class extending <see cref="Exception" /> to hold
///   <see cref="LineNumber" /> and <see cref="LinePosition" />.
/// </summary>
public class XmlInvalidOperationException : Exception
{
  /// <summary>
  ///   Extending constructor.
  ///   If <paramref name="xmlReader" /> is <see cref="XmlTextReader" />
  ///   then <see cref="LineNumber" /> and <see cref="LinePosition" /> are set appriopriately.
  /// </summary>
  /// <param name="message"></param>
  /// <param name="xmlReader"></param>
  /// <param name="innerException"></param>
  /// <param name="methodName"></param>
  public XmlInvalidOperationException(string message, IXmlReader xmlReader, Exception? innerException = null,
    [CallerMemberName] string? methodName = null) : base(ComposeMessage(message, xmlReader), innerException)
  {
    LineNumber = xmlReader.LineNumber;
    LinePosition = xmlReader.LinePosition;
  }

  /// <summary>
  /// Gets line number of XML text file where the exception occured.
  /// </summary>
  public int? LineNumber { get; }

  /// <summary>
  /// Gets the position in line of XML text file where the exception occured.
  /// </summary>
  public int? LinePosition { get; }

  /// <summary>
  /// Composes message adding "in line ... at position ...
  /// </summary>
  protected static string ComposeMessage(string message, XmlReader xmlReader)
  {
    if (xmlReader is XmlTextReader xmlTextReader)
    {
      var lineNumber = xmlTextReader.LineNumber;
      var linePosition = xmlTextReader.LinePosition;
      if (lineNumber != 0)
      {
        if (linePosition != 0)
          return message + $" in line {lineNumber} at position {linePosition}";
        else
          return message + $" in line {lineNumber}";
      }
    }
    return message;
  }

  /// <summary>
  /// Composes message adding "in line ... at position ...
  /// </summary>
  protected static string ComposeMessage(string message, IXmlReader xmlReader)
  {
    var lineNumber = xmlReader.LineNumber;
    var linePosition = xmlReader.LinePosition;
    if (lineNumber != null && lineNumber != 0)
    {
      if (linePosition != null && linePosition != 0)
        return message + $" in line {lineNumber} at position {linePosition}";
      else
        return message + $" in line {lineNumber}";
    }
    return message;
  }
}