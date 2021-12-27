using System.Runtime.CompilerServices;
#nullable enable

namespace Qhta.Erratum
{
  public interface IPatternMonitor
  {
    string CreatePattern(in string str);

    void Notify(string pattern, [CallerMemberName] string? callerName = null);

    bool WasNotified(string pattern, [CallerMemberName] string? callerName = null);

    void Flush();

    void Clear();
  }
}
