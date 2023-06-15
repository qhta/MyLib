using System;
using System.Text;

namespace Qhta.TestHelper;

/// <summary>
/// Extends <see cref="BufferedTextWriter"/> with implementation of
/// color operations and Console output.
/// </summary>
public class ConsoleWriter : BufferedTextWriter, IConsoleWriter
{

  /// <summary>
  /// If color operations are enabled
  /// </summary>
  public bool ColorEnabled { get; set; } = true;

  /// <summary>
  /// If output strings are sent to console
  /// </summary>
  public bool ConsoleOutputEnabled { get; set; } = true;

  /// <summary>
  /// This constructor assures than initial colors would be the same
  /// for each instance;
  /// </summary>
  public ConsoleWriter()
  {
    Console.ResetColor();
    initialForegroundColor = ConsoleColor.White; //Console.ForegroundColor;
    initialBackgroundColor = ConsoleColor.Black;//Console.BackgroundColor;
  }

  #region initial console colors
  static ConsoleColor initialForegroundColor;
  static ConsoleColor initialBackgroundColor;
  #endregion

  /// <summary>
  /// The foreground color as get on init from console or as set by user.
  /// </summary>
  public ConsoleColor ForegroundColor
  {
    get => _ForegroundColor;
    set
    {
      if (_ForegroundColor != value)
      {
        _ForegroundColor = value;
        Buffer.Add(new ColorTag(ColorOp.Foreground, _ForegroundColor));
      }
    }
  }
  private ConsoleColor _ForegroundColor = initialForegroundColor;

  /// <summary>
  /// The background color as get on init from console or as set by user.
  /// </summary>
  public ConsoleColor BackgroundColor
  {
    get => _BackgroundColor;
    set
    {
      if (_BackgroundColor != value)
      {
        _BackgroundColor = value;
        Buffer.Add(new ColorTag(ColorOp.Background, _ForegroundColor));
      }
    }
  }
  private ConsoleColor _BackgroundColor = initialBackgroundColor;

  /// <summary>
  /// Color tags are stored to the buffer to reset colors.
  /// It enables concurrent write.
  /// </summary>
  public void ResetColors()
  {
    _ForegroundColor = initialForegroundColor;
    _BackgroundColor = initialBackgroundColor;
    Buffer.Add(new ColorTag(ColorOp.Reset));
  }

  /// <summary>
  /// Send a string to console, but only if it is enabled.
  /// This protected flush can be overriden in subclasses.
  /// You can control if console is used for output with
  /// <see cref="ConsoleOutputEnabled"/> property.
  /// </summary>
  /// <param name="str">Flushed string</param>
  protected virtual void FlushString(string str)
  {
    if (ConsoleOutputEnabled)
      Console.Write(str);
  }

  /// <summary>
  /// Send a new line character to console, but only if it is enabled.
  /// This protected flush can be overriden in subclasses.
  /// You can control if console is used for output with
  /// <see cref="ConsoleOutputEnabled"/> property.
  /// </summary>
  protected virtual void FlushNewLineTag()
  {
    if (ConsoleOutputEnabled)
      Console.Write("\n");
  }

  /// <summary>
  /// This method manipulates the color tag using console.
  /// This protected operation can be overriden in subclasses.
  /// You can control the operation is done with
  /// <see cref="ColorEnabled"/> property.
  /// </summary>
  protected virtual void FlushColorTag(ColorTag tag)
  {
    if (ConsoleOutputEnabled)
      if (ColorEnabled)
      {
        switch (tag.Type)
        {
          case ColorOp.Foreground:
            Console.ForegroundColor = tag.Color ?? _ForegroundColor;
            break;
          case ColorOp.Background:
            Console.BackgroundColor = tag.Color ?? _BackgroundColor;
            break;
          default:
            Console.ResetColor();
            _ForegroundColor = Console.ForegroundColor;
            _BackgroundColor = Console.BackgroundColor;
            break;
        }
      }
  }

  /// <summary>
  /// This implementation of flush operation sends strings to output via <see cref="FlushString"/>
  /// and manipulates of console colors via <see cref="FlushColorTag"/> operations.
  /// After flush the buffer is cleared.
  /// </summary>
  protected override void FlushBuffer()
  {
    //if (!ConsoleOutputEnabled) return;
    foreach (var item in Buffer)
    {
      if (item is string str)
      {
        FlushString(str);
      }
      if (item is NewLineTag)
      {
        FlushNewLineTag();
      }
      else if (item is ColorTag tag)
      {
        FlushColorTag(tag);
      }
    }
  }

  /// <summary>
  /// Implements TextWriter Encoding property.
  /// </summary>
  public override Encoding Encoding => Encoding.Unicode;
}