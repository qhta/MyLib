using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
#nullable enable

namespace Qhta.TestHelper
{
  public class MessageCountingMonitor : ITraceMonitor
  {

    public MessageCountingMonitor(string filename)
    {
      Filename = filename;
    }

    private string Filename { get; init; }

    private SortedDictionary<string, SortedDictionary<String, int>> Messages = new SortedDictionary<string, SortedDictionary<string, int>>();

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

    public void Flush()
    {
      lock (this)
      {
        var filename = Filename;
        using (var writer = File.CreateText(filename))
        {
          foreach (var item in Messages)
          {
            writer.WriteLine(item.Key);
            foreach (var item2 in item.Value)
              writer.WriteLine($"\t{item2.Key.Replace("\n", "\\n").Replace(" ", "\\s")}\t{item2.Value}");
          }
        }
      }
    }

    public void Clear()
    {
      lock (this)
        Messages.Clear();
    }

  }
}
