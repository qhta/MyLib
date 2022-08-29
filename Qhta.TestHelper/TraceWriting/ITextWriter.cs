using System;

#nullable enable

namespace Qhta.TestHelper;

/// <summary>
/// Defines operations needed to write text
/// </summary>
public interface ITextWriter: IDisposable
{
  /// <summary>
  /// If set, forces flush after each write operation.
  /// </summary>
  bool AutoFlush { get; set; }

  /// <summary>
  /// If set to false then disables operations.
  /// </summary>
  bool Enabled { get; set; }

  ///// <summary>
  ///// An object needed to enable parallel operations with a critical section
  ///// </summary>
  //object LockObject {get; set; }

  /// <summary>
  /// If set then new lines are written visually, as "\n" sequence;
  /// To so, to write a "real" new line use <see cref="WriteLine"/> method.
  /// </summary>
  bool WriteVisualNewLine { get; set; }

  /// <summary>
  /// If set then "\n" and "\s" sequences at start of the string to write are treated specially.
  /// "\n" forces to write a new line if last char is not a new line character.
  /// "\s" forces to write a space if last char is not a space and is not an open punctuation character.
  /// </summary>
  bool AllowIntelligentSpacing { get; set; }

  /// <summary>
  /// Last written character
  /// </summary>
  public char LastChar { get; }

  /// <summary>
  /// Writes a string to output. 
  /// Rules according to <see cref="WriteVisualNewLine"/>
  /// and <see cref="AllowIntelligentSpacing"/> are obeyed.
  /// </summary>
  /// <param name="str">Written string</param>
  void Write(string str);

  /// <summary>
  /// Writes a line of text to output.
  /// Rules according to <see cref="WriteVisualNewLine"/>
  /// and <see cref="AllowIntelligentSpacing"/> are obeyed.
  /// <see cref="IndentLevel"/> is written at the beginning of the line.
  /// </summary>
  /// <param name="line">A line of text to write</param>
  void WriteLine(string line);

  /// <summary>
  /// Writes an empty line to output.
  /// <see cref="IndentLevel"/> is written at the beginning of the line.
  /// </summary>
  /// <param name="line">A line of text to write</param>
  void WriteLine();

  /// <summary>
  /// Indent level of the written line.
  /// </summary>
  int IndentLevel { get; set; }

  /// <summary>
  /// Indent size counted in spaces.
  /// </summary>
  int IndentSize { get; set; }

  /// <summary>
  /// Increments <see cref="IndentLevel"/>
  /// </summary>
  void Indent();

  /// <summary>
  /// Decrements <see cref="IndentLevel"/>
  /// </summary>
  void Unindent();

  /// <summary>
  /// Flushes the written content to output.
  /// </summary>
  void Flush();

}