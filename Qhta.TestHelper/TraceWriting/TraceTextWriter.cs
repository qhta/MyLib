using System;
using System.IO;


namespace Qhta.TestHelper
{
  public class TraceTextWriter : TraceWriter, ITraceTextWriter
  {

    public TraceTextWriter(bool consoleOutputEnabled, bool traceOutputEnabled)
    {
      LockObject = this;
      ConsoleOutputEnabled = consoleOutputEnabled;
      TraceOutputEnabled = traceOutputEnabled;
    }

    public TraceTextWriter(Stream stream, bool consoleOutputEnabled = false, bool traceOutputEnabled = false)
    {
      LockObject = this;
      OutputStream = stream;
      _writer = new StreamWriter(OutputStream);
      StreamOutputEnabled = true;
      ConsoleOutputEnabled = consoleOutputEnabled;
      TraceOutputEnabled = traceOutputEnabled;
    }

    public TraceTextWriter(string? filename, bool consoleOutputEnabled, bool traceOutputEnabled = false)
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
    }

    public TraceTextWriter(ITraceTextWriter origin)
    {
      LockObject = origin.LockObject;
      OutputStream = origin.OutputStream;
      StreamOutputEnabled = origin.StreamOutputEnabled;
      TraceOutputEnabled = origin.TraceOutputEnabled;
      ConsoleOutputEnabled = origin.ConsoleOutputEnabled;
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
    protected TextWriter? _writer {get; private set; }

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
}
