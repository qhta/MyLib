using System.IO;


namespace Qhta.TestHelper;

/// <summary>
/// Main implementation of <see cref="ITraceTextWriter"/>
/// </summary>
/// <seealso cref="Qhta.TestHelper.TraceWriter" />
/// <seealso cref="Qhta.TestHelper.ITraceTextWriter" />
public class TraceTextWriter : TraceWriter, ITraceTextWriter
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

  public TraceTextWriter(bool consoleOutputEnabled, bool traceOutputEnabled, bool debugOutputEnabled)
  {
    LockObject = this;
    ConsoleOutputEnabled = consoleOutputEnabled;
    TraceOutputEnabled = traceOutputEnabled;
    DebugOutputEnabled = debugOutputEnabled;
  }

  public TraceTextWriter(Stream stream, bool consoleOutputEnabled = false, bool traceOutputEnabled = false, bool debugOutputEnabled = false)
  {
    LockObject = this;
    OutputStream = stream;
    _writer = new StreamWriter(OutputStream);
    StreamOutputEnabled = true;
    ConsoleOutputEnabled = consoleOutputEnabled;
    TraceOutputEnabled = traceOutputEnabled;
    DebugOutputEnabled = debugOutputEnabled;
  }

  public TraceTextWriter(string? filename, bool consoleOutputEnabled, bool traceOutputEnabled = false, bool debugOutputEnabled = false)
  {
    LockObject = this;
    if (filename != null)
    {
      OutputStream = File.Create(filename);
      _writer = new StreamWriter(OutputStream);
      StreamOutputEnabled = true;
    }
    ConsoleOutputEnabled = consoleOutputEnabled;
    TraceOutputEnabled = traceOutputEnabled;
    DebugOutputEnabled = debugOutputEnabled;
  }

  public TraceTextWriter(ITraceTextWriter origin)
  {
    LockObject = origin.LockObject;
    OutputStream = origin.OutputStream;
    StreamOutputEnabled = origin.StreamOutputEnabled;
    TraceOutputEnabled = origin.TraceOutputEnabled;
    ConsoleOutputEnabled = origin.ConsoleOutputEnabled;
    DebugOutputEnabled = origin.DebugOutputEnabled;
  }

  /// <summary>
  /// Enables lock operation
  /// </summary>
  public object LockObject { get; set; }

  /// <summary>
  /// Controls if text is sent to stream
  /// </summary>
  public bool StreamOutputEnabled { get; set; }

  /// <summary>
  /// Output stream
  /// </summary>
  public Stream? OutputStream
  {
    get => _outputStream;
    set
    {
      if (value != _outputStream)
      {
        if (_outputStream != null)
          Flush();
        _outputStream = value;
        if (_outputStream != null)
        {
          _writer = new StreamWriter(_outputStream);
          StreamOutputEnabled = true;
        }
        else
          StreamOutputEnabled = false;
      }
    }
  }

  /// <summary>
  /// Set by <see cref="OutputStream"/> set method
  /// </summary>
  protected Stream? _outputStream { get; private set; }


  /// <summary>
  /// Usually set to <see cref="_outputStream"/>
  /// </summary>
  protected TextWriter? _writer { get; private set; }

  /// <summary>
  /// This implementation uses <see cref="TraceWriter.FlushString"/> operation first
  /// and then a string is sent to stream text writer(if <see cref="StreamOutputEnabled"/> is set).
  /// </summary>
  /// <param name="str"></param>
  protected override void FlushString(string str)
  {
    base.FlushString(str);
    if (StreamOutputEnabled)
      _writer?.Write(str);
  }

  /// <summary>
  /// This implementation uses <see cref="TraceWriter.FlushNewLineTag"/> operation first
  /// and then a string is sent to stream text writer (if <see cref="StreamOutputEnabled"/> is set).
  /// </summary>
  protected override void FlushNewLineTag()
  {
    base.FlushNewLineTag();
    if (StreamOutputEnabled)
      _writer?.WriteLine("");
  }

  /// <summary>
  /// This flush is needed due to trace output.
  /// </summary>
  protected override void FlushBuffer()
  {
    base.FlushBuffer();
    if (StreamOutputEnabled)
      _writer?.Flush();
  }

}