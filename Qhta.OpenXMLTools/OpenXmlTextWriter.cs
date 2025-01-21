using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Abstract class for writing OpenXml documents.
/// </summary>
public abstract class OpenXmlTextWriter
{
  /// <summary>
  /// Pass the options to the derived class.
  /// </summary>
  public TextOptions Options { get; set; } = null!;

  private readonly StringBuilder sb = new StringBuilder();

  /// <summary>
  /// Write a string
  /// </summary>
  /// <param name="str"></param>
  public virtual void Write(string str)
  {
    sb.Append(str);
  }

  /// <summary>
  /// Write a single char
  /// </summary>
  /// <param name="ch"></param>
  public virtual void Write(char ch)
  {
    sb.Append(ch);
  }

  /// <summary>
  /// Encode a string for OpenXml.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public string EncodeString(string str)
  {
    sb.Clear();
    Write(str);
    return sb.ToString();
  }

  /// <summary>
  /// Clear the buffer.
  /// </summary>
  public virtual void Clear()
  {
    sb.Clear();
  }
}
