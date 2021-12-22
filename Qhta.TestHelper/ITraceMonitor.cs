using System.Runtime.CompilerServices;
#nullable enable

namespace Qhta.TestHelper
{
  public interface ITraceMonitor
  {
    void Notify(string message, [CallerMemberName] string? callerName = null);

    bool WasNotified(string message, [CallerMemberName] string? callerName = null);

    void Flush();

    void Clear();
  }
}
