using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Qhta.TestHelper
{
  /// <summary>
  /// Trace monitor that counts registered messages
  /// </summary>
  public class MessageCountingMonitor : ITraceMonitor
  {

    /// <summary>
    /// Creates monitor for a file, console or trace output stream is created on flush.
    /// </summary>
    /// <param name="filename">name of output file</param>
    /// <param name="consoleOutputEnabled">if should output to console</param>
    /// <param name="traceOutputEnabled">if should output to trace</param>
    public MessageCountingMonitor(string? filename, bool consoleOutputEnabled = false, bool traceOutputEnabled = false)
    {
      Filename = filename;
      ConsoleOutputEnabled = consoleOutputEnabled;
      TraceOutputEnabled = traceOutputEnabled;
    }

    /// <summary>
    /// Output filename
    /// </summary>
    private string? Filename { get; init; }

    /// <summary>
    /// If monitor flushes to console
    /// </summary>
    public bool ConsoleOutputEnabled { get; set; }

    /// <summary>
    /// If monitor flushes to trace
    /// </summary>
    public bool TraceOutputEnabled { get; set; }

    /// <summary>
    /// Internal registry for incoming messages.
    /// </summary>
    private SortedDictionary<string, SortedDictionary<String, int>> Messages = new SortedDictionary<string, SortedDictionary<string, int>>();

    /// <summary>
    /// Check if a message was already registered.
    /// Enables omitting duplicate message processing.
    /// </summary>
    /// <param name="message">Text message</param>
    /// <param name="callerName">Caller name (by default set as name of caller method)</param>
    /// <returns>true if the message was already registered</returns>
    public bool WasNotified(string msg, [CallerMemberName] string? callerName = null)
    {
      lock (this)
      {
        if (callerName == null)
          callerName = string.Empty;
        if (Messages.TryGetValue(callerName, out var messagesForCaller))
        {
          if (messagesForCaller.TryGetValue(msg, out var delimiterCounter))
            return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Register a message send by a caller.
    /// </summary>
    /// <param name="message">Text message</param>
    /// <param name="callerName">Caller name (by default set as name of caller method)</param>
    public void Notify(string msg, [CallerMemberName] string? callerName = null)
    {
      lock (this)
      {
        if (callerName == null)
          callerName = string.Empty;
        if (!Messages.TryGetValue(callerName, out var messagesForCaller))
        {
          messagesForCaller = new SortedDictionary<string, int>();
          Messages.Add(callerName, messagesForCaller);
        }
        if (!messagesForCaller.TryGetValue(msg, out var delimiterCounter))
          messagesForCaller.Add(msg, 1);
        else
          messagesForCaller[msg] += 1;
      }
    }

    /// <summary>
    /// Flush creates an output
    /// </summary>
    public virtual void Flush()
    {
      lock (this)
      {
        using (var writer = new TraceTextWriter(Filename, ConsoleOutputEnabled, TraceOutputEnabled))
        {
          foreach (var item in Messages)
          {
            writer.WriteLine(item.Key);
            foreach (var item2 in item.Value)
            {
              writer.WriteLine($"\t{item2.Key}\t{item2.Value}");
            }
          }
        }
      }
    }

    /// <summary>
    /// Clears the internal registry.
    /// </summary>
    public void Clear()
    {
      lock (this)
        Messages.Clear();
    }

  }
}
