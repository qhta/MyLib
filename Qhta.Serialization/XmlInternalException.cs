using System;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Qhta.Xml.Serialization;

/// <summary>
/// Class extending <see cref="Exception"/> to hold
/// <see cref="LineNumber"/> and <see cref="LinePosition"/>.
/// </summary>
public class XmlInternalException: Exception
{
  /// <summary>
  /// Extending constructor. 
  /// If <paramref name="xmlReader"/> is <see cref="XmlTextReader"/>
  /// then <see cref="LineNumber"/> and <see cref="LinePosition"/> are set appriopriately.
  /// </summary>
  /// <param name="message"></param>
  /// <param name="xmlReader"></param>
  /// <param name="innerException"></param>
  /// <param name="methodName"></param>
  public XmlInternalException(string message, XmlReader xmlReader, Exception? innerException = null,
    [CallerMemberName] string? methodName = null) : base(ComposeMessage(message, xmlReader), innerException)
  {
    if (xmlReader is XmlTextReader xmlTextReader)
    {
      LineNumber = xmlTextReader.LineNumber;
      LinePosition = xmlTextReader.LinePosition;
    }
  }

  protected static string ComposeMessage(string message, XmlReader xmlReader)
  {
    if (xmlReader is XmlTextReader xmlTextReader)
    {
      var lineNumber = xmlTextReader.LineNumber;
      var linePosition = xmlTextReader.LinePosition;
      return message + $" in line {lineNumber}  at position {linePosition}";
    }
    return message;
  }

  public int? LineNumber { get;}
  public int? LinePosition { get;}
}