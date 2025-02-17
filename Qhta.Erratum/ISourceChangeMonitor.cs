﻿using System.Runtime.CompilerServices;
#nullable enable

namespace Qhta.Erratum
{
  public interface ISourceChangeMonitor
  {
    void NotifyReplacement(string? file, int lineNum, string text, string replacement, string comment, [CallerMemberName] string? callerName = null);

    void Flush();

    void Clear();
  }
}
