using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Abstract class for reading OpenXml documents.
/// </summary>
public abstract class OpenXmlTextReader
{
  /// <summary>
  /// Pass the options to the derived class.
  /// </summary>
  public TextOptions Options { get; set; } = null!;

  private string buffer = "";
  private int index;

  /// <summary>
  /// Check if the end of the buffer has been reached.
  /// </summary>
  /// <returns></returns>
  public bool EOF() => index>=buffer.Length; 
  
  /// <summary>Returns the next available character but does not consume it.</summary>
  public char Peek() => EOF() ? (char)0 : buffer[index];

  /// <summary>
  /// 
  /// </summary>
  public int Length => buffer.Length;

  /// <summary>
  /// Current position in the buffer.
  /// </summary>
  public int Position => index;

  /// <summary>
  /// Move current position.
  /// </summary>
  public void Seek(int offset, SeekOrigin origin)
  {
    if (origin == SeekOrigin.Begin)
      index = offset;
    else if (origin == SeekOrigin.Current)
      index += offset;
    else if (origin == SeekOrigin.End)
      index = buffer.Length - offset;
  }

  /// <summary>
  /// Read a string
  /// </summary>
  public virtual string ReadString()
  {
    var sb = new StringBuilder();
    while (!EOF())
    {
      var ch = ReadChar();
      sb.Append(ch);
    }
    return sb.ToString();
  }

  /// <summary>
  /// Read a single char
  /// </summary>
  public virtual char ReadChar()
  {
    if (EOF())
      throw new InvalidOperationException("No more characters available.");
    return buffer[index++];
  }

  /// <summary>
  /// Decode a string read from OpenXml.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  public string Decode (string str)
  {
    buffer = str;
    index = 0;
    return ReadString();
  }
}
